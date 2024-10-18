using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class Rpm : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public Rpm(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "RPM",
                IsDecimalNumber = false,
                Color = new ColorScheme(),
                IsRangeLocked = true,
                Minimum = 0.ToString()
            };
        }

        public string Description => "Engine RPM.";

        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Maximum = data.NewData.CarSettings.MaxRpm.ToString();
            Data.Value = DecimalValue(data.NewData.Rpms);
        }
    }
}
