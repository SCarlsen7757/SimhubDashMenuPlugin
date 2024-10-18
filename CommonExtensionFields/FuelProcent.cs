using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class FuelProcent : FieldExtensionBase<IGaugeField>, IGaugeFieldExtension
    {
        public FuelProcent(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "FUEL",
                IsDecimalNumber = true,
                Decimal = 0,
                Unit = "%",
                Color = new ColorScheme(),
                IsRangeLocked = true,
                Maximum = 100.ToString(),
                Minimum = 0.ToString()
            };
        }

        public string Description => "Fuel in procent.";
        IDataField IDataFieldExtension.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.FuelPercent);
        }
    }
}
