using GameReaderCommon;

namespace DashMenu
{
    public class EmptyField : IFieldData
    {
        public string Description { get; } = "";
        public FieldData Data { get; } = new FieldData() { Name = "", Value = "", Color = "#000000" };

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
