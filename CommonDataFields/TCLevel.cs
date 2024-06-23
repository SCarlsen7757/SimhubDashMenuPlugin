using DashMenu;
using GameReaderCommon;

namespace CommonDataFields
{
    public class TCLevel : IFieldData
    {
        public string Description { get => "TC Level."; }

        public FieldData Data { get; private set; } = new FieldData() { Name = "TC", Value = "-", Color = "#00a3d9" };

        public bool GameSupported(string game) => true;

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.TCLevel < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.TCLevel.ToString();
        }
    }
}
