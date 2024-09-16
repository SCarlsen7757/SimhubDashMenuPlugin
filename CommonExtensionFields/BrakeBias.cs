using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class BrakeBias : FieldExtensionBase, IDataFieldComponent
    {
        public BrakeBias(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "BB",
                IsDecimalNumber = true,
                Decimal = 1,
                Color = new ColorScheme("#d90028", "#808080")
            };
        }

        public string Description { get => "Brake bias"; }

        IDataField IDataFieldComponent.Data
        {
            get => Data;
            set => Data = value;
        }

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.BrakeBias < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.BrakeBias.ToString($"N{Data.Decimal}");
        }
    }
}
