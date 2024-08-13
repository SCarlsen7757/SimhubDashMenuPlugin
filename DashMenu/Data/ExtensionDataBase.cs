using System;

namespace DashMenu.Data
{
    public abstract class ExtensionDataBase
    {
        protected readonly string gameName = string.Empty;

        protected ExtensionDataBase(string gameName)
        {
            this.gameName = gameName ?? throw new ArgumentNullException(nameof(gameName));
        }

        public bool IsGameSupported => true;

        public string SupportedGames => "All games.";
    }
}
