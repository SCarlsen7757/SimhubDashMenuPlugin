﻿using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;
using SimHub.Plugins.OutputPlugins.Dash.TemplatingCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;


namespace DashMenu
{
    [PluginDescription("Plugin to manage dash menus.")]
    [PluginAuthor("Mark Carlsen")]
    [PluginName("Dash menu")]
    public class DashMenuPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public DashMenuPlugin() { }

        private Settings.Settings Settings { get; set; }
        private MenuConfiguration MenuConfiguration { get; set; } = new MenuConfiguration() { MaxFields = 5 };
        //private static readonly EmptyField emptyField = new EmptyField();
        private IFieldDataComponent[] fieldData;
        /// <summary>
        /// List of all field data, used in the settings UI.
        /// </summary>
        private readonly ObservableCollection<FieldComponent> allFieldData = new ObservableCollection<FieldComponent>();
        /// <summary>
        /// List of available fields that can be used in the displayed fields.
        /// </summary>
        private readonly List<IFieldDataComponent> availableFieldData = new List<IFieldDataComponent>();

        private string oldCar = null;
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
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager) => new UI.SettingsControl(this.Settings, allFieldData);
        /// <summary>
        /// Called once after plugins startup.
        /// Plugins are rebuilt at game change.
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info($"{this.GetType().FullName}. Plugin: {this.PluginName}, Version {Version.PluginVersion}");

            allFieldData.Add(new FieldComponent(EmptyField.Field));
            Settings = this.ReadCommonSettings("DashMenuSettings", () => new Settings.Settings());
            GetCustomFields();
            //TODO : Make UI to be able to disable and enable Data field.

            pluginManager.AddProperty("ConfigMode", this.GetType(), MenuConfiguration.ConfigurationMode, "When in configuration mode, it's possible to change the displayed data.");
            pluginManager.AddProperty("ActiveConfigField", this.GetType(), MenuConfiguration.ActiveField, "Active field in the dash menu config screen.");

            pluginManager.AddAction<DashMenuPlugin>("ToggleConfigMode", (Action<PluginManager, string>)((pm, a) =>
            {
                MenuConfiguration.ConfigurationMode = !MenuConfiguration.ConfigurationMode;

                //When entering configuration mode, reset the active field to 1.
                if (MenuConfiguration.ConfigurationMode) MenuConfiguration.ActiveField = 0;

                pluginManager.SetPropertyValue("ConfigMode", GetType(), MenuConfiguration.ConfigurationMode);
                pluginManager.SetPropertyValue("ActiveConfigField", GetType(), MenuConfiguration.ActiveField + 1);

            }));

            pluginManager.AddAction<DashMenuPlugin>("ConfigNextField", (Action<PluginManager, string>)((pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                if (MenuConfiguration.ActiveField < MenuConfiguration.MaxFields - 1)
                {
                    MenuConfiguration.ActiveField++;
                }
                else
                {
                    MenuConfiguration.ActiveField = 0;
                }
                pluginManager.SetPropertyValue("ActiveConfigField", GetType(), MenuConfiguration.ActiveField + 1);
                SimHub.Logging.Current.Debug("Dash menu action ConfigNextField");
            }));

            pluginManager.AddAction<DashMenuPlugin>("ConfigPrevField", (Action<PluginManager, string>)((pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                if (MenuConfiguration.ActiveField > 0)
                {
                    MenuConfiguration.ActiveField--;
                }
                else
                {
                    MenuConfiguration.ActiveField = MenuConfiguration.MaxFields - 1;
                }
                pluginManager.SetPropertyValue("ActiveConfigField", GetType(), MenuConfiguration.ActiveField + 1);
                SimHub.Logging.Current.Debug("Dash menu action ConfigPrevField");

            }));

            pluginManager.AddAction<DashMenuPlugin>("ChangeFieldTypeNext", (Action<PluginManager, string>)((pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                var currentField = fieldData[MenuConfiguration.ActiveField];
                fieldData[MenuConfiguration.ActiveField] = NextField(currentField);
                SimHub.Logging.Current.Debug("Dash menu action ChangeFieldTypeNext");
            }));

            pluginManager.AddAction<DashMenuPlugin>("ChangeFieldTypePrev", (Action<PluginManager, string>)((pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                var currentField = fieldData[MenuConfiguration.ActiveField];
                fieldData[MenuConfiguration.ActiveField] = PrevField(currentField);
                SimHub.Logging.Current.Debug("Dash menu action ChangeFieldTypePrev");

            }));

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
            catch (System.ArgumentException)
            {
#if DEBUG
                throw;
#endif
            }
            pluginManager.AddProperty<bool>("PluginRunning", this.GetType(), true);
            PluginManagerEvents.Instance.ActiveCarChanged += CarChanged;
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
            //TODO: use DayNightMode for the colors at some point. var mode = SimHub.Plugins.BrightnessControl.BrightnessConfiguration.CurrentMode;
            if (fieldData == null) return;
            for (int i = 0; i < fieldData.Length; i++)
            {
                //TODO: Find a better way to implent GameSupported. So it's called in the init method.
                if (fieldData[i].GameSupported(pluginManager.GameName))
                {
                    fieldData[i].Update(ref data);
                }
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

            SaveDisplayedField(PluginManager.LastCarId);
            this.SaveCommonSettings("DashMenuSettings", Settings);
        }

        internal FieldData GetField(int index)
        {
            if (fieldData == null) return null;
            if (index < 0 && index >= fieldData.Length) return null;
            return fieldData[index].Data;
        }
        private void CarChanged(object sender, EventArgs e)
        {
            //Save field for previues car before loading new car field data settings
            if (!string.IsNullOrWhiteSpace(oldCar))
            {
                SaveDisplayedField(oldCar);
            }

            //Create arrays
            fieldData = new IFieldDataComponent[Settings.DefaultMaxFields];
            var test = Settings.GetDisplayedField(PluginManager.GameName, PluginManager.LastCarId);
            //Assign data from settings
            for (int i = 0; i < fieldData.Length; i++)
            {
                //Check if DisplayField is valid
                if (string.IsNullOrEmpty(test[i]))
                {
                    test[i] = EmptyField.Field.GetType().FullName;
                }
                else if (!availableFieldData.Any(f => test[i] == f.GetType().FullName))
                {
                    test[i] = EmptyField.Field.GetType().FullName;
                }

                //Asign data
                fieldData[i] = availableFieldData.FirstOrDefault(f => f.GetType().FullName == test[i]);
            }

            oldCar = PluginManager.LastCarId;
        }
        private void AllFieldData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null && e.NewItems.Count > 0)
                {
                    foreach (FieldComponent newItem in e.NewItems)
                    {
                        newItem.PropertyChanged += FieldComponent_PropertyChanged;
                        UpdateAvailableFieldData(newItem);
                    }
                }
            }
        }
        private void FieldComponent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateAvailableFieldData((FieldComponent)sender);
        }
        private void SaveDisplayedField(string car)
        {
            if (string.IsNullOrWhiteSpace(car)) return;
            var fieldDataSettings = new string[fieldData.Length];
            for (int i = 0; i < fieldData.Length; i++)
            {
                fieldDataSettings[i] = fieldData[i].GetType().FullName;
            }
            Settings.UpdateDisplayedField(PluginManager.GameName, car, fieldDataSettings);
        }
        private static IEnumerable<Type> GetCustomFieldsType(string sub_dir)
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
            Type interfaceType = typeof(IFieldDataComponent);

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
            foreach (Type type in GetCustomFieldsType("DashMenuCustomFields"))
            {
                //Get field settings else create field settings
                var fieldSetting = Settings.Fields.FirstOrDefault(s => s.FullName == type.FullName);
                if (fieldSetting == null)
                {
                    fieldSetting = new Settings.Fields() { Enabled = true, FullName = type.FullName };
                    this.Settings.Fields.Add(fieldSetting);
                }

                var fieldDataInstance = (IFieldDataComponent)Activator.CreateInstance(type);
                FieldComponent fieldComponent = new FieldComponent(fieldDataInstance) { Enabled = fieldSetting.Enabled };
                allFieldData.Add(fieldComponent);
            }

            foreach (var item in Settings.Fields)
            {
                item.PropertyChanged += FieldSetting_PropertyChanged;
            }

            UpdateAvailableFieldData();
        }

        private void FieldSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Settings.Fields settingsfield)
                if (e.PropertyName == nameof(settingsfield.Enabled))
                {
                    var fieldComponent = allFieldData.FirstOrDefault(f => f.FullName == settingsfield.FullName);
                    if (fieldComponent == null)
                    {
                        SimHub.Logging.Current.Error($"Settings changed for a data field that's not loaded. Missing class: {settingsfield.FullName}");
                        return;
                    }
                    fieldComponent.Enabled = settingsfield.Enabled;
                    UpdateAvailableFieldData(fieldComponent);
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
                for (int i = 0; i < fieldData.Length; i++)
                {
                    if (fieldComponent.FieldData.GetType().FullName == fieldData[i].GetType().FullName)
                    {
                        fieldData[i] = availableFieldData.FirstOrDefault(f => f.GetType().FullName == EmptyField.Field.GetType().FullName);
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
