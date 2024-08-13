using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    public class EngineMap : ExtensionDataBase, IFieldDataComponent
    {
        public EngineMap(string gameName) : base(gameName) { }
        public string Description { get => "Engine map"; }
        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "MAP",
            Color = new ColorScheme("#d9c000", "#ffffff")
        };
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
