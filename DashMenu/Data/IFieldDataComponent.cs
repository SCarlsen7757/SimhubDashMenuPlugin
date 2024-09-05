namespace DashMenu.Data
{
    public interface IFieldDataComponent : IFieldExtensionBasic
    {
        /// <summary>
        /// Field data.
        /// </summary>
        IDataField Data { get; set; }
    }
}