using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    class TyresWearAvg : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public TyresWearAvg(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TWAVG",
                Unit = "%",
                Color = new ColorScheme("#00ed96")
            };
        }

        public string Description => "Avg tire wear for all tires.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = data.NewData.TyresWearAvg.ToString();
        }
    }
}
