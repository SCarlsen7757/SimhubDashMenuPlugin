using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class WaterTemperature : FieldExtensionBase, IGaugeFieldComponent
    {
        public WaterTemperature(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "WT",
                IsDecimalNumber = true,
                Decimal = 0,
                Color = new ColorScheme(),
                Maximum = 100.ToString(),
                Minimum = 20.ToString()
            };
        }

        public string Description => "Water temperature.";

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

        IDataField IDataFieldComponent.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }

        IGaugeField IGaugeFieldComponent.Data
        {
            get => Data;
            set => Data = value;
        }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.WaterTemperature <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = DecimalValue(data.NewData.WaterTemperature);
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
