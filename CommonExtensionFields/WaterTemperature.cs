using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class WaterTemperature : FieldExtensionBase, IFieldDataComponent
    {
        public WaterTemperature(string gameName) : base(gameName) { }
        public string Description { get => "Water temperature"; }
        public IDataField Data { get; set; } = new DataField()
        {
            Name = "Water Temp",
            IsDecimalNumber = true,
            Decimal = 0,
            Color = new ColorScheme("#ffffff", "#ffffff")
        };
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
