using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    public class BrakeBias : ExtensionDataBase, IFieldDataComponent
    {
        public BrakeBias(string gameName) : base(gameName) { }
        public string Description { get => "Brake bias."; }
        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "BB",
            IsDecimalNumber = true,
            Decimal = 1,
            Color = new ColorScheme("#d90028", "#ffffff")
        };
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
