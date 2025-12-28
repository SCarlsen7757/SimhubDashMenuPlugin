using DashMenu.Data;
using DashMenu.Data.Interfaces;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields.Engine
{
    public sealed class WaterTemperature : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public WaterTemperature(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "WT",
                IsDecimalNumber = true,
                Decimal = 0,
                Color = new ColorScheme(),
                Maximum = 100.ToString(),
                Minimum = 20.ToString()
            };
        }

        public string Description => "Water temperature.";

        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.WaterTemperature <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = DecimalValue(data.NewData.WaterTemperature);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
