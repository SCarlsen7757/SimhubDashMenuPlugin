using DashMenu.Data;
using DashMenu.Data.Interfaces;
using GameReaderCommon;
using SimHub.Plugins;

namespace DashMenu
{
    internal sealed class EmptyField : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
    {
        public EmptyField(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                IsRangeLocked = true,
                IsStepLocked = true,
                Maximum = 0.ToString(),
            };
        }

        public string Description { get; } = string.Empty;

        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            return;
        }

        private static readonly EmptyField instance = new EmptyField(string.Empty);

        /// <summary>
        /// Empty field
        /// </summary>
        public static EmptyField Field => instance;

        private static string fullName;

        public static string FullName => fullName ?? (fullName = Field.GetType().FullName);
    }
}
