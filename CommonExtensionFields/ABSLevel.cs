using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class ABSLevel : FieldExtensionBase, IDataFieldComponent
    {
        public ABSLevel(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "ABS",
                Color = new ColorScheme("#00ff2a", "#ffffff")
            };
        }

        public string Description { get => "ABS Level"; }

        IDataField IDataFieldComponent.Data { get => Data; set => Data = value; }

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
