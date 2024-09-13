using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class TCLevel : FieldExtensionBase, IDataFieldComponent
    {
        public TCLevel(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TC",
                Color = new ColorScheme("#00a3d9", "#ffffff")
            };
        }

        public string Description { get => "TC Level."; }
        IDataField IDataFieldComponent.Data
        {
            get => Data;
            set => Data = value;
        }

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
