using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace RrreExtensionFields
{
    internal class Battery : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public Battery(string gameName) : base(gameName, "RRRE")
        {
            Data = new GaugeField()
            {
                Name = "SOC",
                Color = new ColorScheme("#67ff3d"),
                IsDecimalNumber = true,
                IsRangeLocked = true,
                Maximum = 100.ToString(),
                Minimum = 0.ToString()
            };
        }

        public string Description => "Battery level.";

        protected override bool GameSupported(string gameName)
        {
            return gameName == "RRRE";
        }

        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            var r3eGameData = (R3E.Data.Shared)data.NewData.GetRawDataObject();
            float soc = r3eGameData.BatterySoC;
            if (soc < 0)
            {
                Data.Value = "-";
            }
            else
            {
                Data.Value = r3eGameData.BatterySoC.ToString();
            }
        }
    }
}
