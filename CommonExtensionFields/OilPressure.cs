using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{

    public class OilPressure : FieldExtensionBase, IGaugeFieldComponent
    {
        public OilPressure(string gameName) : base(gameName) { }
        public string Description { get => "Oil pressure"; }
        public IGaugeField Data { get; set; } = new GaugeField()
        {
            Name = "Oil Press",
            IsDecimalNumber = true,
            Decimal = 0,
            Color = new ColorScheme("#ffffff", "#000000"),
            Maximum = 150.ToString(),
            Minimum = 50.ToString()
        };
        IDataField IDataFieldComponent.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }
        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.OilPressure <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.OilPressure.ToString($"N{Data.Decimal}");
            Data.Unit = data.NewData.OilPressureUnit;
        }
    }
}
