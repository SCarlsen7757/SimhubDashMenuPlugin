using DashMenu.Data;
using DashMenu.Data.Interfaces;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields.Electronics
{
    public sealed class EngineMap : AlertBase, IDataFieldExtension
    {
        public EngineMap(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "MAP",
                Color = new ColorScheme("#d9c000")
            };
            Data.PropertyChanged += DataAlert_PropertyChanged;
        }

        public string Description => "Engine map.";

        public override void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.EngineMap < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.EngineMap.ToString();
        }
    }
}
