using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    class TyresTemperatureMin : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public TyresTemperatureMin(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TTMIN",
                Color = new ColorScheme("#edc900")
            };
        }

        public string Description => "Min tire temperature for all tyres.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = data.NewData.TyresTemperatureMin.ToString();
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
