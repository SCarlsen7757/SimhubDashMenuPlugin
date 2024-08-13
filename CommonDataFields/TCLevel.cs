using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    public class TCLevel : ExtensionDataBase, IFieldDataComponent
    {
        public TCLevel(string gameName) : base(gameName) { }
        public string Description { get => "TC Level."; }
        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "TC",
            Color = new ColorScheme("#00a3d9", "#ffffff")
        };
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
