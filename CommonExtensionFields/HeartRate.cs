using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class HeartRate : FieldExtensionBase<IGaugeField>, IGaugeFieldExtension
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
        IDataField IDataFieldExtension.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            var heartRate = pluginManager.GetPropertyValue("DataCorePlugin.HeartRateMonitorLastBPM");
            Data.Value = heartRate != null ? heartRate.ToString() : "-";
        }
    }
}
