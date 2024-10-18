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
    internal class GaugeFieldManager : FieldManagerBase
    {
        private const string FIELD_TYPE_NAME = "Gauge";
        internal GaugeFieldManager(PluginManager pluginManager, Type pluginType) : base(pluginManager, pluginType, FIELD_TYPE_NAME)
        {
            this.pluginManager.AddProperty(AmountOfFieldName, this.pluginType, SelectedFields.Count);
            SelectedFields.CollectionChanged += SelectedFields_CollectionChanged;

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}name",
                "Returns the name of the field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Name));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}value",
                "Returns the value of the field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Value));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}decimal",
                "Returns the number of decimals the value has of the field of the specified field.",
                "index",
                engine => (Func<int, int>)(index => GetField(index - 1).Decimal));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}unit",
                "Returns the unit of the  field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Unit));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}colorprimary",
                "Returns the primary color of the field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Color.Primary));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}coloraccent",
                "Returns the accent color of the field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Color.Accent));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}maximum",
                "Return the maximum value of the field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Maximum));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}minimum",
                "Return the minimum value of the field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Minimum));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}step",
                "Return the step value of the field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Step));
        }

        internal ObservableCollection<IGaugeFieldExtension> SelectedFields { get; private set; } = new ObservableCollection<IGaugeFieldExtension>();
        protected readonly ObservableCollection<FieldComponent<IGaugeFieldExtension, IGaugeField>> allFields = new ObservableCollection<FieldComponent<IGaugeFieldExtension, IGaugeField>>();
        internal IList<FieldComponent<IGaugeFieldExtension, IGaugeField>> AllFields { get => allFields; }

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

        protected IGaugeField GetField(int index)
        {
            if (index < 0 || index >= SelectedFields.Count) return EmptyField.Field.Data;
            return SelectedFields[index].Data;
        }

        public void UpdateSelectedFields(Settings.ICarFields carFields)
        {
            UpdateSelectedFields(carFields.DisplayedGaugeFields);
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
        public void AddExtensionField(Type type, IDictionary<string, Settings.GaugeField> settings)
        {
            IGaugeFieldExtension fieldInstance;
            try
            {
                fieldInstance = (IGaugeFieldExtension)Activator.CreateInstance(type, gameName);
            }
            catch (Exception e)
            {
                SimHub.Logging.Current.Error(type, e);
                return;
            }

            if (!fieldInstance.IsGameSupported) return;

            //Get field settings else create field settings
            if (!(settings.TryGetValue(type.FullName, out Settings.GaugeField fieldSetting)))
            {

                fieldSetting = new Settings.GaugeField
                {
                    Enabled = true,
                };
                fieldSetting.Override.Name = new PropertyOverride<string>(fieldInstance.Data.Name);
                fieldSetting.Override.Decimal = new PropertyOverride<int>(fieldInstance.Data.Decimal);
                fieldSetting.Override.DayNightColorScheme = new DayNightColorScheme(fieldInstance.Data.Color);
                settings.Add(type.FullName, fieldSetting);
            }

            //Make sure that empty field can't be disabled.
            if (fieldInstance.GetType().FullName == EmptyField.FullName) fieldSetting.Enabled = true;

            fieldSetting.Namespace = type.Namespace;
            fieldSetting.Name = type.Name;
            fieldSetting.FullName = type.FullName;
            fieldSetting.IsDecimal = fieldInstance.Data.IsDecimalNumber;
            fieldSetting.IsRangeLocked = fieldInstance.Data.IsRangeLocked;
            fieldSetting.IsStepLocked = fieldInstance.Data.IsStepLocked;

            //Get default values before they are overriden
            fieldSetting.Override.Name.DefaultValue = fieldInstance.Data.Name;
            fieldSetting.Override.Decimal.DefaultValue = fieldInstance.Data.Decimal;
            fieldSetting.Override.DayNightColorScheme.DayModeColor.DefaultValue = fieldInstance.Data.Color;
            fieldSetting.Override.DayNightColorScheme.NightModeColor.DefaultValue = fieldInstance.Data.Color;
            fieldSetting.Override.Maximum.DefaultValue = fieldInstance.Data.Maximum;
            fieldSetting.Override.Minimum.DefaultValue = fieldInstance.Data.Minimum;
            fieldSetting.Override.Step.DefaultValue = fieldInstance.Data.Step;

            fieldSetting.GameSupported = fieldInstance.IsGameSupported;
            fieldSetting.SupportedGames = fieldInstance.SupportedGames;
            fieldSetting.Description = fieldInstance.Description;

            AllFields.Add(new FieldComponent<IGaugeFieldExtension, IGaugeField>(fieldInstance) { Enabled = fieldSetting.Enabled });

            UpdateNameOverride(fieldSetting);
            UpdateColorOveride(fieldSetting);
            UpdateDecimalOverride(fieldSetting);
            UpdateMaximumOverride(fieldSetting);
            UpdateMinimumOverride(fieldSetting);
            UpdateStepOverride(fieldSetting);
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

        public void DayNightModeChanged(IDictionary<string, Settings.GaugeField> settings)
        {
            foreach (var field in AllFields)
            {
                if (!(settings.TryGetValue(field.GetType().FullName, out var fieldSettings))) continue;
                UpdateColorOveride(fieldSettings);
            }
        }

        internal void UpdateProperties(Settings.GaugeField settings)
        {
            var field = AllFields.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Field not found! {settings.FullName}");

            field.Enabled = settings.Enabled;
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

        internal void UpdateColorOveride(Settings.GaugeField settings)
        {
            var field = AllFields.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Field not found! {settings.FullName}");

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

        internal void UpdateDecimalOverride(Settings.GaugeField settings)
        {
            var field = AllFields.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Field not found! {settings.FullName}");

            field.FieldExtension.Data.Decimal = settings.Override.Decimal.Override
                ? settings.Override.Decimal.OverrideValue
                : settings.Override.Decimal.DefaultValue;
        }

        internal void UpdateNameOverride(Settings.GaugeField settings)
        {
            var field = AllFields.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Field not found! {settings.FullName}");

            field.FieldExtension.Data.Name = settings.Override.Name.Override
                ? settings.Override.Name.OverrideValue
                : settings.Override.Name.DefaultValue;
        }

        internal void UpdateMaximumOverride(Settings.GaugeField settings)
        {
            var field = AllFields.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Field not found! {settings.FullName}");

            field.FieldExtension.Data.Maximum = settings.Override.Maximum.Override
                ? settings.Override.Maximum.OverrideValue
                : settings.Override.Maximum.DefaultValue;
        }

        internal void UpdateMinimumOverride(Settings.GaugeField settings)
        {
            var field = AllFields.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Field not found! {settings.FullName}");

            field.FieldExtension.Data.Minimum = settings.Override.Minimum.Override
                ? settings.Override.Minimum.OverrideValue
                : settings.Override.Minimum.DefaultValue;
        }

        internal void UpdateStepOverride(Settings.GaugeField settings)
        {
            var field = AllFields.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Field not found! {settings.FullName}");

            field.FieldExtension.Data.Step = settings.Override.Step.Override
                ? settings.Override.Step.OverrideValue
                : settings.Override.Step.DefaultValue;
        }
        public void NextSelectedField(int index)
        {
            if (index < 0 || index > SelectedFields.Count - 1) throw new ArgumentOutOfRangeException($"{nameof(index)}");

            string fieldName = SelectedFields[index].GetType().FullName;
            int allFieldIndex = AllFields.IndexOf(AllFields.First(x => x.FullName == fieldName));

            do
            {
                allFieldIndex++;
                if (allFieldIndex >= AllFields.Count)
                {
                    allFieldIndex = 0;
                }
            } while (!AllFields[allFieldIndex].Enabled);

            SelectedFields[index] = AllFields[allFieldIndex].FieldExtension;

            SelectedFieldsChanged?.Invoke(SelectedFields.Select(field => field.GetType().FullName).ToList());
        }

        public void PrevSelectedField(int index)
        {
            if (index < 0 || index > SelectedFields.Count - 1) throw new ArgumentOutOfRangeException($"{nameof(index)}");

            string fieldName = SelectedFields[index].GetType().FullName;
            int allFieldIndex = AllFields.IndexOf(AllFields.First(x => x.FullName == fieldName));

            do
            {
                allFieldIndex--;
                if (allFieldIndex < 0)
                {
                    allFieldIndex = AllFields.Count - 1;
                }
            } while (!AllFields[allFieldIndex].Enabled);
            SelectedFields[index] = AllFields[allFieldIndex].FieldExtension;

            SelectedFieldsChanged?.Invoke(SelectedFields.Select(field => field.GetType().FullName).ToList());
        }
    }
}
