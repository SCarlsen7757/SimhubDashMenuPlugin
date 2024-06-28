using DashMenu.Data;
using GameReaderCommon;

namespace DashMenu
{
    internal class EmptyField : IFieldDataComponent
    {
        public string Description { get; } = "";
        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "",
            Value = "",
            Color = new ColorScheme("#ffffff", "#ffffff")
        };

        public bool GameSupported(string name)
        {
            return true;
        }

        public void Update(ref GameData data)
        {
            return;
        }
    }
}
