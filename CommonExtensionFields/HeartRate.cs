using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class HeartRate : FieldExtensionBase, IGaugeFieldExtension
    {
        public HeartRate(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "HR",
                Color = new ColorScheme("#c51104"),
                IsRangeLocked = true,
                Maximum = 210.ToString(),
                Minimum = 60.ToString()
            };
        }

        public string Description => "Heart rate.";

        private IGaugeField data;
        new IGaugeField Data
        {
            get => data;
            set
            {
                data = value;
                base.Data = value;
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
            set => Data = value; //Make sure to set base Data
        }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            var heartRate = pluginManager.GetPropertyValue("DataCorePlugin.HeartRateMonitorLastBPM");
            Data.Value = heartRate != null ? heartRate.ToString() : "-";
        }
    }
}
