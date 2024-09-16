using DashMenu.Data;
using GameReaderCommon;

namespace DashMenu
{
    internal class EmptyGaugeField : FieldExtensionBase, IGaugeFieldComponent
    {
        public EmptyGaugeField(string gameName) : base(gameName) { }
        public string Description { get; } = string.Empty;
        public IGaugeField Data { get; set; } = new GaugeField()
        {
            Name = string.Empty,
            Value = string.Empty,
            Color = new ColorScheme("#ffffff", "#808080"),
            IsRangeLocked = true,
            IsStepLocked = true,
            Minimum = 0.ToString(),
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
