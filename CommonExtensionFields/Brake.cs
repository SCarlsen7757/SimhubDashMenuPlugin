using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class Brake : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public Brake(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "BRK",
                IsDecimalNumber = true,
                Decimal = 0,
                Unit = "%",
                Color = new ColorScheme(),
                IsRangeLocked = true,
                Maximum = 100.ToString(),
                Minimum = 0.ToString()
            };
        }
        public string Description => "Brake position.";

        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.Brake);
        }
    }
}
