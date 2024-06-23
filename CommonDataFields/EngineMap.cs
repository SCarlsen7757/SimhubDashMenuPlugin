using DashMenu;
using GameReaderCommon;

namespace CommonDataFields
{
    public class EngineMap : IFieldData
    {
        public string Description { get => "Engine map"; }
        public FieldData Data { get; private set; } = new FieldData() { Name = "MAP", Value = "-", Color = "#d9c000" };

        public bool GameSupported(string game) => true;

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.EngineMap < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.EngineMap.ToString();
        }
    }
}
