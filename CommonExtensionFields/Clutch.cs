using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class Clutch : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public Clutch(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "CPP",
                IsDecimalNumber = true,
                Decimal = 0,
                Unit = "%",
                Color = new ColorScheme(),
                IsRangeLocked = true,
                Maximum = 100.ToString(),
                Minimum = 0.ToString()
            };
        }

        public string Description => "Clutch position. CPP stands for Clutch Pedal Position.";
        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.Clutch);
        }
    }
}
