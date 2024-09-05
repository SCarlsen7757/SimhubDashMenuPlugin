namespace DashMenu.Data
{
    public interface IFieldGaugeComponent : IFieldExtensionBasic
    {
        /// <summary>
        /// Field data.
        /// </summary>
        IGaugeField Data { get; set; }
    }
}
