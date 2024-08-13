using GameReaderCommon;

namespace DashMenu.Data
{
    public interface IFieldDataComponent
    {
        /// <summary>
        /// Description of the field.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Field data.
        /// </summary>
        FieldData Data { get; set; }
        /// <summary>
        /// Update field.
        /// </summary>
        /// <param name="data"></param>
        void Update(ref GameData data);

        /// <summary>
        /// Does the field support this game? With this field break or make expections if this fields is run with the game?
        /// </summary>
        /// <returns>Game support.</returns>
        bool IsGameSupported { get; }
        /// <summary>
        /// Describe what games this field supportes.
        /// </summary>
        string SupportedGames { get; }
    }
}