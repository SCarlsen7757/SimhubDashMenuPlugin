using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace MediaExtensionFields
{
    class Title : AlertBase, IDataFieldExtension, IAlert
    {
        public Title(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TITLE",
                Color = new ColorScheme("#2a6901")
            };
            Data.PropertyChanged += DataAlert_PropertyChanged;
        }

        public string Description => "Media Title.";

        public override void Update(PluginManager pluginManager, ref GameData data)
        {
            var title = pluginManager.GetPropertyValue("MediaInfo.Title");
            Data.Value = title != null ? title.ToString() : string.Empty;
        }
    }
}
