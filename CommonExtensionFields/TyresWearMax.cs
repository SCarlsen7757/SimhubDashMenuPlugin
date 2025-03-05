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
                Color = new ColorScheme("#00ed96")
            };
        }

        public string Description => "Max tire wear for all tires.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = data.NewData.TyresWearMax.ToString();
        }
    }
}
