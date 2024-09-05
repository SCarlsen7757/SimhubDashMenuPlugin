namespace DashMenu.Data
{
    public interface IFieldDataComponent : IFieldExtensionBasic
    {
        /// <summary>
        /// Field data.
        /// </summary>
        IFieldData Data { get; set; }
    }
}