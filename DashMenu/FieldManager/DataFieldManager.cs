using DashMenu.Data;
using DashMenu.Extensions;
using SimHub.Plugins;
using SimHub.Plugins.BrightnessControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DashMenu.FieldManager
{
    internal class DataFieldManager : FieldManagerBase, IFieldManager<Settings.DataField>
    {
        private const string FIELD_TYPE_NAME = "Data";
        internal DataFieldManager(PluginManager pluginManager, Type pluginType, IList<string> dataFieldOrder) : base(pluginManager, pluginType, dataFieldOrder, FIELD_TYPE_NAME)
        {
            this.pluginManager.AddProperty(AmountOfFieldName, this.pluginType, SelectedFields.Count);
            SelectedFields.CollectionChanged += SelectedFields_CollectionChanged;

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}name",
                "Gets the name of the field at the specified index.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Name));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}value",
                "Gets the value of the field at the specified index.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Value));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}decimal",
                "Gets the number of decimal places for the field at the specified index.",
                "index",
                engine => (Func<int, int>)(index => GetField(index - 1).Decimal));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}unit",
                "Gets the unit of measurement for the field at the specified index.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Unit));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}colorprimary",
                "Gets the primary color of the field at the specified index.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Color.Primary));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}coloraccent",
                "Gets the accent color of the field at the specified index.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Color.Accent));


        }
        internal ObservableCollection<IDataFieldExtension> SelectedFields { get; private set; } = new ObservableCollection<IDataFieldExtension>();
        protected readonly ObservableCollection<IFieldComponent<IDataFieldExtension, IDataField>> allFields = new ObservableCollection<IFieldComponent<IDataFieldExtension, IDataField>>();
        internal ObservableCollection<IFieldComponent<IDataFieldExtension, IDataField>> AllFields { get => allFields; }

        internal event SelectedFieldsChangedEventHandler SelectedFieldsChanged;

        private void SelectedFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    pluginManager.SetPropertyValue(AmountOfFieldName, pluginType, SelectedFields.Count);
                    break;
                default:
                    break;
            }
        }
        protected IDataField GetField(int index)
        {
            if (index < 0 || index >= SelectedFields.Count) return EmptyField.Field.Data;
            return SelectedFields[index].Data;
        }

        public void UpdateSelectedFields(Settings.ICarFields carFields)
        {
            UpdateSelectedFields(carFields.DisplayedDataFields);
        }

        protected void UpdateSelectedFields(IList<string> selectedFields)
        {
            SelectedFields.Clear();
            for (int i = 0; i < selectedFields.Count; i++)
            {
                string fieldName = selectedFields[i];
                //Check if DisplayField is valid
                if (string.IsNullOrEmpty(fieldName) || !AllFields.Any(x => fieldName == x.FullName) || !AllFields.First(x => fieldName == x.FullName).Enabled)
                {
                    selectedFields[i] = EmptyField.FullName;
                }

                SelectedFields.Add(AllFields.First(x => x.FullName == selectedFields[i]).FieldExtension);
            }
        }

        public void AddExtensionField(Type type, Settings.FieldSettings<Settings.DataField> fieldSettings)
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
            if (!(fieldSettings.Settings.TryGetValue(type.FullName, out Settings.DataField fieldSetting)))
            {

                fieldSetting = new Settings.DataField { Enabled = true, };
                fieldSetting.Override.Name.OverrideValue = fieldInstance.Data.Name;
                fieldSetting.Override.Decimal.OverrideValue = fieldInstance.Data.Decimal;
                fieldSetting.Override.DayNightColorScheme.DayModeColor.OverrideValue = fieldInstance.Data.Color.Clone();
                fieldSetting.Override.DayNightColorScheme.NightModeColor.OverrideValue = fieldInstance.Data.Color.Clone();
                fieldSettings.Settings.Add(type.FullName, fieldSetting);
            }

            if (!fieldSettings.Order.Contains(type.FullName) && !fieldSetting.Hide) fieldSettings.Order.Add(type.FullName);

            //Make sure that empty field can't be disabled.
            if (fieldInstance.GetType().FullName == EmptyField.FullName) fieldSetting.Enabled = true;

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

            var field = new FieldComponent<IDataFieldExtension, IDataField>(fieldInstance) { Enabled = fieldSetting.Enabled };
            AllFields.Add(field);

            UpdateNameOverride(fieldSetting, field);
            UpdateColorOveride(fieldSetting, field);
            UpdateDecimalOverride(fieldSetting, field);
        }

        public void AddField()
        {
            if (SelectedFields.Count >= 20) return;
            SelectedFields.Add(AllFields.First(x => x.FullName == EmptyField.FullName).FieldExtension);
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
            foreach (var field in AllFields)
            {
                if (!(settings.TryGetValue(field.GetType().FullName, out var fieldSettings))) continue;
                UpdateColorOveride(fieldSettings, field);
            }
        }

        internal void UpdateProperties(Settings.IDataField settings, PropertyChangedEventArgs e)
        {
            var field = AllFields.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Field not found! {settings.FullName}");

            switch (e.PropertyName)
            {
                case nameof(settings.Override.Name):
                    UpdateNameOverride(settings, field);
                    return;
                case nameof(settings.Override.Decimal):
                    UpdateDecimalOverride(settings, field);
                    return;
                case nameof(settings.Override.DayNightColorScheme):
                    UpdateColorOveride(settings, field);
                    return;
                case nameof(settings.Enabled):
                    UpdateEnabledProperties(settings, field);
                    return;
                case nameof(settings.Hide):
                    UpdateOrder(settings, fieldOrder);
                    return;
                default:
                    return;
            }
        }

        private void UpdateOrder(Settings.IDataField dataField, in IList<string> order)
        {
            if (dataField.Hide)
            {
                order.Remove(dataField.FullName);
            }
            else
            {
                order.Add(dataField.FullName);
            }

        }

        private void UpdateEnabledProperties(Settings.IDataField settings, IFieldComponent<IDataFieldExtension, IDataField> field)
        {
            if (!settings.Enabled && field.Enabled)
            {
                if (SelectedFields.Any(x => x.GetType().FullName == field.FullName))
                {
                    for (int i = 0; i < SelectedFields.Count; i++)
                    {
                        if (!field.Enabled && SelectedFields[i].GetType().FullName == field.FullName)
                        {
                            SelectedFields[i] = AllFields.First(x => x.FullName == EmptyField.FullName).FieldExtension;
                        }
                    }
                }
            }
            field.Enabled = settings.Enabled;
        }

        private void UpdateColorOveride(Settings.IDataField settings, IFieldComponent<IDataFieldExtension, IDataField> field)
        {
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

        private void UpdateDecimalOverride(Settings.IDataField settings, IFieldComponent<IDataFieldExtension, IDataField> field)
        {
            field.FieldExtension.Data.Decimal = settings.Override.Decimal.Override
                ? settings.Override.Decimal.OverrideValue
                : settings.Override.Decimal.DefaultValue;
        }

        private void UpdateNameOverride(Settings.IDataField settings, IFieldComponent<IDataFieldExtension, IDataField> field)
        {
            field.FieldExtension.Data.Name = settings.Override.Name.Override
                ? settings.Override.Name.OverrideValue
                : settings.Override.Name.DefaultValue;
        }

        public void NextSelectedField(int index)
        {
            if (index < 0 || index > SelectedFields.Count - 1) throw new ArgumentOutOfRangeException($"{nameof(index)}");

            string fieldName = SelectedFields[index].GetType().FullName;
            int fieldIndex = fieldOrder.IndexOf(fieldName);
            IFieldComponent<IDataFieldExtension, IDataField> field;

            do
            {
                fieldIndex++;
                if (fieldIndex >= fieldOrder.Count)
                {
                    fieldIndex = 0;
                }
                field = AllFields.First(x => x.FullName == fieldOrder[fieldIndex]);
            } while (!field.Enabled);

            SelectedFields[index] = field.FieldExtension;

            SelectedFieldsChanged?.Invoke(SelectedFields.Select(x => x.GetType().FullName).ToList());
        }

        public void PrevSelectedField(int index)
        {
            if (index < 0 || index > SelectedFields.Count - 1) throw new ArgumentOutOfRangeException($"{nameof(index)}");

            string fieldName = SelectedFields[index].GetType().FullName;
            int fieldIndex = fieldOrder.IndexOf(fieldName);
            IFieldComponent<IDataFieldExtension, IDataField> field;

            do
            {
                fieldIndex--;
                if (fieldIndex < 0)
                {
                    fieldIndex = fieldOrder.Count - 1;
                }
                field = AllFields.First(x => x.FullName == fieldOrder[fieldIndex]);
            } while (!field.Enabled);
            SelectedFields[index] = field.FieldExtension;

            SelectedFieldsChanged?.Invoke(SelectedFields.Select(x => x.GetType().FullName).ToList());
        }
    }
}