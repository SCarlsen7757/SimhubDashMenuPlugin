using DashMenu.Extensions;
using SimHub.Plugins;
using System;

namespace DashMenu
{
    internal class MenuConfiguration
    {
        internal static class PropertyNames
        {
            public const string ConfigMode = "ConfigMode";
            public const string ActiveConfigField = "ActiveConfigField";
            public const string FieldType = "FieldType";
        }
        public MenuConfiguration() { }
        public MenuConfiguration(PluginManager pluginManager, Type pluginType)
        {
            this.pluginManager = pluginManager;
            this.pluginType = pluginType;
            this.pluginManager.AddProperty(PropertyNames.ConfigMode, this.pluginType, ConfigurationMode);
            this.pluginManager.AddProperty(PropertyNames.ActiveConfigField, this.pluginType, ActiveField + 1);
            this.pluginManager.AddProperty(PropertyNames.FieldType, this.pluginType, FieldType.ToString());

            SimhubHelper.AddNCalcFunction("dashfielddataactive",
                "Returns true if the data field is active for configuration.",
                "index",
                engine => (Func<int, bool>)(index => DataFieldActive(index)));

            SimhubHelper.AddNCalcFunction("dashfieldgaugeactive",
                "Returns true if the gauge field is active for configuration.",
                "index",
                engine => (Func<int, bool>)(index => GaugeFieldActive(index)));
        }

        public delegate void ChangeFieldEventHandler(int index);
        public delegate void ChangeSizeEventHandler();

        public event ChangeFieldEventHandler ChangeDataFieldNext;
        public event ChangeFieldEventHandler ChangeDataFieldPrev;
        public event ChangeFieldEventHandler ChangeGaugeFieldNext;
        public event ChangeFieldEventHandler ChangeGaugeFieldPrev;

        public event ChangeSizeEventHandler IncreaseNumberOfDataFields;
        public event ChangeSizeEventHandler DecreaseNumberOfDataFields;
        public event ChangeSizeEventHandler IncreaseNumberOfGaugeFields;
        public event ChangeSizeEventHandler DecreaseNumberOfGaugeFields;

        private readonly PluginManager pluginManager;
        private readonly Type pluginType;
        private bool configurationMode = false;

        /// <summary>
        /// When confiugration mode is true, it's possible to change the displayed field configuration.
        /// </summary>
        internal bool ConfigurationMode
        {
            get => configurationMode;
            private set
            {
                if (configurationMode == value) return;
                configurationMode = value;
                pluginManager.SetPropertyValue(PropertyNames.ConfigMode, pluginType, ConfigurationMode);
            }
        }
        private int activeField = 0;
        /// <summary>
        /// Active field in the dash menu configuration window.
        /// </summary>
        internal int ActiveField
        {
            get => activeField;
            private set
            {
                if (activeField == value) return;
                activeField = value;
                pluginManager.SetPropertyValue(PropertyNames.ActiveConfigField, pluginType, ActiveField + 1);
            }
        }
        private FieldType fieldType = FieldType.Data;
        /// <summary>
        /// Field type that are active in configuration mode.
        /// </summary>
        internal FieldType FieldType
        {
            get => fieldType;
            private set
            {
                if (fieldType == value) return;
                fieldType = value;
                pluginManager.SetPropertyValue(PropertyNames.FieldType, pluginType, FieldType.ToString());
            }
        }

        internal bool DataFieldActive(int index)
        {
            return ConfigurationMode && FieldType == FieldType.Data && ActiveField == index - 1;
        }

        internal bool GaugeFieldActive(int index)
        {
            return ConfigurationMode && FieldType == FieldType.Gauge && ActiveField == index - 1;
        }

        internal void ToggleConfigMode(PluginManager pluginManager)
        {
            if (string.IsNullOrWhiteSpace(pluginManager.LastCarId)) return;
            ConfigurationMode = !ConfigurationMode;

            //When entering configuration mode, reset the active field to 1.
            if (ConfigurationMode)
            {
                ActiveField = 0;
                FieldType = FieldType.Data;
            }
        }

        internal void NextActiveField(Func<int> fieldCount)
        {
            if (!ConfigurationMode) return;
            if (ActiveField < fieldCount() - 1)
            {
                ActiveField++;
            }
            else
            {
                ActiveField = 0;
            }
        }
        internal void PrevActiveField(Func<int> fieldCount)
        {
            if (!ConfigurationMode) return;
            if (ActiveField > 0)
            {
                ActiveField--;
            }
            else
            {
                ActiveField = fieldCount() - 1;
            }
        }

        internal void ChangeFieldType()
        {
            if (!ConfigurationMode) return;
            //It works for now.
            switch (FieldType)
            {
                case FieldType.Data:
                    FieldType = FieldType.Gauge;
                    break;
                case FieldType.Gauge:
                    FieldType = FieldType.Data;
                    break;
                default:
                    throw new NotImplementedException(FieldType.ToString());
            }
            ActiveField = 0;
        }

        internal void ChangeFieldTypeNext()
        {
            if (!ConfigurationMode) return;
            switch (FieldType)
            {
                case FieldType.Data:
                    ChangeDataFieldNext?.Invoke(ActiveField);
                    break;
                case FieldType.Gauge:
                    ChangeGaugeFieldNext?.Invoke(ActiveField);
                    break;
                default:
                    throw new NotImplementedException(FieldType.ToString());
            }
        }

        internal void ChangeFieldTypePrev()
        {
            if (!ConfigurationMode) return;
            switch (FieldType)
            {
                case FieldType.Data:
                    ChangeDataFieldPrev?.Invoke(ActiveField);
                    break;
                case FieldType.Gauge:
                    ChangeGaugeFieldPrev?.Invoke(ActiveField);
                    break;
                default:
                    throw new NotImplementedException(FieldType.ToString());
            }
        }

        internal void IncreaseNumberOfField()
        {
            if (!ConfigurationMode) return;
            switch (FieldType)
            {
                case FieldType.Data:
                    IncreaseNumberOfDataFields?.Invoke();
                    break;
                case FieldType.Gauge:
                    IncreaseNumberOfGaugeFields?.Invoke();
                    break;
                default:
                    throw new NotImplementedException(FieldType.ToString());
            }
        }

        internal void DecreaseNumberOfField()
        {
            if (!ConfigurationMode) return;
            switch (FieldType)
            {
                case FieldType.Data:
                    DecreaseNumberOfDataFields?.Invoke();
                    break;
                case FieldType.Gauge:
                    DecreaseNumberOfGaugeFields?.Invoke();
                    break;
                default:
                    throw new NotImplementedException(FieldType.ToString());
            }
        }

    }
}
