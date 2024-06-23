using DashMenu;
using GameReaderCommon;

namespace CommonDataFields
{
    public class ABSLevel : IFieldData
    {
        public string Description { get => "ABS Level."; }

        public FieldData Data { get; private set; } = new FieldData()
        {
            Name = "ABS",
            Color = new FieldData.ColorScheme("#00ff2a", "#ffffff")
        };

        public bool GameSupported(string name) => true;
        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.ABSLevel < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.ABSLevel.ToString();
        }
    }
}
