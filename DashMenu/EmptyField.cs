using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace DashMenu
{
    internal class EmptyField : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension
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

        private readonly static EmptyField @field = new EmptyField("");

        /// <summary>
        /// Empty field
        /// </summary>
        public static EmptyField Field => @field;
        private static string fullname = null;
        public static string FullName
        {
            get
            {
                if (fullname == null)
                {
                    fullname = Field.GetType().FullName;
                }
                return fullname;
            }
        }

    }
}
