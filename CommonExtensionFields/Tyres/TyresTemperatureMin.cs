using DashMenu.Data;
using DashMenu.Data.Interfaces;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields.Tyres
{
    public sealed class TyresTemperatureMin : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public TyresTemperatureMin(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TTMIN",
                IsDecimalNumber = true,
                Decimal = 1,
                Color = new ColorScheme("#edc900")
            };
        }

        public string Description => "Min tire temperature for all tyres.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.TyresTemperatureMin);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
