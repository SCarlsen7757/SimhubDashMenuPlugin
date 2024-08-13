using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    public class OilTemperature : ExtensionDataBase, IFieldDataComponent
    {
        public OilTemperature(string gameName) : base(gameName) { }
        public string Description { get => "Oil temperature"; }
        public FieldData Data { get; set; } = new FieldData()
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
