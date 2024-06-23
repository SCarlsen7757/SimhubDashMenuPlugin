using DashMenu;
using GameReaderCommon;

namespace CommonDataFields
{
    public class BrakeBias : IFieldData
    {
        public string Description { get => "Brake bias."; }

        public FieldData Data { get; private set; } = new FieldData() { Name = "BB", Value = "-", Color = "#d90028" };

        public bool GameSupported(string game) => true;
        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.BrakeBias < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.BrakeBias.ToString();
        }
    }
}
