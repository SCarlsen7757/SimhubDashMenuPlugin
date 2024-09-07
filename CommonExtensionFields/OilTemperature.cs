using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class OilTemperature : FieldExtensionBase, IGaugeFieldComponent
    {
        public OilTemperature(string gameName) : base(gameName) { }
        public string Description { get => "Oil temperature"; }
        public IGaugeField Data { get; set; } = new GaugeField()
        {
            Name = "Oil Temp",
            IsDecimalNumber = true,
            Decimal = 0,
            Color = new ColorScheme("#ffffff", "#ffffff"),
            Maximum = 150.ToString(),
            Minimum = 50.ToString(),
        };
        IDataField IDataFieldComponent.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }
        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.OilTemperature <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.OilTemperature.ToString($"N{Data.Decimal}");
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
