using DashMenu;
using GameReaderCommon;

namespace CommonDataFields
{
    public class WaterTemperature : IFieldData
    {
        public string Description { get => "Water temperature"; }
        public FieldData Data { get; } = new FieldData()
        {
            Name = "Water Temp",
            Color = new FieldData.ColorScheme("#ffffff", "#ffffff")
        };

        public bool GameSupported(string game) => true;

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
