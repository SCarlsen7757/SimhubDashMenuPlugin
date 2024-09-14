using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class FuelProcent : FieldExtensionBase, IGaugeFieldComponent
    {
        public FuelProcent(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "Fuel",
                IsDecimalNumber = true,
                Decimal = 0,
                Unit = "%",
                Color = new ColorScheme("#ffffff", "#000000"),
                IsRangeLocked = true,
                Maximum = 100.ToString(),
                Minimum = 0.ToString()
            };
        }

        public string Description { get => "Fuel in procent"; }

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

        IDataField IDataFieldComponent.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }

        IGaugeField IGaugeFieldComponent.Data
        {
            get => Data;
            set => Data = value; //Make sure to set base Data
        }

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.FuelPercent);
        }
    }
}
