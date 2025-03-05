using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    class TyresWearMin : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public TyresWearMin(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TWMIN",
                Unit = "%",
                Color = new ColorScheme("#00ed96")
            };
        }

        public string Description => "Min tire wear for all tires.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = data.NewData.TyresWearMin.ToString();
        }
    }
}
