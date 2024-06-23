using DashMenu;
using GameReaderCommon;

namespace CommonDataFields
{
    public class WaterTemperature : IFieldData
    {
        public string Description { get => "Water temperature"; }
        public FieldData Data { get; } = new FieldData() { Name = "Water", Value = "-", Color = "#ffffff" };

        public bool GameSupported(string game) => true;

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.WaterTemperature <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.WaterTemperature.ToString() + "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
