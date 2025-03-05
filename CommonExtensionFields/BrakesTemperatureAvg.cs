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
                Color = new ColorScheme("#ed6b00")
            };
        }

        public string Description => "Avg Brake temperature for all brakes.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = data.NewData.BrakesTemperatureAvg.ToString();
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
