using DashMenu.Data;
using DashMenu.Settings;
using GameReaderCommon;
using SimHub.Plugins;
using SimHub.Plugins.BrightnessControl;
using SimHub.Plugins.OutputPlugins.Dash.TemplatingCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;


namespace DashMenu
{
    [PluginDescription("Plugin to manage dash menus.")]
    [PluginAuthor("Mark Carlsen")]
    [PluginName("Dash menu")]
    public class DashMenuPlugin : IPlugin, IDataPlugin, IWPFSettingsV2, ISettingPlugin
    {
        public DashMenuPlugin() { }

        private Settings.Settings Settings { get; set; }
        private MenuConfiguration MenuConfiguration { get; set; } = new MenuConfiguration();
        private List<IDataFieldComponent> dataFields = new List<IDataFieldComponent>();
        private List<IGaugeFieldComponent> gaugeFields = new List<IGaugeFieldComponent>();
        /// <summary>
        /// List of all data fields.
        /// </summary>
        private readonly ObservableCollection<DataFieldComponent> allDataField = new ObservableCollection<DataFieldComponent>();
        /// <summary>
        /// List of all gauge fields.
        /// </summary>
        private readonly ObservableCollection<GaugeFieldComponent> allGaugeField = new ObservableCollection<GaugeFieldComponent>();
        /// <summary>
        /// List of available data fields that can be used in the displayed data fields.
        /// </summary>
        private readonly List<IDataFieldComponent> availableDataField = new List<IDataFieldComponent>();
        /// <summary>
        /// List of avaiable gauge fields that can be used in the displayed gauge fields.
        /// </summary>
        private readonly List<IGaugeFieldComponent> availableGaugeField = new List<IGaugeFieldComponent>();
        private string oldCarId = null;
        private string oldCarModel = null;
        public string PluginName
        {
            get
            {
                PluginNameAttribute attribute = (PluginNameAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PluginNameAttribute));
                return attribute.name;
            }
        }
        /// <summary>
        /// Instance of the current plugin manager.
        /// </summary>
        public PluginManager PluginManager { get; set; }
        /// <summary>
        /// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);
        /// <summary>
        /// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
        /// </summary>
        public string LeftMenuTitle => null;
        /// <summary>
        /// Returns the settings control, return null if no settings control is required.
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager) => new UI.SettingsControl(Settings.GameSettings[PluginManager.GameName]);

        internal static class PropertyNames
        {
            public const string PluginRunning = "PluginRunning";
            public const string ConfigMode = "ConfigMode";
            public const string ActiveConfigField = "ActiveConfigField";
            public const string FieldType = "FieldType";
            public const string AmountOfDataFields = "AmountOfDataFields";
            public const string AmountOfGaugeFields = "AmountOfGaugeFields";
        }

        internal static class ActionNames
        {
            public const string ToggleConfigMode = "ToggleConfigMode";
            public const string ChangeFieldType = "ChangeFieldType";
            public const string ConfigNextField = "ConfigNextField";
            public const string ConfigPrevField = "ConfigPrevField";
            public const string ChangeFieldTypeNext = "ChangeFieldTypeNext";
            public const string ChangeFieldTypePrev = "ChangeFieldTypePrev";
            public const string IncreaseNumberOfField = "IncreaseNumberOfFields";
            public const string DecreaseNumberOfField = "DecreasenumberOfFields";
        }
        /// <summary>
        /// Called once after plugins startup.
        /// Plugins are rebuilt at game change.
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info($"{GetType().FullName}. Plugin: {PluginName}, Version {Version.PluginVersion}");
            LoadSettings();

            //Check if Empty field is in settings else add it
            AddDataFieldExtensionComponent(typeof(EmptyDataField));
            AddGaugeFieldExtensionComponent(typeof(EmptyGaugeField));
            GetCustomFields();
            SettingsExtensionFieldsCleanUp();
            UpdateAvailableFields();

            pluginManager.AddProperty(PropertyNames.ConfigMode, GetType(), MenuConfiguration.ConfigurationMode, "When in configuration mode, it's possible to change the displayed data.");
            pluginManager.AddProperty(PropertyNames.ActiveConfigField, GetType(), MenuConfiguration.ActiveField, "Active field in the dash menu config screen.");
            pluginManager.AddProperty(PropertyNames.FieldType, GetType(), MenuConfiguration.FieldType.ToString(), "Field type that selected, that can be changed when in configuration mode.");
            pluginManager.AddProperty(PropertyNames.AmountOfDataFields, GetType(), dataFields.Count, "Amount of data fields for the current car.");
            pluginManager.AddProperty(PropertyNames.AmountOfGaugeFields, GetType(), gaugeFields.Count, "Amount of gauge fields for the current car.");
            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ToggleConfigMode, (pm, a) =>
            {
                if (string.IsNullOrWhiteSpace(PluginManager.LastCarId)) return;
                MenuConfiguration.ConfigurationMode = !MenuConfiguration.ConfigurationMode;

                //When entering configuration mode, reset the active field to 1.
                if (MenuConfiguration.ConfigurationMode)
                {
                    MenuConfiguration.ActiveField = 0;
                    MenuConfiguration.FieldType = FieldType.Data;
                }
                else
                {
                    //Save displaye field when exiting config mode.
                    SaveDisplayedField(PluginManager.LastCarId, PluginManager.GameManager.CarManager.LastCarSettings.CarModel);
                }

                pluginManager.SetPropertyValue(PropertyNames.ConfigMode, GetType(), MenuConfiguration.ConfigurationMode);
                pluginManager.SetPropertyValue(PropertyNames.ActiveConfigField, GetType(), MenuConfiguration.ActiveField + 1);
                pluginManager.SetPropertyValue(PropertyNames.FieldType, GetType(), MenuConfiguration.FieldType.ToString());
            });
            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ChangeFieldType, (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                //It works for now.
                if (MenuConfiguration.FieldType == FieldType.Data)
                {
                    MenuConfiguration.FieldType = FieldType.Gauge;
                }
                else
                {
                    MenuConfiguration.FieldType = FieldType.Data;
                }

                MenuConfiguration.ActiveField = 0;
                pluginManager.SetPropertyValue(PropertyNames.FieldType, GetType(), MenuConfiguration.FieldType.ToString());
                pluginManager.SetPropertyValue(PropertyNames.ActiveConfigField, GetType(), MenuConfiguration.ActiveField + 1);
            });

            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ConfigNextField, (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                int count = CurrentFieldTypeCount() - 1;
                if (count <= 0) return;
                if (MenuConfiguration.ActiveField < count)
                {
                    MenuConfiguration.ActiveField++;
                }
                else
                {
                    MenuConfiguration.ActiveField = 0;
                }
                pluginManager.SetPropertyValue(PropertyNames.ActiveConfigField, GetType(), MenuConfiguration.ActiveField + 1);
                SimHub.Logging.Current.Debug($"Dash menu action ConfigNextField. New active field: {MenuConfiguration.ActiveField + 1}.");
            });

            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ConfigPrevField, (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                int count = CurrentFieldTypeCount() - 1;
                if (count <= 0) return;
                if (MenuConfiguration.ActiveField > 0)
                {
                    MenuConfiguration.ActiveField--;
                }
                else
                {
                    MenuConfiguration.ActiveField = count;
                }
                pluginManager.SetPropertyValue(PropertyNames.ActiveConfigField, GetType(), MenuConfiguration.ActiveField + 1);
                SimHub.Logging.Current.Debug($"Dash menu action ConfigPrevField. New active field {MenuConfiguration.ActiveField + 1}.");
            });

            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ChangeFieldTypeNext, (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                switch (MenuConfiguration.FieldType)
                {
                    case FieldType.Data:
                        var currentDataField = dataFields[MenuConfiguration.ActiveField];
                        dataFields[MenuConfiguration.ActiveField] = NextField(currentDataField);
                        break;
                    case FieldType.Gauge:
                        var currentGuageField = gaugeFields[MenuConfiguration.ActiveField];
                        gaugeFields[MenuConfiguration.ActiveField] = NextField(currentGuageField);
                        break;
                    default:
#if DEBUG
                        throw new ArgumentOutOfRangeException();
#else
                        SimHub.Logging.Current.Error($"Invalid FieldType: {MenuConfiguration.FieldType}");
                        break;
#endif
                }
                SimHub.Logging.Current.Debug($"Dash menu action ChangeFieldTypeNext of field type: {MenuConfiguration.FieldType}.");
            });

            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ChangeFieldTypePrev, (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                switch (MenuConfiguration.FieldType)
                {
                    case FieldType.Data:
                        var currentDataField = dataFields[MenuConfiguration.ActiveField];
                        dataFields[MenuConfiguration.ActiveField] = PrevField(currentDataField);
                        break;
                    case FieldType.Gauge:
                        var currentGuageField = gaugeFields[MenuConfiguration.ActiveField];
                        gaugeFields[MenuConfiguration.ActiveField] = PrevFied(currentGuageField);
                        break;
                    default:
#if DEBUG
                        throw new ArgumentOutOfRangeException();
#else
                        SimHub.Logging.Current.Error($"Invalid FieldType: {MenuConfiguration.FieldType}");
                        break;
#endif
                }
                SimHub.Logging.Current.Debug($"Dash menu action ChangeFieldTypePrev of field type: {MenuConfiguration.FieldType}.");
            });

            pluginManager.AddAction<DashMenuPlugin>(ActionNames.IncreaseNumberOfField, (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                int count = CurrentFieldTypeCount();
                if (count <= 0 || count >= 20) return;
                switch (MenuConfiguration.FieldType)
                {
                    case FieldType.Data:
                        dataFields.Add(EmptyDataField.Field);
                        pluginManager.SetPropertyValue(PropertyNames.AmountOfDataFields, GetType(), dataFields.Count);
                        break;
                    case FieldType.Gauge:
                        gaugeFields.Add(EmptyGaugeField.Field);
                        pluginManager.SetPropertyValue(PropertyNames.AmountOfGaugeFields, GetType(), gaugeFields.Count);
                        break;
                    default:
#if DEBUG
                        throw new ArgumentOutOfRangeException();
#else
                        SimHub.Logging.Current.Error($"Invalid FieldType: {MenuConfiguration.FieldType}");
                        break;
#endif
                }
            });

            pluginManager.AddAction<DashMenuPlugin>(ActionNames.DecreaseNumberOfField, (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                switch (MenuConfiguration.FieldType)
                {
                    case FieldType.Data:
                        if (dataFields.Count <= 0) return;
                        dataFields.RemoveAt(dataFields.Count - 1);
                        pluginManager.SetPropertyValue(PropertyNames.AmountOfDataFields, GetType(), dataFields.Count);
                        break;
                    case FieldType.Gauge:
                        if (gaugeFields.Count <= 0) return;
                        gaugeFields.RemoveAt(gaugeFields.Count - 1);
                        pluginManager.SetPropertyValue(PropertyNames.AmountOfGaugeFields, GetType(), gaugeFields.Count);
                        break;
                    default:
#if DEBUG
                        throw new ArgumentOutOfRangeException();
#else
                        SimHub.Logging.Current.Error($"Invalid FieldType: {MenuConfiguration.FieldType}");
                        break;
#endif
                }
            });

            //Add NCalc method
            //NCalc Methods are normaly added when Simhub starts. So when this plugin is initialize after the first time, due to settings change or other game selected.
            //It will add the method again, but it's already added.
            AddNCalcFunction("dashfielddataname",
                "Returns the name of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetDataFieldName(index)));

            AddNCalcFunction("dashfielddatavalue",
                "Returns the value of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetDataFieldValue(index)));

            AddNCalcFunction("dashfielddatadecimal",
                "Returns the number of decimals the value has of the data field of the specified field.",
                "index",
                engine => (Func<int, int>)(index => GetDataFieldDecimal(index)));

            AddNCalcFunction("dashfielddataunit",
                "Returns the unit of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetDataFieldUnit(index)));

            AddNCalcFunction("dashfielddatacolorprimary",
                "Returns the primary color of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetDataFieldColorPrimary(index)));

            AddNCalcFunction("dashfielddatacoloraccent",
                "Returns the accent color of the data field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetDataFieldColorAccent(index)));

            AddNCalcFunction("dashfieldgaugename",
                "Return the name of the gauge field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetGaugeFieldName(index)));

            AddNCalcFunction("dashfieldgaugevalue",
                "Return the value of the gauge field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetGaugeFieldValue(index)));

            AddNCalcFunction("dashfieldgaugedecimal",
                "Returns the number of decimals the value has of the gauge field of the specified field.",
                "index",
                engine => (Func<int, int>)(index => GetGaugeFieldDecimal(index)));

            AddNCalcFunction("dashfieldgaugeunit",
                "Return the unit of the gauge field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetGaugeFieldUnit(index)));

            AddNCalcFunction("dashfieldgaugemaximum",
                "Return the maximum value of the gauge field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetGaugeFieldMaximum(index)));

            AddNCalcFunction("dashfieldgaugeminimum",
                "Return the minimum value of the gauge field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetGaugeFieldMinimum(index)));

            AddNCalcFunction("dashfieldgaugestep",
                "Return the step value of the gauge field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetGaugeFieldStep(index)));

            AddNCalcFunction("dashfieldgaugecolorprimary",
                "Return the primary color of the gauge field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetGaugeFieldColorPrimary(index)));

            AddNCalcFunction("dashfieldgaugecoloraccent",
                "Return the accent color of the gauge field of the specified field.",
                "index",
                engine => (Func<int, string>)(index => GetGaugeFieldColorAccent(index)));

            pluginManager.AddProperty<bool>(PropertyNames.PluginRunning, GetType(), true);
            PluginManagerEvents.Instance.ActiveCarChanged += CarChanged;
            BrightnessConfiguration.Configuration.PropertyChanged += DayNightMode_PropertyChanged;

            SimHub.Logging.Current.Info("Plugin started");
        }

        /// <summary>
        /// Called one time per game data update, contains all normalized game data,
        /// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT).
        /// 
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error.
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including current and previous data frame.</param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            foreach (IDataFieldComponent field in dataFields)
            {
                if (!field.IsGameSupported) continue;
                field.Update(ref data);
            }
            foreach (IGaugeFieldComponent field in gaugeFields)
            {
                if (!field.IsGameSupported) continue;
                field.Update(ref data);
            }
        }
        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here!
        /// Plugins are rebuilt at game change.
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            PluginManagerEvents.Instance.ActiveCarChanged -= CarChanged;
            BrightnessConfiguration.Configuration.PropertyChanged -= DayNightMode_PropertyChanged;

        }
        private void LoadSettings()
        {
            //Get field settings else create field settings
            Settings = this.ReadCommonSettings("DashMenuSettings", () => new Settings.Settings());

            if (!Settings.GameSettings.ContainsKey(PluginManager.GameName))
            {
                Settings.GameSettings.Add(PluginManager.GameName, new Settings.GameSettings());
            }
        }
        public void SaveSettings()
        {
            SaveDisplayedField(PluginManager.LastCarId, oldCarModel);
            this.SaveCommonSettings("DashMenuSettings", Settings);
        }
        private int CurrentFieldTypeCount()
        {
            switch (MenuConfiguration.FieldType)
            {
                case FieldType.Data:
                    return dataFields.Count;
                case FieldType.Gauge:
                    return gaugeFields.Count;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void SettingsExtensionFieldsCleanUp()
        {
            foreach (var gameSettings in Settings.GameSettings.Values)
            {
                // Create a list to hold the keys to remove
                var fieldsToRemove = new List<string>();

                foreach (var dataField in gameSettings.DataFields.Keys)
                {
                    if (!allDataField.Any(x => x.FullName == dataField))
                    {
                        fieldsToRemove.Add(dataField);
                    }
                }
                // Remove the fields from gameSettings.DataFields that are not present in allDataField
                foreach (var field in fieldsToRemove)
                {
                    gameSettings.DataFields.Remove(field);
                }
            }
        }
        private bool AddNCalcFunction(string name, string description, string syntax, Func<NCalcEngineBase, Delegate> func)
        {
            if (NCalcEngineMethodsRegistry.GenericMethodsProvider.ContainsKey(name.ToLower())) { return false; }

            NCalcEngineMethodsRegistry.AddMethod(name,
                syntax,
                description,
                func);
            NCalcEngineBase.AvailableFunctions.Add(new SimHub.Plugins.OutputPlugins.Dash.WPFUI.FormulaPropertyEntry(name, $"{name}({syntax})", description));
            return true;
        }

        private IDataField GetDataField(int index)
        {
            if (index <= 0 || index > dataFields.Count) return EmptyDataField.Field.Data;
            return dataFields[index - 1].Data;
        }

        private string GetDataFieldName(int index)
        {
            return GetDataField(index).Name;
        }

        private string GetDataFieldValue(int index)
        {
            return GetDataField(index).Value;
        }

        private int GetDataFieldDecimal(int index)
        {
            return GetDataField(index).Decimal;
        }

        private string GetDataFieldUnit(int index)
        {
            return GetDataField(index).Unit;
        }

        private string GetDataFieldColorPrimary(int index)
        {
            return GetDataField(index).Color.Primary;
        }

        private string GetDataFieldColorAccent(int index)
        {
            return GetDataField(index).Color.Accent;
        }

        private IGaugeField GetGaugeField(int index)
        {
            if (index <= 0 || index > gaugeFields.Count) return EmptyGaugeField.Field.Data;
            return gaugeFields[index - 1].Data;
        }

        private string GetGaugeFieldName(int index)
        {
            return GetGaugeField(index).Name;
        }

        private string GetGaugeFieldValue(int index)
        {
            return GetGaugeField(index).Value;
        }

        private int GetGaugeFieldDecimal(int index)
        {
            return GetGaugeField(index).Decimal;
        }

        private string GetGaugeFieldUnit(int index)
        {
            return GetGaugeField(index).Unit;
        }

        private string GetGaugeFieldMaximum(int index)
        {
            return GetGaugeField(index).Maximum;
        }

        private string GetGaugeFieldMinimum(int index)
        {
            return GetGaugeField(index).Minimum;
        }

        private string GetGaugeFieldStep(int index)
        {
            return GetGaugeField(index).Step;
        }

        private string GetGaugeFieldColorPrimary(int index)
        {
            return GetGaugeField(index).Color.Primary;
        }

        private string GetGaugeFieldColorAccent(int index)
        {
            return GetGaugeField(index).Color.Accent;
        }

        private void CarChanged(object sender, EventArgs e)
        {
            //Save field for previues car before loading new car field data settings
            if (!string.IsNullOrWhiteSpace(oldCarId) || !string.IsNullOrWhiteSpace(oldCarModel))
            {
                SaveDisplayedField(oldCarId, oldCarModel);
                Settings.GameSettings[PluginManager.GameName].CarFields[oldCarId].IsActive = false;
            }

            //Create arrays
            dataFields = new List<IDataFieldComponent>();
            var fieldDataSettings = Settings.GameSettings[PluginManager.GameName].GetDisplayedDataField(PluginManager.LastCarId);
            //Assign from settings
            for (int i = 0; i < fieldDataSettings.Count; i++)
            {
                //Check if DisplayField is valid
                if (string.IsNullOrEmpty(fieldDataSettings[i]) || !availableDataField.Any(f => fieldDataSettings[i] == f.GetType().FullName))
                {
                    fieldDataSettings[i] = EmptyDataField.FullName;
                }

                //Asign data
                dataFields.Add(availableDataField.FirstOrDefault(f => f.GetType().FullName == fieldDataSettings[i]));
            }

            gaugeFields = new List<IGaugeFieldComponent>();
            var fieldGaugeSettings = Settings.GameSettings[PluginManager.GameName].GetDisplayedGaugeField(PluginManager.LastCarId);
            //Assign from settings
            for (int i = 0; i < fieldGaugeSettings.Count; i++)
            {
                //Check if DisplayField is valid
                if (string.IsNullOrEmpty(fieldGaugeSettings[i]) || !availableGaugeField.Any(f => fieldGaugeSettings[i] == f.GetType().FullName))
                {
                    fieldGaugeSettings[i] = EmptyDataField.FullName;
                }

                //Asign data
                gaugeFields.Add(availableGaugeField.FirstOrDefault(f => f.GetType().FullName == fieldGaugeSettings[i]));
            }

            //Save the new cars fields
            SaveDisplayedField(PluginManager.LastCarId, PluginManager.GameManager.CarManager.LastCarSettings.CarModel);
            Settings.GameSettings[PluginManager.GameName].CarFields[PluginManager.LastCarId].IsActive = true;

            PluginManager.SetPropertyValue(PropertyNames.AmountOfDataFields, GetType(), dataFields.Count);
            PluginManager.SetPropertyValue(PropertyNames.AmountOfGaugeFields, GetType(), gaugeFields.Count);

            oldCarId = PluginManager.LastCarId;
            oldCarModel = PluginManager.GameManager.CarManager.LastCarSettings.CarModel;
        }
        private void FieldComponent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateAvailableFields((DataFieldComponent)sender);
        }
        private void SaveDisplayedField(string carId, string carModel)
        {
            if (string.IsNullOrWhiteSpace(carId) || string.IsNullOrWhiteSpace(carModel)) return;

            var fieldDataSettings = new List<string>();
            for (int i = 0; i < dataFields.Count; i++)
            {
                fieldDataSettings.Add(dataFields[i].GetType().FullName);
            }

            var fieldGaugeSettings = new List<string>();
            for (int i = 0; i < gaugeFields.Count; i++)
            {
                fieldGaugeSettings.Add(gaugeFields[i].GetType().FullName);
            }
            Settings.GameSettings[PluginManager.GameName].UpdateDisplayedFields(carId, carModel, fieldDataSettings, fieldGaugeSettings);
        }
        private static IEnumerable<Type> GetExtensionFieldsType(string sub_dir)
        {
            string rootDirectory = Path.Combine(Path.GetDirectoryName((Assembly.GetExecutingAssembly().Location)), sub_dir);
            string[] dllFiles;
            try
            {
                dllFiles = Directory.GetFiles(rootDirectory, "*.dll");

            }
            catch (DirectoryNotFoundException)
            {
                SimHub.Logging.Current.Info($"Creating custom field dll folder. {rootDirectory}");
                Directory.CreateDirectory(rootDirectory);
                yield break;
            }
            Type interfaceType = typeof(IFieldExtensionBasic);

            // Iterate through each DLL file
            foreach (string dllFile in dllFiles)
            {
                Assembly assembly;
                try
                {
                    // Load the assembly
                    assembly = Assembly.LoadFrom(dllFile);
                }
                catch (Exception ex)
                {
                    SimHub.Logging.Current.Error($"Error loading DLL {dllFile}: {ex.Message}. No field data class will be loaded from this dll.");
                    continue;
                }

                // Iterate through all types in the assembly
                foreach (Type type in assembly.GetTypes())
                {
                    // Check if the type implements the interface
                    if (type.GetInterfaces().Contains(interfaceType))
                    {
                        yield return type;
                    }
                }
            }
        }
        private void GetCustomFields()
        {
            foreach (Type type in GetExtensionFieldsType("DashMenuExtensionFields"))
            {
                AddExtensionComponent(type);
            }
        }

        private void AddExtensionComponent(Type type)
        {
            if (type.GetInterfaces().Contains(typeof(IDataFieldComponent)))
            {
                AddDataFieldExtensionComponent(type);
            }
            if (type.GetInterfaces().Contains(typeof(IGaugeFieldComponent)))
            {
                AddGaugeFieldExtensionComponent(type);
            }
        }

        private void AddDataFieldExtensionComponent(Type type)
        {
            IDataFieldComponent fieldInstance;
            try
            {
                fieldInstance = (IDataFieldComponent)Activator.CreateInstance(type, PluginManager.GameName);
            }
            catch (Exception e)
            {
                SimHub.Logging.Current.Error(type, e);
                return;
            }
            if (!fieldInstance.IsGameSupported) return;

            //Get field settings else create field settings
            if (!(Settings.GameSettings[PluginManager.GameName].DataFields.TryGetValue(type.FullName, out Settings.DataField fieldSetting)))
            {

                fieldSetting = new Settings.DataField
                {
                    Enabled = true
                };
                fieldSetting.Override.Name = new PropertyOverride<string>(fieldInstance.Data.Name);
                fieldSetting.Override.Decimal = new PropertyOverride<int>(fieldInstance.Data.Decimal);
                fieldSetting.Override.DayNightColorScheme = new DayNightColorScheme(fieldInstance.Data.Color);
                Settings.GameSettings[PluginManager.GameName].DataFields.Add(type.FullName, fieldSetting);
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

            fieldSetting.PropertyChanged += DataFieldSetting_PropertyChanged;
            fieldSetting.Override.NamePropertyChanged += DataFieldNameOverride_PropertyChanged;
            fieldSetting.Override.DecimalPropertyChanged += DataFieldDecimalOverride_PropertyChanged;
            fieldSetting.Override.ColorSchemePropertyChanged += DataFieldColorSchemeOverridePropertyChanged;
            DataFieldComponent fieldComponent = new DataFieldComponent(fieldInstance)
            {
                Enabled = fieldSetting.Enabled
            };
            allDataField.Add(fieldComponent);
            UpdateNameOverride(fieldSetting);
            UpdateColorOveride(fieldSetting);
            UpdateDecimalOverride(fieldSetting);
        }
        private void AddGaugeFieldExtensionComponent(Type type)
        {
            IGaugeFieldComponent fieldInstance;
            try
            {
                fieldInstance = (IGaugeFieldComponent)Activator.CreateInstance(type, PluginManager.GameName);
            }
            catch (Exception e)
            {
                SimHub.Logging.Current.Error(type, e);
                return;
            }
            if (!fieldInstance.IsGameSupported) return;

            //Get field settings else create field settings
            if (!(Settings.GameSettings[PluginManager.GameName].GaugeFields.TryGetValue(type.FullName, out Settings.GaugeField fieldSetting)))
            {
                fieldSetting = new Settings.GaugeField
                {
                    Enabled = true
                };
                fieldSetting.Override.Name = new PropertyOverride<string>(fieldInstance.Data.Name);
                fieldSetting.Override.Decimal = new PropertyOverride<int>(fieldInstance.Data.Decimal);
                fieldSetting.Override.DayNightColorScheme = new DayNightColorScheme(fieldInstance.Data.Color);
                fieldSetting.Override.Maximum = new PropertyOverride<string>(fieldInstance.Data.Maximum);
                fieldSetting.Override.Minimum = new PropertyOverride<string>(fieldInstance.Data.Minimum);
                fieldSetting.Override.Step = new PropertyOverride<string>(fieldInstance.Data.Step);
                Settings.GameSettings[PluginManager.GameName].GaugeFields.Add(type.FullName, fieldSetting);
            }

            //Make sure that empty field can't be disabled.
            if (fieldInstance.GetType().FullName == EmptyGaugeField.FullName) fieldSetting.Enabled = true;

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

            fieldSetting.PropertyChanged += GaugeFieldSetting_PropertyChanged;
            fieldSetting.Override.NamePropertyChanged += GaugeFieldNameOverride_PropertyChanged;
            fieldSetting.Override.DecimalPropertyChanged += GaugeFieldDecimalOverride_PropertyChanged;
            fieldSetting.Override.ColorSchemePropertyChanged += GaugeFieldColorSchemeOverridePropertyChanged;

            fieldSetting.Override.MaximumPropertyChanged += GaugeFieldMaximumOverridePropertyChanged;
            fieldSetting.Override.MinimumPropertyChanged += GaugeFieldMinimumOverridePropertyChanged;
            fieldSetting.Override.StepPropertyChanged += GaugeFieldStepOverridePropertyChanged;
            GaugeFieldComponent fieldComponent = new GaugeFieldComponent(fieldInstance)
            {
                Enabled = fieldSetting.Enabled
            };
            allGaugeField.Add(fieldComponent);
            UpdateNameOverride(fieldSetting);
            UpdateColorOveride(fieldSetting);
            UpdateDecimalOverride(fieldSetting);
            UpdateMaximumOverride(fieldSetting);
            UpdateMinimumOverride(fieldSetting);
            UpdateStepOverride(fieldSetting);
        }

        private void GaugeFieldStepOverridePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.GaugeField settings)) return;
            UpdateStepOverride(settings);
        }

        private void UpdateStepOverride(Settings.GaugeField settings)
        {
            var field = allGaugeField.First(x => x.FullName == settings.FullName);
            if (field == null) return;

            field.FieldComponent.Data.Step = settings.Override.Step.Override
                ? settings.Override.Step.OverrideValue
                : settings.Override.Step.DefaultValue;
        }

        private void GaugeFieldMinimumOverridePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.GaugeField settings)) return;
            UpdateMinimumOverride(settings);
        }

        private void UpdateMinimumOverride(Settings.GaugeField settings)
        {
            var field = allGaugeField.First(x => x.FullName == settings.FullName);
            if (field == null) return;

            field.FieldComponent.Data.Minimum = settings.Override.Minimum.Override
                ? settings.Override.Minimum.OverrideValue
                : settings.Override.Minimum.DefaultValue;
        }

        private void GaugeFieldMaximumOverridePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.GaugeField settings)) return;
            UpdateMaximumOverride(settings);
        }

        private void UpdateMaximumOverride(Settings.GaugeField settings)
        {
            var field = allGaugeField.First(x => x.FullName == settings.FullName);
            if (field == null) return;

            field.FieldComponent.Data.Maximum = settings.Override.Maximum.Override
                ? settings.Override.Maximum.OverrideValue
                : settings.Override.Maximum.DefaultValue;
        }

        private void DataFieldNameOverride_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.DataField.OverrideProperties settings)) return;
            UpdateNameOverride(settings.parent);
        }

        private void UpdateNameOverride(Settings.DataField settings)
        {
            var field = allDataField.First(x => x.FullName == settings.FullName);
            if (field == null) return;

            field.FieldComponent.Data.Name = settings.Override.Name.Override
                ? settings.Override.Name.OverrideValue
                : settings.Override.Name.DefaultValue;
        }

        private void GaugeFieldNameOverride_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.GaugeField.OverrideProperties settings)) return;
            UpdateNameOverride(settings.parent);
        }

        private void UpdateNameOverride(Settings.GaugeField settings)
        {
            var field = allGaugeField.First(x => x.FullName == settings.FullName);
            if (field == null) return;

            field.FieldComponent.Data.Name = settings.Override.Name.Override
                ? settings.Override.Name.OverrideValue
                : settings.Override.Name.DefaultValue;
        }

        private void DataFieldColorSchemeOverridePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((sender is Settings.DataField.OverrideProperties settings))
            {
                UpdateColorOveride(settings.parent);
            }
        }

        private void GaugeFieldColorSchemeOverridePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Settings.GaugeField.OverrideProperties settings)
            {
                UpdateColorOveride(settings.parent);
            }
        }

        private void UpdateColorOveride(Settings.DataField settings)
        {
            var datafield = allDataField.FirstOrDefault(x => x.FullName == settings.FullName);
            if (datafield == null) return;

            if (!settings.Override.DayNightColorScheme.DayModeColor.Override)
            {
                datafield.FieldComponent.Data.Color = settings.Override.DayNightColorScheme.DayModeColor.DefaultValue;
                return;
            }

            if (BrightnessConfiguration.Configuration.IsNightMode && settings.Override.DayNightColorScheme.NightModeColor.Override)
            {
                datafield.FieldComponent.Data.Color = settings.Override.DayNightColorScheme.NightModeColor.OverrideValue;
                return;
            }
            datafield.FieldComponent.Data.Color = settings.Override.DayNightColorScheme.DayModeColor.OverrideValue;
        }

        private void UpdateColorOveride(Settings.GaugeField settings)
        {
            var gaugeField = allGaugeField.FirstOrDefault(x => x.FullName == settings.FullName);
            if (gaugeField == null) return;

            if (!settings.Override.DayNightColorScheme.DayModeColor.Override)
            {
                gaugeField.FieldComponent.Data.Color = settings.Override.DayNightColorScheme.DayModeColor.DefaultValue;
                return;
            }

            if (BrightnessConfiguration.Configuration.IsNightMode && settings.Override.DayNightColorScheme.NightModeColor.Override)
            {
                gaugeField.FieldComponent.Data.Color = settings.Override.DayNightColorScheme.NightModeColor.OverrideValue;
                return;
            }
            gaugeField.FieldComponent.Data.Color = settings.Override.DayNightColorScheme.DayModeColor.OverrideValue;
        }

        private void DataFieldDecimalOverride_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.DataField.OverrideProperties settings)) return;
            UpdateDecimalOverride(settings.parent);
        }

        private void UpdateDecimalOverride(Settings.DataField settings)
        {
            var field = allDataField.FirstOrDefault(x => x.FullName == settings.FullName);
            if (field == null) return;

            field.FieldComponent.Data.Decimal = settings.Override.Decimal.Override
                ? settings.Override.Decimal.OverrideValue
                : settings.Override.Decimal.DefaultValue;
        }

        private void GaugeFieldDecimalOverride_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.GaugeField.OverrideProperties settings)) return;
            UpdateDecimalOverride(settings.parent);
        }

        private void UpdateDecimalOverride(Settings.GaugeField settings)
        {
            var field = allGaugeField.FirstOrDefault(x => x.FullName == settings.FullName);
            if (field == null) return;

            field.FieldComponent.Data.Decimal = settings.Override.Decimal.Override
                ? settings.Override.Decimal.OverrideValue
                : settings.Override.Decimal.DefaultValue;
        }

        private void DataFieldSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.GaugeField settingsGaugeField)) return;
            switch (e.PropertyName)
            {
                case nameof(settingsGaugeField.Enabled):
                    var fieldComponent = allGaugeField.FirstOrDefault(f => f.FullName == settingsGaugeField.FullName);
                    if (fieldComponent == null)
                    {
                        SimHub.Logging.Current.Error($"Settings changed for a data field that's not loaded. Missing class: {settingsGaugeField.FullName}");
                        return;
                    }
                    fieldComponent.Enabled = settingsGaugeField.Enabled;
                    UpdateAvailableFields(fieldComponent);
                    break;
                default:
                    break;
            }
        }

        private void GaugeFieldSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.DataField settingsDataField)) return;
            switch (e.PropertyName)
            {
                case nameof(settingsDataField.Enabled):
                    var fieldComponent = allDataField.FirstOrDefault(f => f.FullName == settingsDataField.FullName);
                    if (fieldComponent == null)
                    {
                        SimHub.Logging.Current.Error($"Settings changed for a data field that's not loaded. Missing class: {settingsDataField.FullName}");
                        return;
                    }
                    fieldComponent.Enabled = settingsDataField.Enabled;
                    UpdateAvailableFields(fieldComponent);
                    break;
                default:
                    break;
            }
        }

        private void DayNightMode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            foreach (var field in availableDataField)
            {
                if (!(Settings.GameSettings[PluginManager.GameName].DataFields.TryGetValue(field.GetType().FullName, out var fieldSettings))) continue;
                UpdateColorOveride(fieldSettings);
            }
            foreach (var field in availableGaugeField)
            {
                if (!(Settings.GameSettings[PluginManager.GameName].GaugeFields.TryGetValue(field.GetType().FullName, out var fieldSettings))) continue;
                UpdateColorOveride(fieldSettings);
            }
        }

        private void UpdateAvailableFields()
        {
            availableDataField.Clear();
            foreach (var fieldComponent in allDataField)
            {
                if (fieldComponent.Enabled) availableDataField.Add(fieldComponent.FieldComponent);
            }

            availableGaugeField.Clear();
            foreach (var fieldComponent in allGaugeField)
            {
                if (fieldComponent.Enabled) availableGaugeField.Add(fieldComponent.FieldComponent);
            }
        }

        private void UpdateAvailableFields(DataFieldComponent fieldComponent)
        {
            if (fieldComponent.Enabled)
            {
                availableDataField.Add(fieldComponent.FieldComponent);
            }
            else
            {
                //Check if the field is in the displayed fields, and change it to empty field.
                //Then remove it from the available field list.
                for (int i = 0; i < dataFields.Count; i++)
                {
                    if (fieldComponent.FieldComponent.GetType().FullName == dataFields[i].GetType().FullName)
                    {
                        dataFields[i] = availableDataField.FirstOrDefault(f => f.GetType().FullName == EmptyDataField.Field.GetType().FullName);
                    }
                }
                availableDataField.Remove(fieldComponent.FieldComponent);
            }
        }

        private void UpdateAvailableFields(GaugeFieldComponent fieldComponent)
        {
            if (fieldComponent.Enabled)
            {
                availableGaugeField.Add(fieldComponent.FieldComponent);
            }
            else
            {
                //Check if the field is in the displayed fields, and change it to empty field.
                //Then remove it from the available field list.
                for (int i = 0; i < dataFields.Count; i++)
                {
                    if (fieldComponent.FieldComponent.GetType().FullName == gaugeFields[i].GetType().FullName)
                    {
                        gaugeFields[i] = availableGaugeField.FirstOrDefault(f => f.GetType().FullName == EmptyGaugeField.Field.GetType().FullName);
                    }
                }
                availableGaugeField.Remove(fieldComponent.FieldComponent);
            }
        }

        private int CurrentFieldIndex(IDataFieldComponent field)
        {
            return availableDataField.FindIndex(f => f == field);
        }

        private int CurrentFieldIndex(IGaugeFieldComponent field)
        {
            return availableGaugeField.FindIndex(f => f == field);
        }

        private IDataFieldComponent NextField(IDataFieldComponent currentField)
        {
            int maxIndex = availableDataField.Count - 1;
            int currentIndex = CurrentFieldIndex(currentField);
            int nextIndex = (currentIndex >= maxIndex) ? 0 : currentIndex + 1;
            return availableDataField[nextIndex];
        }

        private IGaugeFieldComponent NextField(IGaugeFieldComponent currentField)
        {
            int maxIndex = availableGaugeField.Count - 1;
            int currentIndex = CurrentFieldIndex(currentField);
            int nextIndex = (currentIndex >= maxIndex) ? 0 : currentIndex + 1;
            return availableGaugeField[nextIndex];
        }

        private IDataFieldComponent PrevField(IDataFieldComponent currentField)
        {
            int maxIndex = availableDataField.Count - 1;
            int currentIndex = CurrentFieldIndex(currentField);
            int prevIndex = (currentIndex <= 0) ? maxIndex : currentIndex - 1;
            return availableDataField[prevIndex];
        }

        private IGaugeFieldComponent PrevFied(IGaugeFieldComponent currentField)
        {
            int maxIndex = availableGaugeField.Count - 1;
            int currentIndex = CurrentFieldIndex(currentField);
            int prevIndex = (currentIndex <= 0) ? maxIndex : currentIndex - 1;
            return availableGaugeField[prevIndex];
        }
    }
}
