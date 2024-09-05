using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{

    public class OilPressure : FieldExtensionBase, IFieldDataComponent
    {
        public OilPressure(string gameName) : base(gameName) { }
        public string Description { get => "Oil pressure"; }
        public IDataField Data { get; set; } = new DataField()
        {
            Name = "Oil Press",
            IsDecimalNumber = true,
            Decimal = 0,
            Color = new ColorScheme("#ffffff", "ffffff")
        };
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
