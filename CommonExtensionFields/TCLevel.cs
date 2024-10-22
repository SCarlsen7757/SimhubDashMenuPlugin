using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class TCLevel : AlertBase, IDataFieldExtension
    {
        public TCLevel(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TC",
                Color = new ColorScheme("#00a3d9")
            };
            Data.PropertyChanged += DataAlert_PropertyChanged;
        }

        public string Description => "TC Level.";

        public override void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.TCLevel < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.TCLevel.ToString();
        }
    }
}
