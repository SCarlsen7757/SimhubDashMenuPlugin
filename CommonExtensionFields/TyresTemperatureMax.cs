using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    class TyresTemperatureMax : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public TyresTemperatureMax(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TTMAX",
                IsDecimalNumber = true,
                Decimal = 1,
                Color = new ColorScheme("#edc900")
            };
        }

        public string Description => "Max tire temperature for all tyres.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.TyresTemperatureMax);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
