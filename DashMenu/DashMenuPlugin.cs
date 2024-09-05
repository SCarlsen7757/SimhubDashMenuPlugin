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
        private List<IFieldDataComponent> fieldData = new List<IFieldDataComponent>();
        /// <summary>
        /// List of all field data, used in the settings UI.
        /// </summary>
        private readonly ObservableCollection<FieldComponent> allFieldData = new ObservableCollection<FieldComponent>();
        /// <summary>
        /// List of available fields that can be used in the displayed fields.
        /// </summary>
        private readonly List<IFieldDataComponent> availableFieldData = new List<IFieldDataComponent>();

        private string oldCarId = null;
        private string oldCarModel = null;
        public string PluginName
        {
            get
            {
                PluginNameAttribute attribute = (PluginNameAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(PluginNameAttribute));
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
        /// <summary>
        /// Called once after plugins startup.
        /// Plugins are rebuilt at game change.
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info($"{this.GetType().FullName}. Plugin: {this.PluginName}, Version {Version.PluginVersion}");
            LoadSettings();

            //Check if Empty field is in settings else add it
            AddExtensionComponent(typeof(EmptyDataField));

            GetCustomFields();
            //TODO : Make UI to be able to disable and enable Data field.
            pluginManager.AddProperty("ConfigMode", this.GetType(), MenuConfiguration.ConfigurationMode, "When in configuration mode, it's possible to change the displayed data.");
            pluginManager.AddProperty("ActiveConfigField", this.GetType(), MenuConfiguration.ActiveField, "Active field in the dash menu config screen.");
            pluginManager.AddProperty("AmountOfFields", GetType(), fieldData.Count, "Amount of fields for the current car.");
            pluginManager.AddAction<DashMenuPlugin>("ToggleConfigMode", (pm, a) =>
            {
                if (fieldData.Count == 0) return;
                MenuConfiguration.ConfigurationMode = !MenuConfiguration.ConfigurationMode;

                //When entering configuration mode, reset the active field to 1.
                if (MenuConfiguration.ConfigurationMode)
                {
                    MenuConfiguration.ActiveField = 0;
                }
                else
                {
                    //Save displaye field when exiting config mode.
                    SaveDisplayedField(PluginManager.LastCarId, PluginManager.GameManager.CarManager.LastCarSettings.CarModel);
                }

                pluginManager.SetPropertyValue("ConfigMode", GetType(), MenuConfiguration.ConfigurationMode);
                pluginManager.SetPropertyValue("ActiveConfigField", GetType(), MenuConfiguration.ActiveField + 1);
            });

            pluginManager.AddAction<DashMenuPlugin>("ConfigNextField", (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                if (MenuConfiguration.ActiveField < fieldData.Count - 1)
                {
                    MenuConfiguration.ActiveField++;
                }
                else
                {
                    MenuConfiguration.ActiveField = 0;
                }
                pluginManager.SetPropertyValue("ActiveConfigField", GetType(), MenuConfiguration.ActiveField + 1);
                SimHub.Logging.Current.Debug("Dash menu action ConfigNextField");
            });

            pluginManager.AddAction<DashMenuPlugin>("ConfigPrevField", (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                if (MenuConfiguration.ActiveField > 0)
                {
                    MenuConfiguration.ActiveField--;
                }
                else
                {
                    MenuConfiguration.ActiveField = fieldData.Count - 1;
                }
                pluginManager.SetPropertyValue("ActiveConfigField", GetType(), MenuConfiguration.ActiveField + 1);
                SimHub.Logging.Current.Debug("Dash menu action ConfigPrevField");
            });

            pluginManager.AddAction<DashMenuPlugin>("ChangeFieldTypeNext", (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                var currentField = fieldData[MenuConfiguration.ActiveField];
                fieldData[MenuConfiguration.ActiveField] = NextField(currentField);
                SimHub.Logging.Current.Debug("Dash menu action ChangeFieldTypeNext");
            });

            pluginManager.AddAction<DashMenuPlugin>("ChangeFieldTypePrev", (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                var currentField = fieldData[MenuConfiguration.ActiveField];
                fieldData[MenuConfiguration.ActiveField] = PrevField(currentField);
                SimHub.Logging.Current.Debug("Dash menu action ChangeFieldTypePrev");
            });

            pluginManager.AddAction<DashMenuPlugin>("IncreaseNumberOfFieldData", (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                if (fieldData.Count <= 0 || fieldData.Count >= 20) return;
                fieldData.Add(EmptyDataField.Field);
                pluginManager.SetPropertyValue("AmountOfFields", GetType(), fieldData.Count);
            });

            pluginManager.AddAction<DashMenuPlugin>("DecreaseNumberOfFieldData", (pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                if (fieldData.Count <= 1) return;
                fieldData.RemoveAt(fieldData.Count - 1);
                pluginManager.SetPropertyValue("AmountOfFields", GetType(), fieldData.Count);
            });

            //Add NCalc method
            //NCalc Methods are normaly added when Simhub starts. So when this plugin is initialize after the first time, due to settings change or other game selected.
            //It will add the method again, but it's already added.
            try
            {
                if (!NCalcEngineMethodsRegistry.GenericMethodsProvider.ContainsKey("dashfielddata".ToLower()))
                {
                    NCalcEngineMethodsRegistry.AddMethod("dashfielddata",
                        "field",
                        "Returns the data of the specified field.",
                        engine => (Func<int, object>)(field => GetField(field)));
                }
            }
            catch (ArgumentException)
            {
#if DEBUG
                throw;
#endif
            }
            pluginManager.AddProperty<bool>("PluginRunning", this.GetType(), true);
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
            foreach (IFieldDataComponent field in fieldData)
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

        internal IDataField GetField(int index)
        {
            if (index <= 0 || index > fieldData.Count) return null;
            return fieldData[index - 1].Data;
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
            fieldData = new List<IFieldDataComponent>();
            var fieldDataSettings = Settings.GameSettings[PluginManager.GameName].GetDisplayedField(PluginManager.LastCarId);
            //Assign data from settings
            for (int i = 0; i < fieldDataSettings.Count; i++)
            {
                //Check if DisplayField is valid
                if (string.IsNullOrEmpty(fieldDataSettings[i]) || !availableFieldData.Any(f => fieldDataSettings[i] == f.GetType().FullName))
                {
                    fieldDataSettings[i] = EmptyDataField.FullName;
                }

                //Asign data
                fieldData.Add(availableFieldData.FirstOrDefault(f => f.GetType().FullName == fieldDataSettings[i]));
            }
            SaveDisplayedField(PluginManager.LastCarId, PluginManager.GameManager.CarManager.LastCarSettings.CarModel);
            Settings.GameSettings[PluginManager.GameName].CarFields[PluginManager.LastCarId].IsActive = true;

            PluginManager.SetPropertyValue("AmountOfFields", GetType(), fieldData.Count);
            oldCarId = PluginManager.LastCarId;
            oldCarModel = PluginManager.GameManager.CarManager.LastCarSettings.CarModel;
        }
        private void FieldComponent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateAvailableFieldData((FieldComponent)sender);
        }
        private void SaveDisplayedField(string carId, string carModel)
        {
            if (string.IsNullOrWhiteSpace(carId) || string.IsNullOrWhiteSpace(carModel)) return;
            var fieldDataSettings = new ObservableCollection<string>();
            for (int i = 0; i < fieldData.Count; i++)
            {
                fieldDataSettings.Add(fieldData[i].GetType().FullName);
            }
            Settings.GameSettings[PluginManager.GameName].UpdateDisplayedField(carId, carModel, fieldDataSettings);
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
            UpdateAvailableFieldData();
        }

        private void AddExtensionComponent(Type type)
        {
            IFieldDataComponent fieldDataInstance;
            try
            {
                fieldDataInstance = (IFieldDataComponent)Activator.CreateInstance(type, PluginManager.GameName);
            }
            catch (Exception e)
            {
                SimHub.Logging.Current.Error(type, e);
                return;
            }
            //Get field settings else create field settings
            if (!(Settings.GameSettings[PluginManager.GameName].DataFields.TryGetValue(type.FullName, out DataFields fieldSetting)))
            {
                fieldSetting = new DataFields
                {
                    Enabled = true,
                    NameOverride = new PropertyOverride<string>(fieldDataInstance.Data.Name),
                    DecimalOverride = new PropertyOverride<int>(fieldDataInstance.Data.Decimal),
                    DayNightColorScheme = new DayNightColorScheme(fieldDataInstance.Data.Color)
                };
                Settings.GameSettings[PluginManager.GameName].DataFields.Add(type.FullName, fieldSetting);
            }
            fieldSetting.Namespace = type.Namespace;
            fieldSetting.Name = type.Name;
            fieldSetting.FullName = type.FullName;
            fieldSetting.IsDecimal = fieldDataInstance.Data.IsDecimalNumber;

            fieldSetting.GameSupported = fieldDataInstance.IsGameSupported;
            fieldSetting.SupportedGames = fieldDataInstance.SupportedGames;

            fieldSetting.PropertyChanged += FieldSetting_PropertyChanged;
            fieldSetting.NameOverridePropertyChanged += NameOverride_PropertyChanged;
            fieldSetting.DecimalOverridePropertyChanged += DecimalOverride_PropertyChanged;
            fieldSetting.ColorSchemeOverridePropertyChanged += FieldSetting_ColorSchemeOverridePropertyChanged;
            FieldComponent fieldComponent = new FieldComponent(fieldDataInstance)
            {
                Enabled = fieldSetting.Enabled
            };
            allFieldData.Add(fieldComponent);
            UpdateNameOverride(fieldSetting);
            UpdateColorOveride(fieldSetting);
            UpdateDecimalOverride(fieldSetting);
        }

        private void NameOverride_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.DataFields fieldSettings)) return;
            UpdateNameOverride(fieldSettings);
        }
        private void UpdateNameOverride(Settings.DataFields fieldSettings)
        {
            var fieldData = allFieldData.FirstOrDefault(x => x.FullName == fieldSettings.FullName);
            if (fieldData == null) return;

            if (fieldSettings.NameOverride.Override)
            {
                fieldData.FieldData.Data.Name = fieldSettings.NameOverride.OverrideValue;
            }
            else
            {
                fieldData.FieldData.Data.Name = fieldSettings.NameOverride.DefaultValue;
            }
        }
        private void FieldSetting_ColorSchemeOverridePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.DataFields fieldSettings)) return;
            UpdateColorOveride(fieldSettings);
        }

        private void UpdateColorOveride(DataFields fieldSettings)
        {
            var fieldData = allFieldData.FirstOrDefault(x => x.FullName == fieldSettings.FullName);
            if (fieldData == null) return;

            if (!fieldSettings.DayNightColorScheme.DayModeColor.Override)
            {
                fieldData.FieldData.Data.Color = fieldSettings.DayNightColorScheme.DayModeColor.DefaultValue;
                return;
            }

            if (BrightnessConfiguration.Configuration.IsNightMode && fieldSettings.DayNightColorScheme.NightModeColor.Override)
            {
                fieldData.FieldData.Data.Color = fieldSettings.DayNightColorScheme.NightModeColor.OverrideValue;
                return;
            }
            fieldData.FieldData.Data.Color = fieldSettings.DayNightColorScheme.DayModeColor.OverrideValue;
        }

        private void DecimalOverride_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Settings.DataFields fieldSettings)) return;
            UpdateDecimalOverride(fieldSettings);
        }
        private void UpdateDecimalOverride(Settings.DataFields fieldSettings)
        {
            var fieldData = allFieldData.FirstOrDefault(x => x.FullName == fieldSettings.FullName);
            if (fieldData == null) return;

            if (fieldSettings.DecimalOverride.Override)
            {
                fieldData.FieldData.Data.Decimal = fieldSettings.DecimalOverride.OverrideValue;
            }
            else
            {
                fieldData.FieldData.Data.Decimal = fieldSettings.DecimalOverride.DefaultValue;
            }
        }
        private void FieldSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is DataFields settingsfield)) return;
            switch (e.PropertyName)
            {
                case nameof(settingsfield.Enabled):
                    var fieldComponent = allFieldData.FirstOrDefault(f => f.FullName == settingsfield.FullName);
                    if (fieldComponent == null)
                    {
                        SimHub.Logging.Current.Error($"Settings changed for a data field that's not loaded. Missing class: {settingsfield.FullName}");
                        return;
                    }
                    fieldComponent.Enabled = settingsfield.Enabled;
                    UpdateAvailableFieldData(fieldComponent);
                    break;
                default:
                    break;
            }
        }
        private void DayNightMode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            foreach (var field in availableFieldData)
            {
                if (!(Settings.GameSettings[PluginManager.GameName].DataFields.TryGetValue(field.GetType().FullName, out var fieldSettings))) continue;
                UpdateColorOveride(fieldSettings);
            }
        }

        private void UpdateAvailableFieldData()
        {
            availableFieldData.Clear();
            foreach (var fieldComponent in allFieldData)
            {
                if (fieldComponent.Enabled) availableFieldData.Add(fieldComponent.FieldData);
            }
        }
        private void UpdateAvailableFieldData(FieldComponent fieldComponent)
        {
            if (fieldComponent.Enabled)
            {
                availableFieldData.Add(fieldComponent.FieldData);
            }
            else
            {
                //Check if the field data is in the displayed fields, and change it to empty field.
                //Then remove it from the available field data list.
                for (int i = 0; i < fieldData.Count; i++)
                {
                    if (fieldComponent.FieldData.GetType().FullName == fieldData[i].GetType().FullName)
                    {
                        fieldData[i] = availableFieldData.FirstOrDefault(f => f.GetType().FullName == EmptyDataField.Field.GetType().FullName);
                    }
                }
                availableFieldData.Remove(fieldComponent.FieldData);
            }
        }
        private int CurrentFieldIndex(IFieldDataComponent fieldData)
        {
            return availableFieldData.FindIndex(f => f == fieldData);
        }
        private IFieldDataComponent NextField(IFieldDataComponent currentField)
        {
            int maxIndex = availableFieldData.Count - 1;
            int currentIndex = CurrentFieldIndex(currentField);
            int nextIndex = (currentIndex >= maxIndex) ? 0 : currentIndex + 1;
            return availableFieldData[nextIndex];
        }

        private IFieldDataComponent PrevField(IFieldDataComponent currentField)
        {
            int maxIndex = availableFieldData.Count - 1;
            int currentIndex = CurrentFieldIndex(currentField);
            int prevIndex = (currentIndex <= 0) ? maxIndex : currentIndex - 1;
            return availableFieldData[prevIndex];
        }
    }
}
