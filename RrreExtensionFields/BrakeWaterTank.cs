using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace RrreExtensionFields
{
    public class BrakeWaterTank : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public BrakeWaterTank(string gameName) : base(gameName, "RRRE")
        {
            Data = new GaugeField()
            {
                Name = "BWT",
                Color = new ColorScheme("#1e74c9"),
                IsDecimalNumber = true,
                Decimal = 1,
                IsRangeLocked = true,
                Maximum = 200.ToString(),
                Minimum = 0.ToString(),
                Unit = "L"
            };
        }

        public string Description => "Truck brake cooling water tank.";

        protected override bool GameSupported(string gameName)
        {
            return gameName == "RRRE";
        }

        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            var r3eGameData = (R3E.Data.Shared)data.NewData.GetRawDataObject();
            float waterLeft = r3eGameData.WaterLeft;
            if (waterLeft < 0)
            {
                Data.Value = "-";
            }
            else
            {
                Data.Value = DecimalValue(waterLeft);
            }
        }
    }
}
