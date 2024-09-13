using DashMenu.Data;
using GameReaderCommon;


namespace CommonExtensionFields
{
    public class Throttle : FieldExtensionBase, IGaugeFieldComponent
    {
        public Throttle(string gameName) : base(gameName) { }
        public string Description { get => "Throttle position"; }
        public IGaugeField Data { get; set; } = new GaugeField()
        {
            Name = "Throttle",
            IsDecimalNumber = true,
            Decimal = 0,
            Unit = "%",
            Color = new ColorScheme("#ffffff", "#000000"),
            IsRangeLocked = true,
            Maximum = 100.ToString(),
            Minimum = 0.ToString()
        };
        IDataField IDataFieldComponent.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.Throttle < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.Throttle.ToString($"N{Data.Decimal}");
        }
    }
}
