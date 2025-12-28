using DashMenu.Data;
using DashMenu.Data.Interfaces;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields.Tyres
{
    public sealed class TyresTemperatureAvg : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public TyresTemperatureAvg(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TTAVG",
                IsDecimalNumber = true,
                Decimal = 1,
                Color = new ColorScheme("#edc900")
            };
        }

        public string Description => "Avg tire temperature for all tyres.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.TyresTemperatureAvg);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
