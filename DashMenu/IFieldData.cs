using GameReaderCommon;

namespace DashMenu
{
    public interface IFieldData
    {
        /// <summary>
        /// Description of the field.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Field data.
        /// </summary>
        FieldData Data { get; }
        /// <summary>
        /// Update field.
        /// </summary>
        /// <param name="data"></param>
        void Update(ref GameData data);

        /// <summary>
        /// Does the field support this game? With this field break or make expections if this fields is run with the game?
        /// </summary>
        /// <param name="game">Name of the game.</param>
        /// <returns>Game support.</returns>
        bool GameSupported(string game);
    }
}
