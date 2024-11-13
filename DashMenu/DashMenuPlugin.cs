using DashMenu.Data;
using DashMenu.Extensions;
using GameReaderCommon;
using SimHub.Plugins;
using SimHub.Plugins.BrightnessControl;
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
    public class DashMenuPlugin : IPlugin, IDataPlugin, IWPFSettingsV2, ISettingPlugin
    {
        public DashMenuPlugin() { }

        private Settings.Settings Settings { get; set; }
        private MenuConfiguration MenuConfiguration { get; set; }
        private FieldManager.DataFieldManager DataFieldManager { get; set; }
        private FieldManager.GaugeFieldManager GaugeFieldManager { get; set; }
        private FieldManager.AlertManager AlertManager { get; set; }

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
        public PluginManager PluginManager { private get; set; }
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
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager) => new UI.SettingsControl(Settings.GetCurrentGameSettings());

        internal static class PropertyNames
        {
            public const string PluginRunning = "PluginRunning";
            public const string AmountOfGaugeFields = "AmountOfGaugeFields";
        }

        internal static class ActionNames
        {
            //If changing the name of actions remember to change the names in Settings Control.xaml under control section!
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
            MenuConfiguration = new MenuConfiguration(pluginManager, GetType());

            LoadSettings();

            DataFieldManager = new FieldManager.DataFieldManager(pluginManager, GetType());
            GaugeFieldManager = new FieldManager.GaugeFieldManager(pluginManager, GetType());
            AlertManager = new FieldManager.AlertManager();

            DataFieldManager.AddExtensionField(typeof(EmptyField), Settings.GetCurrentGameSettings().DataFields);
            GaugeFieldManager.AddExtensionField(typeof(EmptyField), Settings.GetCurrentGameSettings().GaugeFields);

            GetCustomFields();
            SettingsExtensionFieldsCleanUp();

            AlertManager.AddAlerts(DataFieldManager.AllFields, Settings.GetCurrentGameSettings().Alerts);
            AlertManager.UpdateSelectedAlerts(Settings.GetCurrentGameSettings().DataFields, Settings.GetCurrentGameSettings().Alerts);

            MenuConfiguration.ChangeDataFieldNext += DataFieldManager.NextSelectedField;
            MenuConfiguration.ChangeDataFieldPrev += DataFieldManager.PrevSelectedField;
            MenuConfiguration.ChangeGaugeFieldNext += GaugeFieldManager.NextSelectedField;
            MenuConfiguration.ChangeGaugeFieldPrev += GaugeFieldManager.PrevSelectedField;

            MenuConfiguration.IncreaseNumberOfDataFields += DataFieldManager.AddField;
            MenuConfiguration.DecreaseNumberOfDataFields += DataFieldManager.RemoveField;
            MenuConfiguration.IncreaseNumberOfGaugeFields += GaugeFieldManager.AddField;
            MenuConfiguration.DecreaseNumberOfGaugeFields += GaugeFieldManager.RemoveField;

            var gameSettings = Settings.GetCurrentGameSettings();
            DataFieldManager.SelectedFieldsChanged += gameSettings.UpdateDisplayedDataFields;
            GaugeFieldManager.SelectedFieldsChanged += gameSettings.UpdateDisplayedGaugeFields;

            gameSettings.CurrentCarFieldChanged += DataFieldManager.UpdateSelectedFields;
            gameSettings.CurrentCarFieldChanged += GaugeFieldManager.UpdateSelectedFields;

            gameSettings.DataFieldSettingsChanged += DataFieldManager.UpdateProperties;

            gameSettings.GaugeFieldSettingsChanged += GaugeFieldManager.UpdateProperties;

            gameSettings.AlertSettingsChanged += AlertManager.UpdateProperties;

            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ToggleConfigMode, (pm, a) => MenuConfiguration.ToggleConfigMode(pm));
            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ChangeFieldType, (pm, a) => MenuConfiguration.ChangeFieldType());
            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ConfigNextField, (pm, a) => MenuConfiguration.NextActiveField(CurrentFieldTypeCount));
            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ConfigPrevField, (pm, a) => MenuConfiguration.PrevActiveField(CurrentFieldTypeCount));
            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ChangeFieldTypeNext, (pm, a) => MenuConfiguration.ChangeFieldTypeNext());
            pluginManager.AddAction<DashMenuPlugin>(ActionNames.ChangeFieldTypePrev, (pm, a) => MenuConfiguration.ChangeFieldTypePrev());
            pluginManager.AddAction<DashMenuPlugin>(ActionNames.IncreaseNumberOfField, (pm, a) => MenuConfiguration.IncreaseNumberOfField());
            pluginManager.AddAction<DashMenuPlugin>(ActionNames.DecreaseNumberOfField, (pm, a) => MenuConfiguration.DecreaseNumberOfField());

            pluginManager.AddProperty<bool>(PropertyNames.PluginRunning, GetType(), true);
            PluginManagerEvents.Instance.ActiveCarChanged += Settings.GetCurrentGameSettings().CarChanged;
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
            foreach (IDataFieldExtension field in DataFieldManager.SelectedFields) field.Update(pluginManager, ref data);
            foreach (IGaugeFieldExtension field in GaugeFieldManager.SelectedFields) field.Update(pluginManager, ref data);
            foreach (var alert in AlertManager.SelectedAlerts) alert.Update(pluginManager, ref data);
        }

        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here!
        /// Plugins are rebuilt at game change.
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            PluginManagerEvents.Instance.ActiveCarChanged -= Settings.GetCurrentGameSettings().CarChanged;
            BrightnessConfiguration.Configuration.PropertyChanged -= DayNightMode_PropertyChanged;
        }

        private void LoadSettings()
        {
            //Get field settings else create field settings
            Settings = this.ReadCommonSettings("DashMenuSettings", () => new Settings.Settings());
        }

        public void SaveSettings()
        {
            this.SaveCommonSettings("DashMenuSettings", Settings);
        }

        public void SettingsExtensionFieldsCleanUp()
        {
            var gameSettings = Settings.GetCurrentGameSettings();

            CleanUpFields(gameSettings.DataFields, DataFieldManager.AllFields.Select(x => x.FullName));
            CleanUpFields(gameSettings.GaugeFields, GaugeFieldManager.AllFields.Select(x => x.FullName));
        }

        private void CleanUpFields<TSettingsField>(IDictionary<string, TSettingsField> fields, IEnumerable<string> allFieldNames)
        {
            var validFieldNames = new HashSet<string>(allFieldNames);

            foreach (var field in fields.Keys.ToList())
            {
                if (validFieldNames.Contains(field)) continue;
                fields.Remove(field);
            }
        }

        private int CurrentFieldTypeCount()
        {
            switch (MenuConfiguration.FieldType)
            {
                case FieldType.Data:
                    return DataFieldManager.SelectedFields.Count;
                case FieldType.Gauge:
                    return GaugeFieldManager.SelectedFields.Count;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DayNightMode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DataFieldManager.DayNightModeChanged(Settings.GetCurrentGameSettings().DataFields);
            GaugeFieldManager.DayNightModeChanged(Settings.GetCurrentGameSettings().GaugeFields);
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
            Type interfaceType = typeof(IDashMenuPluginExtension);

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
                    if (type.ContainsInterface(interfaceType))
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
            if (type.ContainsInterface(typeof(IDataFieldExtension)))
            {
                DataFieldManager.AddExtensionField(type, Settings.GetCurrentGameSettings().DataFields);
            }
            if (type.ContainsInterface(typeof(IGaugeFieldExtension)))
            {
                GaugeFieldManager.AddExtensionField(type, Settings.GetCurrentGameSettings().GaugeFields);
            }
        }
    }
}
