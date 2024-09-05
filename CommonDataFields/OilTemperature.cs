using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    public class OilTemperature : FieldExtensionBase, IFieldDataComponent
    {
        public OilTemperature(string gameName) : base(gameName) { }
        public string Description { get => "Oil temperature"; }
        public IDataField Data { get; set; } = new DataField()
        {
            Name = "Oil Temp",
            IsDecimalNumber = true,
            Decimal = 0,
            Color = new ColorScheme("#ffffff", "#ffffff")
        };
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
