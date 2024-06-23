using DashMenu;
using GameReaderCommon;

namespace CommonDataFields
{
    public class OilPressure : IFieldData
    {
        public string Description { get => "Oil pressure"; }
        public FieldData Data { get; } = new FieldData()
        {
            Name = "Oil Press",
            Decimal = 0,
            Color = new FieldData.ColorScheme("#ffffff", "ffffff")
        };

        public bool GameSupported(string game) => true;

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.OilPressure <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.OilPressure.ToString($"N{Data.Decimal}");
            Data.Unit = data.NewData.OilPressureUnit;
        }
    }
}
