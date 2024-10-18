using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class FuelVolume : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public FuelVolume(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "FUEL",
                IsDecimalNumber = true,
                Decimal = 0,
                Unit = "%",
                Color = new ColorScheme(),
                IsRangeLocked = true,
                Minimum = 0.ToString()
            };
        }

        public string Description => "Fuel in liters or gallons depending on what Simhub is configured to display.";
        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Maximum = data.NewData.CarSettings.MaxFuel.ToString();
            Data.Value = DecimalValue(data.NewData.Fuel);
            Data.Unit = data.NewData.FuelUnit[0].ToString();
        }
    }
}
