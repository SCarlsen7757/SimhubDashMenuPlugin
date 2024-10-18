using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class OilTemperature : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public OilTemperature(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "EOT",
                IsDecimalNumber = true,
                Decimal = 0,
                Color = new ColorScheme(),
                Maximum = 150.ToString(),
                Minimum = 50.ToString(),
            };
        }

        public string Description => "Oil temperature. EOT stands for Engine oil temperature.";
        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.OilTemperature <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = DecimalValue(data.NewData.OilTemperature);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
