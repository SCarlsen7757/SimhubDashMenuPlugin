using DashMenu.Data;
using DashMenu.Data.Interfaces;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields.Tyres
{
    public sealed class TyresWearMin : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public TyresWearMin(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TWMIN",
                Unit = "%",
                IsDecimalNumber = true,
                Decimal = 1,
                Color = new ColorScheme("#00ed96")
            };
        }

        public string Description => "Min tire wear for all tires.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.TyresWearMin);
        }
    }
}
