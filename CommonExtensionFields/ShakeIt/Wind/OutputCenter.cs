using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields.ShakeIt.Wind
{
    public class OutputCenter : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        private bool shakeItWindPluginLoaded = true;

        public OutputCenter(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "WC",
                IsDecimalNumber = true,
                Decimal = 1,
                Unit = "%",
                Color = new ColorScheme(),
                IsRangeLocked = true,
                Maximum = 100.ToString(),
                Minimum = 0.ToString()
            };
        }
        public string Description => "ShakeIt Wind output center.";

        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!shakeItWindPluginLoaded) return;

            var objectOutput = pluginManager.GetPropertyValue("ShakeItWindPlugin.OutputCenter");

            if (objectOutput is null)
            {
                shakeItWindPluginLoaded = false;
                return;
            }

            Data.Value = DecimalValue((double)objectOutput);
        }
    }
}
