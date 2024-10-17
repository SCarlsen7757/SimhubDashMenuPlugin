using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{

    public class OilPressure : FieldExtensionBase, IGaugeFieldExtension
    {
        public OilPressure(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "EOP",
                IsDecimalNumber = true,
                Decimal = 0,
                Color = new ColorScheme(),
                Maximum = 150.ToString(),
                Minimum = 50.ToString()
            };
        }
        public string Description => "Oil pressure. EOP stands for Engine oil pressure.";

        private IGaugeField data;
        new IGaugeField Data
        {
            get => data;
            set
            {
                data = value;
                base.Data = value; //Make sure to set base Data
            }
        }

        IDataField IDataFieldExtension.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }

        IGaugeField IGaugeFieldExtension.Data
        {
            get => Data;
            set => Data = value;
        }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.OilPressure <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = DecimalValue(data.NewData.OilPressure);
            Data.Unit = data.NewData.OilPressureUnit;
        }
    }
}
