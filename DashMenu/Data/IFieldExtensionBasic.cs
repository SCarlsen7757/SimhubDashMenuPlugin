using GameReaderCommon;
using SimHub.Plugins;

namespace DashMenu.Data
{
    public interface IFieldExtensionBasic<TData> : IDashMenuPluginExtension
        where TData : IDataField
    {
        /// <summary>
        /// Description of the field.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Does the field support this game? With this field break or make expections if this fields is run with the game?
        /// </summary>
        /// <returns>Game support.</returns>
        bool IsGameSupported { get; }
        /// <summary>
        /// Describe what games this field supportes.
        /// </summary>
        string SupportedGames { get; }
        /// <summary>
        /// Update field.
        /// </summary>
        /// <param name="data"></param>
        void Update(PluginManager pluginManager, ref GameData data);
        /// <summary>
        /// Field data.
        /// </summary>
        TData Data { get; set; }
    }
}