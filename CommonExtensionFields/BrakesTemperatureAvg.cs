using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    class BrakesTemperatureAvg : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public BrakesTemperatureAvg(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "BTAVG",
                IsDecimalNumber = true,
                Decimal = 0,
                Color = new ColorScheme("#ed6b00")
            };
        }

        public string Description => "Avg Brake temperature for all brakes.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.BrakesTemperatureAvg);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
