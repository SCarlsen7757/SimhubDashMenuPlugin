using GameReaderCommon;
using SimHub.Plugins;
using SimHub.Plugins.OutputPlugins.Dash.TemplatingCommon;
using System;
using System.Collections.Generic;
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
        private IFieldData[] fieldData;
        private readonly EmptyField emptyField = new EmptyField();
        private List<FieldComponent> allFieldData = new List<FieldComponent>();
        private List<IFieldData> availableFieldData = new List<IFieldData>();

        public string PluginName
        {
            get
            {
                PluginNameAttribute attribute = (PluginNameAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(PluginNameAttribute));
                return attribute.name;
            }
        }
        /// <summary>
        /// Instance of the current plugin manager
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
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new UI.SettingsControl();
        }
        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info($"Starting plugin: {this.PluginName}, Version {Version.PluginVersion}");

            allFieldData.Add(new FieldComponent(emptyField));
            Settings = this.ReadCommonSettings("DashMenuSettings", () => new Settings.Settings());
            GetCustomFields();
            UpdateAvailableFieldData();
            LoadFields();
            for (int i = 0; i < fieldData.Length; i++)
            {
                pluginManager.AddProperty($"FieldData{i + 1}", GetType(), fieldData[i].Data);
            }
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
            }));

            pluginManager.AddAction<DashMenuPlugin>("ChangeFieldTypeNext", (Action<PluginManager, string>)((pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                var currentField = fieldData[MenuConfiguration.ActiveField];
                fieldData[MenuConfiguration.ActiveField] = NextField(currentField);
            }));


            pluginManager.AddAction<DashMenuPlugin>("ChangeFieldTypePrev", (Action<PluginManager, string>)((pm, a) =>
            {
                if (!MenuConfiguration.ConfigurationMode) return;
                var currentField = fieldData[MenuConfiguration.ActiveField];
                fieldData[MenuConfiguration.ActiveField] = PrevField(currentField);
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
                        (Func<NCalcEngineBase, Delegate>)(engine => (Delegate)(Func<int, object>)(field => { if (field > 0 && field <= fieldData.Length) { return fieldData[field - 1].Data; } else { return null; } })));
                }
            }
            catch (System.ArgumentException)
            {
#if DEBUG
                throw;
#endif
            }


            pluginManager.AddProperty<bool>("PluginRunning", this.GetType(), true);
            SimHub.Logging.Current.Info("Plugin started");
        }
        /// <summary>
        /// Called one time per game data update, contains all normalized game data,
        /// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        ///
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        ///
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including current and previous data frame.</param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            for (int i = 0; i < fieldData.Length; i++)
            {
                fieldData[i].Update(ref data);
                pluginManager.SetPropertyValue($"FieldData{i + 1}", GetType(), fieldData[i].Data);
            }
        }
        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here !
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            if (Settings != null)
            {
                SaveField();
                this.SaveCommonSettings("DashMenuSettings", Settings);
            }
        }

        private void LoadFields()
        {
            //Create arrays
            fieldData = new IFieldData[Settings.MaxFields];
            if (Settings.DisplayedFields == null || Settings.DisplayedFields.Length != Settings.MaxFields)
            {
                Settings.DisplayedFields = new string[Settings.MaxFields];
            }

            //Assign data from settings
            for (int i = 0; i < fieldData.Length; i++)
            {
                //Check if DisplayField is valid
                if (string.IsNullOrEmpty(Settings.DisplayedFields[i]))
                {
                    Settings.DisplayedFields[i] = emptyField.GetType().FullName;
                }
                else if (!availableFieldData.Any(f => Settings.DisplayedFields[i] == f.GetType().FullName))
                {
                    Settings.DisplayedFields[i] = emptyField.GetType().FullName;
                }

                //Asign data
                fieldData[i] = availableFieldData.FirstOrDefault(f => f.GetType().FullName == Settings.DisplayedFields[i]);
            }
        }

        private void SaveField()
        {
            for (int i = 0; i < fieldData.Length; i++)
            {
                Settings.DisplayedFields[i] = fieldData[i].GetType().FullName;
            }
        }

        private static IEnumerable<Type> GetCustomFieldsType(string sub_dir)
        {
            string rootDirectory = Path.Combine(Path.GetDirectoryName((Assembly.GetExecutingAssembly().Location)), sub_dir);
            string[] dllFiles = Directory.GetFiles(rootDirectory, "*.dll");
            Type interfaceType = typeof(IFieldData);

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
                var settings = this.Settings.Fields.FirstOrDefault(s => s.FullName == type.FullName);
                bool enabled = (settings == null) || settings.Enabled;

                if (settings == null)
                {
                    var field = new Settings.Fields() { Enabled = true, FullName = type.FullName };
                    this.Settings.Fields.Add(field);
                }

                var fieldDataInstance = (IFieldData)Activator.CreateInstance(type);
                FieldComponent fieldComponent = new FieldComponent() { Enabled = enabled, FieldData = fieldDataInstance };
                allFieldData.Add(fieldComponent);
            }
        }

        private void UpdateAvailableFieldData()
        {
            foreach (var fieldComponent in allFieldData)
            {
                if (fieldComponent.Enabled) availableFieldData.Add(fieldComponent.FieldData);
            }
        }

        private int CurrentFieldIndex(IFieldData fieldData)
        {
            return availableFieldData.FindIndex(f => f == fieldData);
        }
        private IFieldData NextField(IFieldData currentField)
        {
            int maxIndex = allFieldData.Count - 1;
            int currentIndex = CurrentFieldIndex(currentField);
            int nextIndex = (currentIndex >= maxIndex) ? 0 : currentIndex + 1;
            return availableFieldData[nextIndex];
        }

        private IFieldData PrevField(IFieldData currentField)
        {
            int maxIndex = allFieldData.Count - 1;
            int currentIndex = CurrentFieldIndex(currentField);
            int prevIndex = (currentIndex <= 0) ? maxIndex : currentIndex - 1;
            return availableFieldData[prevIndex];
        }
    }
}
