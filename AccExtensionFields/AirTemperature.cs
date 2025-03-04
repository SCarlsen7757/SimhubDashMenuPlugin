using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace AccExtensionFields
{
    class AirTemperature : AlertBase, IDataFieldExtension
    {
        public AirTemperature(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "AMB",
                Color = new ColorScheme("#b00273")
            };
            Data.PropertyChanged += DataAlert_PropertyChanged;
        }

        public string Description => "Ambient Temperature.";

        protected override bool GameSupported(string gameName)
        {
            return gameName == "AssettoCorsaCompetizione";
        }

        public override void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            var airTemp = data.NewData.AirTemperature;
            if (airTemp < 0)
            {
                Data.Value = "-";
                Data.Unit = string.Empty;
                return;
            }
            Data.Value = airTemp.ToString();
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
