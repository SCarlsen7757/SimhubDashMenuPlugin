using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    class BrakesTemperatureMin : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public BrakesTemperatureMin(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "BTMIN",
                IsDecimalNumber = true,
                Decimal = 0,
                Color = new ColorScheme("#ed6b00")
            };
        }

        public string Description => "Min Brake temperature for all brakes.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.BrakesTemperatureMin);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
