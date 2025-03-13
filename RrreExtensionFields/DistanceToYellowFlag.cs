using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace RrreExtensionFields
{
    class DistanceToYellowFlag : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public DistanceToYellowFlag(string gameName) : base(gameName, "RRRE")
        {
            Data = new GaugeField()
            {
                Name = "D2Y",
                Color = new ColorScheme("#fcba03"),
                IsDecimalNumber = true,
                Decimal = 0,
                IsRangeLocked = false,
                Maximum = 50.ToString(),
                Minimum = 0.ToString(),
                Unit = "m"
            };
        }

        public string Description => "Distance to yellow flag.";

        protected override bool GameSupported(string gameName)
        {
            return gameName == "RRRE";
        }

        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            var r3eGameData = (R3E.Data.Shared)data.NewData.GetRawDataObject();
            var distanceToYellow = r3eGameData.Flags.ClosestYellowDistanceIntoTrack;
            if (distanceToYellow < 0)
            {
                Data.Value = "-";
            }
            else
            {
                Data.Value = DecimalValue(distanceToYellow);
            }
        }
    }
}
