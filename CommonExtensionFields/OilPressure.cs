using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{

    public class OilPressure : FieldExtensionBase, IGaugeFieldComponent
    {
        public OilPressure(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "Oil Press",
                IsDecimalNumber = true,
                Decimal = 0,
                Color = new ColorScheme("#ffffff", "#000000"),
                Maximum = 150.ToString(),
                Minimum = 50.ToString()
            };
        }
        public string Description { get => "Oil pressure"; }

        private IGaugeField data;
        new IGaugeField Data
        {
            get => data;
            set
            {
                data = value;
                base.Data = value; //Make sure to set base Data
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
            set => Data = value;
        }

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.OilPressure <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = DecimalValue(data.NewData.OilPressure);
            Data.Unit = data.NewData.OilPressureUnit;
        }
    }
}
