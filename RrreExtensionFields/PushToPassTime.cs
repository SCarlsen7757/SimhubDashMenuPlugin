using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace RrreExtensionFields
{
    internal class PushToPassTime : FieldExtensionBase<IGaugeField>, IGaugeFieldExtension
    {
        public PushToPassTime(string gameName) : base(gameName, "RRRE")
        {
            Data = new GaugeField()
            {
                Name = "P2P",
                Color = new ColorScheme("#ba42ff"),
                IsDecimalNumber = true,
                Decimal = 1,
                IsRangeLocked = true,
                Minimum = 0.ToString(),
                Unit = "s"
            };
        }
        public string Description => "Push to pass delay/wait time.";

        protected override bool GameSupported(string gameName)
        {
            return gameName == "RRRE";
        }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            var r3eGameData = (R3E.Data.Shared)data.NewData.GetRawDataObject();
            var push2pass = r3eGameData.PushToPass;

            if (push2pass.Available != 1)
            {
                Data.Maximum = 0.ToString();
                Data.Value = 0.ToString();
            }
            else if (push2pass.Engaged == 1)
            {
                Data.Maximum = 10.ToString();
                Data.Value = DecimalValue(push2pass.EngagedTimeLeft);
            }
            else
            {
                Data.Maximum = 20.ToString();
                Data.Value = DecimalValue(push2pass.WaitTimeLeft);
            }
        }
    }
}
