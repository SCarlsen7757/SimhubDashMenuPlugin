using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace AccExtensionFields
{
    class RoadTemperature : AlertBase, IDataFieldExtension
    {
        public RoadTemperature(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TST",
                Color = new ColorScheme("#b00273")
            };
            Data.PropertyChanged += DataAlert_PropertyChanged;
        }

        public string Description => "Track Surface Temperature.";

        protected override bool GameSupported(string gameName)
        {
            return gameName == "AssettoCorsaCompetizione";
        }

        public override void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            var roadTemp = data.NewData.RoadTemperature;
            if (roadTemp < 0)
            {
                Data.Value = "-";
                Data.Unit = string.Empty;
                return;
            }
            Data.Value = roadTemp.ToString();
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
