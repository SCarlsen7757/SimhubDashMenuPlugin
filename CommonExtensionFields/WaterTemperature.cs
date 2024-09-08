using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class WaterTemperature : FieldExtensionBase, IGaugeFieldComponent
    {
        public WaterTemperature(string gameName) : base(gameName) { }
        public string Description { get => "Water temperature"; }
        public IGaugeField Data { get; set; } = new GaugeField()
        {
            Name = "Water Temp",
            IsDecimalNumber = true,
            Decimal = 0,
            Color = new ColorScheme("#ffffff", "#000000"),
            Maximum = 100.ToString(),
            Minimum = 20.ToString()
        };
        IDataField IDataFieldComponent.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.WaterTemperature <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.WaterTemperature.ToString($"N{Data.Decimal}");
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
