using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    public class BrakeBias : IFieldDataComponent
    {
        public string Description { get => "Brake bias."; }

        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "BB",
            Color = new ColorScheme("#d90028", "#ffffff")
        };

        public bool GameSupported(string game) => true;
        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.BrakeBias < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.BrakeBias.ToString();
        }
    }
}
