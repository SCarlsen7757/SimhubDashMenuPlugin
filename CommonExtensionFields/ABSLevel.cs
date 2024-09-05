using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class ABSLevel : FieldExtensionBase, IFieldDataComponent
    {
        public ABSLevel(string gameName) : base(gameName) { }
        public string Description { get => "ABS Level."; }
        public IDataField Data { get; set; } = new DataField()
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
