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
        }

        public IDataField Data;
        public bool IsGameSupported => true;

        public string SupportedGames => "All games.";

        protected string DecimalValue(double value)
        {
            return value.ToString($"N{Data.Decimal}");
        }

    }
}
