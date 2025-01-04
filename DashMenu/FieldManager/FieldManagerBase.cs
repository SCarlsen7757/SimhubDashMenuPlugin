using SimHub.Plugins;
using System;
using System.Collections.Generic;

namespace DashMenu.FieldManager
{
    internal abstract class FieldManagerBase
    {
        public FieldManagerBase(PluginManager pluginManager, Type pluginType, IList<string> fieldOrder, string fieldTypeName)
        {
            gameName = PluginManager.GetInstance().GameName;

            this.pluginManager = pluginManager;
            this.pluginType = pluginType;
            this.fieldOrder = fieldOrder;

            amountOfFieldName = $"AmountOf{fieldTypeName}Fields";
        }

        protected readonly PluginManager pluginManager;
        protected readonly Type pluginType;
        protected readonly string gameName;
        /// <summary>
        /// Ref to Field settings order.
        /// </summary>
        protected readonly IList<string> fieldOrder;
        private readonly string amountOfFieldName;
        protected string AmountOfFieldName { get => amountOfFieldName; }

        internal delegate void SelectedFieldsChangedEventHandler(IList<string> selectedFields);
    }
}