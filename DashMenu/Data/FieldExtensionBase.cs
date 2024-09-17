using System;

namespace DashMenu.Data
{
    public abstract class FieldExtensionBase
    {
        protected readonly string gameName = string.Empty;

        // Default constructor throws an exception, enforcing the other constructor to be used.
        public FieldExtensionBase()
        {
            throw new NotImplementedException("Use the other constructor. The game name is required.");
        }
        protected FieldExtensionBase(string gameName)
        {
            this.gameName = gameName ?? throw new ArgumentNullException(nameof(gameName));
            isGameSupported = GameSupported(gameName);
        }

        public virtual IDataField Data { get; set; }

        protected virtual bool GameSupported(string gameName) => true;

        private readonly bool isGameSupported = true;
        public bool IsGameSupported => isGameSupported;

        protected readonly string supportedGames = "All games.";

        public string SupportedGames => supportedGames;

        protected string DecimalValue(double value)
        {
            return value.ToString($"F{Data.Decimal}");
        }

    }
}
