namespace DashMenu.Data
{
    public interface IDataFieldExtension : IFieldExtensionBasic
    {
        /// <summary>
        /// Field data.
        /// </summary>
        IDataField Data { get; set; }
    }
}