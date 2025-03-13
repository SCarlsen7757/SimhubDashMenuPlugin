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
    internal class GaugeFieldManager : FieldManagerBase, IFieldManager<Settings.GaugeField>
    {
        private const string FIELD_TYPE_NAME = "Gauge";
        internal GaugeFieldManager(PluginManager pluginManager, Type pluginType, IList<string> gaugeFieldOrder) : base(pluginManager, pluginType, gaugeFieldOrder, FIELD_TYPE_NAME)
        {
            this.pluginManager.AddProperty(AmountOfFieldName, this.pluginType, SelectedFields.Count);
            SelectedFields.CollectionChanged += SelectedFields_CollectionChanged;

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}name",
                "Gets the name of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Name));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}value",
                "Gets the value of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Value));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}decimal",
                "Gets the number of decimal places for the specified field's value.",
                "index",
                engine => (Func<int, int>)(index => GetField(index - 1).Decimal));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}unit",
                "Gets the unit of measurement for the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Unit));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}colorprimary",
                "Gets the primary color of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Color.Primary));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}coloraccent",
                "Gets the accent color of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Color.Accent));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}maximum",
                "Gets the maximum value of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Maximum));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}minimum",
                "Gets the minimum value of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Minimum));

            SimhubHelper.AddNCalcFunction($"dashfield{FIELD_TYPE_NAME.ToLower()}step",
                "Gets the step value of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetField(index - 1).Step));
        }

        internal ObservableCollection<IGaugeFieldExtension> SelectedFields { get; private set; } = new ObservableCollection<IGaugeFieldExtension>();
        protected readonly ObservableCollection<IFieldComponent<IGaugeFieldExtension, IGaugeField>> allFields = new ObservableCollection<IFieldComponent<IGaugeFieldExtension, IGaugeField>>();
        internal ObservableCollection<IFieldComponent<IGaugeFieldExtension, IGaugeField>> AllFields { get => allFields; }

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
        public void AddExtensionField(Type type, Settings.FieldSettings<Settings.GaugeField> settings)
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
            if (!(settings.Settings.TryGetValue(type.FullName, out Settings.GaugeField fieldSetting)))
            {

                fieldSetting = new Settings.GaugeField { Enabled = true };
                fieldSetting.Override.Name.OverrideValue = fieldInstance.Data.Name;
                fieldSetting.Override.Decimal.OverrideValue = fieldInstance.Data.Decimal;
                fieldSetting.Override.DayNightColorScheme.DayModeColor.OverrideValue = fieldInstance.Data.Color.Clone();
                fieldSetting.Override.DayNightColorScheme.NightModeColor.OverrideValue = fieldInstance.Data.Color.Clone();
                fieldSetting.Override.Maximum.OverrideValue = fieldInstance.Data.Maximum;
                fieldSetting.Override.Minimum.OverrideValue = fieldInstance.Data.Minimum;
                fieldSetting.Override.Step.OverrideValue = fieldInstance.Data.Step;
                settings.Settings.Add(type.FullName, fieldSetting);
            }

            if (!settings.Order.Contains(type.FullName) && !fieldSetting.Hide) settings.Order.Add(type.FullName);

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

            var field = new FieldComponent<IGaugeFieldExtension, IGaugeField>(fieldInstance) { Enabled = fieldSetting.Enabled };
            AllFields.Add(field);

            UpdateNameOverride(fieldSetting, field);
            UpdateColorOveride(fieldSetting, field);
            UpdateDecimalOverride(fieldSetting, field);
            UpdateMaximumOverride(fieldSetting, field);
            UpdateMinimumOverride(fieldSetting, field);
            UpdateStepOverride(fieldSetting, field);
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
                UpdateColorOveride(fieldSettings, field);
            }
        }

        internal void UpdateProperties(Settings.IGaugeField settings, PropertyChangedEventArgs e)
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
                case nameof(settings.Override.Maximum):
                    UpdateMaximumOverride(settings, field);
                    return;
                case nameof(settings.Override.Minimum):
                    UpdateMinimumOverride(settings, field);
                    return;
                case nameof(settings.Override.Step):
                    UpdateStepOverride(settings, field);
                    return;
                case nameof(settings.Enabled):
                    UpdateProperties(settings, field);
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

        private void UpdateProperties(Settings.IGaugeField settings, IFieldComponent<IGaugeFieldExtension, IGaugeField> field)
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

        private void UpdateColorOveride(Settings.IGaugeField settings, IFieldComponent<IGaugeFieldExtension, IGaugeField> field)
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

        private void UpdateDecimalOverride(Settings.IGaugeField settings, IFieldComponent<IGaugeFieldExtension, IGaugeField> field)
        {
            field.FieldExtension.Data.Decimal = settings.Override.Decimal.Override
                ? settings.Override.Decimal.OverrideValue
                : settings.Override.Decimal.DefaultValue;
        }

        private void UpdateNameOverride(Settings.IGaugeField settings, IFieldComponent<IGaugeFieldExtension, IGaugeField> field)
        {
            field.FieldExtension.Data.Name = settings.Override.Name.Override
                ? settings.Override.Name.OverrideValue
                : settings.Override.Name.DefaultValue;
        }

        private void UpdateMaximumOverride(Settings.IGaugeField settings, IFieldComponent<IGaugeFieldExtension, IGaugeField> field)
        {
            field.FieldExtension.Data.Maximum = settings.Override.Maximum.Override
                ? settings.Override.Maximum.OverrideValue
                : settings.Override.Maximum.DefaultValue;
        }

        private void UpdateMinimumOverride(Settings.IGaugeField settings, IFieldComponent<IGaugeFieldExtension, IGaugeField> field)
        {
            field.FieldExtension.Data.Minimum = settings.Override.Minimum.Override
                ? settings.Override.Minimum.OverrideValue
                : settings.Override.Minimum.DefaultValue;
        }

        private void UpdateStepOverride(Settings.IGaugeField settings, IFieldComponent<IGaugeFieldExtension, IGaugeField> field)
        {
            field.FieldExtension.Data.Step = settings.Override.Step.Override
                ? settings.Override.Step.OverrideValue
                : settings.Override.Step.DefaultValue;
        }
        public void NextSelectedField(int index)
        {
            if (index < 0 || index > SelectedFields.Count - 1) throw new ArgumentOutOfRangeException($"{nameof(index)}");

            string fieldName = SelectedFields[index].GetType().FullName;
            int fieldIndex = fieldOrder.IndexOf(fieldName);
            IFieldComponent<IGaugeFieldExtension, IGaugeField> field;

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
            IFieldComponent<IGaugeFieldExtension, IGaugeField> field;

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
