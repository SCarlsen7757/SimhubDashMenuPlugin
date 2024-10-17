using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class FuelVolume : FieldExtensionBase, IGaugeFieldExtension
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

        private IGaugeField data;
        new IGaugeField Data
        {
            get => data;
            set
            {
                data = value;
                base.Data = value;
            }
        }

        IDataField IDataFieldExtension.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }

        IGaugeField IGaugeFieldExtension.Data
        {
            get => Data;
            set => Data = value; //Make sure to set base Data
        }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Maximum = data.NewData.CarSettings.MaxFuel.ToString();
            Data.Value = DecimalValue(data.NewData.Fuel);
            Data.Unit = data.NewData.FuelUnit[0].ToString();
        }
    }
}
