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
                Color = new ColorScheme("#ed6b00")
            };
        }

        public string Description => "Min Brake temperature for all brakes.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = data.NewData.BrakesTemperatureMin.ToString();
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
