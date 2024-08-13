using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    public class ABSLevel : ExtensionDataBase, IFieldDataComponent
    {
        public ABSLevel(string gameName) : base(gameName) { }
        public string Description { get => "ABS Level."; }
        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "ABS",
            Color = new ColorScheme("#00ff2a", "#ffffff")
        };
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
