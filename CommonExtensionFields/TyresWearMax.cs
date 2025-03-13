using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    class TyresWearMax : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public TyresWearMax(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TWMAX",
                Unit = "%",
                IsDecimalNumber = true,
                Decimal = 1,
                Color = new ColorScheme("#00ed96")
            };
        }

        public string Description => "Max tire wear for all tires.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.TyresWearMax);
        }
    }
}
