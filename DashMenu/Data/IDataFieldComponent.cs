namespace DashMenu.Data
{
    public interface IDataFieldComponent : IFieldExtensionBasic
    {
        /// <summary>
        /// Field data.
        /// </summary>
        IDataField Data { get; set; }
    }
}