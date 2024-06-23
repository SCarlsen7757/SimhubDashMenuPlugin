using DashMenu;
using GameReaderCommon;

namespace CommonDataFields
{
    public class OilTemperature : IFieldData
    {
        public string Description { get => "Oil temperature"; }
        public FieldData Data { get; } = new FieldData() { Name = "Oil", Value = "-", Color = "#ffffff" };

        public bool GameSupported(string game) => true;

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.OilTemperature <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.OilTemperature.ToString() + "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
