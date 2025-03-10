﻿using System;
using System.ComponentModel;

namespace DashMenu.Data
{
    public abstract class FieldExtensionBase<TField>
        where TField : IDataField, INotifyPropertyChanged
    {
        protected readonly string gameName = string.Empty;

        protected FieldExtensionBase(string gameName)
        {
            this.gameName = gameName ?? throw new ArgumentNullException(nameof(gameName));
            isGameSupported = GameSupported(gameName);
        }

        protected FieldExtensionBase(string gameName, string supportedGames) : this(gameName)
        {
            this.supportedGames = supportedGames ?? throw new ArgumentNullException(nameof(supportedGames));
        }

        public TField Data { get; set; }

        protected virtual bool GameSupported(string gameName) => true;

        private readonly bool isGameSupported = true;
        public bool IsGameSupported => isGameSupported;

        private readonly string supportedGames = "All games.";

        public string SupportedGames => supportedGames;

        protected string DecimalValue(double value)
        {
            return value.ToString($"F{Data.Decimal}");
        }

    }
}
