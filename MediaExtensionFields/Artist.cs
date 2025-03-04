using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace MediaExtensionFields
{
    public class Artist : AlertBase, IDataFieldExtension, IAlert
    {
        public Artist(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "ARTIST",
                Color = new ColorScheme("#2a6901")
            };
            Data.PropertyChanged += DataAlert_PropertyChanged;
        }

        public string Description => "Media Artist.";

        public override void Update(PluginManager pluginManager, ref GameData data)
        {
            var artist = pluginManager.GetPropertyValue("MediaInfo.Artist");
            Data.Value = artist != null ? artist.ToString() : string.Empty;
        }
    }
}
