using DashMenu.Data;
using GameReaderCommon;

namespace DashMenu
{
    internal class EmptyGaugeField : FieldExtensionBase, IGaugeFieldComponent
    {
        public EmptyGaugeField(string gameName) : base(gameName) { }
        public string Description { get; } = string.Empty;
        new public IGaugeField Data { get; set; } = new GaugeField()
        {
            IsRangeLocked = true,
            IsStepLocked = true,
            Maximum = 0.ToString(),
        };
        IDataField IDataFieldComponent.Data
        {
            get => Data;
            set => Data = (IGaugeField)value;
        }
        public void Update(ref GameData data)
        {
            return;
        }

        private readonly static EmptyGaugeField @field = new EmptyGaugeField("");
        /// <summary>
        /// Empty field
        /// </summary>
        public static EmptyGaugeField Field => @field;
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
