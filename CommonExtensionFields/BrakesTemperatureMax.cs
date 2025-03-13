using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    class BrakesTemperatureMax : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public BrakesTemperatureMax(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "BTMAX",
                IsDecimalNumber = true,
                Decimal = 0,
                Color = new ColorScheme("#ed6b00")
            };
        }

        public string Description => "Max Brake temperature for all brakes.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.BrakesTemperatureAvg);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
