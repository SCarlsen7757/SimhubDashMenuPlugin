using DashMenu.Data;
using DashMenu.Extensions;
using DashMenu.Settings;
using SimHub.Plugins;
using SimHub.Plugins.BrightnessControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DashMenu.FieldManager
{
    internal class DataFieldManager : FieldManagerBase, IFieldManager<Settings.DataField>
    {

        private static class PropertyNames
        {
            public const string AmountOfFields = "AmountOfDataFields";
        }
        private readonly PluginManager pluginManager;
        private readonly Type pluginType;
        internal ObservableCollection<IDataFieldExtension> SelectedFields { get; private set; } = new ObservableCollection<IDataFieldExtension>();
        protected readonly ObservableCollection<DataFieldComponent> allFields = new ObservableCollection<DataFieldComponent>();

        internal event SelectedFieldsChangedEventHandler SelectedFieldsChanged;

        internal DataFieldManager(PluginManager pluginManager, Type pluginType) : base()
        {
            this.pluginManager = pluginManager;
            this.pluginType = pluginType;

            this.pluginManager.AddProperty(PropertyNames.AmountOfFields, this.pluginType, SelectedFields.Count);
            SelectedFields.CollectionChanged += SelectedFields_CollectionChanged;

            SimhubHelper.AddNCalcFunction("dashfielddataname",
                "Returns the name of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Name));

            SimhubHelper.AddNCalcFunction("dashfielddatavalue",
                "Returns the value of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Value));

            SimhubHelper.AddNCalcFunction("dashfielddatadecimal",
                "Returns the number of decimals the value has of the data field of the specified field.",
                "index",
                engine => (Func<int, int>)(index => GetField(index - 1).Decimal));

            SimhubHelper.AddNCalcFunction("dashfielddataunit",
                "Returns the unit of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Unit));

            SimhubHelper.AddNCalcFunction("dashfielddatacolorprimary",
                "Returns the primary color of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Color.Primary));

            SimhubHelper.AddNCalcFunction("dashfielddatacoloraccent",
                "Returns the accent color of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Color.Accent));
        }

        private void SelectedFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    pluginManager.SetPropertyValue(PropertyNames.AmountOfFields, pluginType, SelectedFields.Count);
                    break;
                default:
                    break;
            }
        }

        public void AddExtensionField(Type type, IDictionary<string, Settings.DataField> settings)
        {
            IDataFieldExtension fieldInstance;
            try
            {
                fieldInstance = (IDataFieldExtension)Activator.CreateInstance(type, gameName);
            }
            catch (Exception e)
            {
                SimHub.Logging.Current.Error(type, e);
                return;
            }

            if (!fieldInstance.IsGameSupported) return;

            //Get field settings else create field settings
            if (!(settings.TryGetValue(type.FullName, out Settings.DataField fieldSetting)))
            {

                fieldSetting = new Settings.DataField
                {
                    Enabled = true,
                };
                fieldSetting.Override.Name = new PropertyOverride<string>(fieldInstance.Data.Name);
                fieldSetting.Override.Decimal = new PropertyOverride<int>(fieldInstance.Data.Decimal);
                fieldSetting.Override.DayNightColorScheme = new DayNightColorScheme(fieldInstance.Data.Color);
                settings.Add(type.FullName, fieldSetting);
            }

            //Make sure that empty field can't be disabled.
            if (fieldInstance.GetType().FullName == EmptyDataField.FullName) fieldSetting.Enabled = true;

            fieldSetting.Namespace = type.Namespace;
            fieldSetting.Name = type.Name;
            fieldSetting.FullName = type.FullName;
            fieldSetting.IsDecimal = fieldInstance.Data.IsDecimalNumber;

            //Get default values before they are overriden
            fieldSetting.Override.Name.DefaultValue = fieldInstance.Data.Name;
            fieldSetting.Override.Decimal.DefaultValue = fieldInstance.Data.Decimal;
            fieldSetting.Override.DayNightColorScheme.DayModeColor.DefaultValue = fieldInstance.Data.Color;
            fieldSetting.Override.DayNightColorScheme.NightModeColor.DefaultValue = fieldInstance.Data.Color;

            fieldSetting.GameSupported = fieldInstance.IsGameSupported;
            fieldSetting.SupportedGames = fieldInstance.SupportedGames;
            fieldSetting.Description = fieldInstance.Description;

            allFields.Add(new DataFieldComponent(fieldInstance) { Enabled = fieldSetting.Enabled });

            UpdateNameOverride(fieldSetting);
            UpdateColorOveride(fieldSetting);
            UpdateDecimalOverride(fieldSetting);
        }

        public void UpdateSelectedFields(Settings.ICarFields carFields)
        {
            UpdateSelectedFields(carFields.DisplayedDataFields);
        }

        private void UpdateSelectedFields(IList<string> selectedFields)
        {
            SelectedFields.Clear();
            for (int i = 0; i < selectedFields.Count; i++)
            {
                string fieldName = selectedFields[i];
                //Check if DisplayField is valid
                if (string.IsNullOrEmpty(fieldName) || !allFields.Any(x => fieldName == x.FullName) || !allFields.First(x => fieldName == x.FullName).Enabled)
                {
                    selectedFields[i] = EmptyDataField.FullName;
                }

                SelectedFields.Add(allFields.First(x => x.FullName == selectedFields[i]).FieldExtension);
            }
        }

        public void AddField()
        {
            if (SelectedFields.Count >= 20) return;
            SelectedFields.Add(allFields.First(x => x.FullName == EmptyDataField.FullName).FieldExtension);
            SelectedFieldsChanged?.Invoke(SelectedFields.Select(field => field.GetType().FullName).ToList());
        }

        public void RemoveField()
        {
            if (SelectedFields.Count <= 0) return;
            SelectedFields.RemoveAt(SelectedFields.Count - 1);
            SelectedFieldsChanged?.Invoke(SelectedFields.Select(field => field.GetType().FullName).ToList());
        }

        public void DayNightModeChanged(IDictionary<string, Settings.DataField> settings)
        {
            foreach (var field in allFields)
            {
                if (!(settings.TryGetValue(field.GetType().FullName, out var fieldSettings))) continue;
                UpdateColorOveride(fieldSettings);
            }
        }

        internal void UpdateProperties(Settings.DataField settings)
        {
            var field = allFields.First(x => x.FullName == settings.FullName);
            if (field == null) return;

            field.Enabled = settings.Enabled;
            if (SelectedFields.Any(x => x.GetType().FullName == field.FullName))
            {
                for (int i = 0; i < SelectedFields.Count; i++)
                {
                    if (!field.Enabled && SelectedFields[i].GetType().FullName == field.FullName)
                    {
                        SelectedFields[i] = allFields.First(x => x.FullName == EmptyDataField.FullName).FieldExtension;
                    }
                }
            }
        }

        internal void UpdateColorOveride(Settings.DataField settings)
        {
            var field = allFields.First(x => x.FullName == settings.FullName);
            if (field == null) return;

            if (!settings.Override.DayNightColorScheme.DayModeColor.Override)
            {
                settings.Override.DayNightColorScheme.NightModeColor.Override = false;
            }

            if (!settings.Override.DayNightColorScheme.DayModeColor.Override)
            {
                field.FieldExtension.Data.Color = settings.Override.DayNightColorScheme.DayModeColor.DefaultValue;
                return;
            }

            if (BrightnessConfiguration.Configuration.IsNightMode && settings.Override.DayNightColorScheme.NightModeColor.Override)
            {
                field.FieldExtension.Data.Color = settings.Override.DayNightColorScheme.NightModeColor.OverrideValue;
                return;
            }
            field.FieldExtension.Data.Color = settings.Override.DayNightColorScheme.DayModeColor.OverrideValue;
        }

        internal void UpdateDecimalOverride(Settings.DataField settings)
        {
            var field = allFields.First(x => x.FullName == settings.FullName);
            if (field == null) return;

            field.FieldExtension.Data.Decimal = settings.Override.Decimal.Override
                ? settings.Override.Decimal.OverrideValue
                : settings.Override.Decimal.DefaultValue;
        }

        internal void UpdateNameOverride(Settings.DataField settings)
        {
            var field = allFields.First(x => x.FullName == settings.FullName);
            if (field == null)
            {
                SimHub.Logging.Current.Error($"{settings.FullName} field not found in loaded fields.");
                return;
            }

            field.FieldExtension.Data.Name = settings.Override.Name.Override
                ? settings.Override.Name.OverrideValue
                : settings.Override.Name.DefaultValue;
        }

        public void NextSelectedField(int index)
        {
            if (index < 0 || index > SelectedFields.Count - 1) throw new ArgumentOutOfRangeException($"{nameof(index)}");

            string fieldName = SelectedFields[index].GetType().FullName;
            int allFieldIndex = allFields.IndexOf(allFields.First(x => x.FullName == fieldName));

            do
            {
                allFieldIndex++;
                if (allFieldIndex >= allFields.Count)
                {
                    allFieldIndex = 0;
                }
            } while (!allFields[allFieldIndex].Enabled);

            SelectedFields[index] = allFields[allFieldIndex].FieldExtension;

            SelectedFieldsChanged?.Invoke(SelectedFields.Select(field => field.GetType().FullName).ToList());
        }

        public void PrevSelectedField(int index)
        {
            if (index < 0 || index > SelectedFields.Count - 1) throw new ArgumentOutOfRangeException($"{nameof(index)}");

            string fieldName = SelectedFields[index].GetType().FullName;
            int allFieldIndex = allFields.IndexOf(allFields.First(x => x.FullName == fieldName));

            do
            {
                allFieldIndex--;
                if (allFieldIndex < 0)
                {
                    allFieldIndex = allFields.Count - 1;
                }
            } while (!allFields[allFieldIndex].Enabled);
            SelectedFields[index] = allFields[allFieldIndex].FieldExtension;

            SelectedFieldsChanged?.Invoke(SelectedFields.Select(field => field.GetType().FullName).ToList());
        }

        private IDataField GetField(int index)
        {
            if (index < 0 || index >= SelectedFields.Count) return EmptyDataField.Field.Data;
            return SelectedFields[index].Data;
        }
    }
}