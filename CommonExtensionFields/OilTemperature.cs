using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class OilTemperature : FieldExtensionBase, IGaugeFieldComponent
    {
        public OilTemperature(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "Oil Temp",
                IsDecimalNumber = true,
                Decimal = 0,
                Color = new ColorScheme("#ffffff", "#808080"),
                Maximum = 150.ToString(),
                Minimum = 50.ToString(),
            };
        }

        public string Description { get => "Oil temperature"; }

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
            if (data.NewData.OilTemperature <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = DecimalValue(data.NewData.OilTemperature);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
