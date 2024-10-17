namespace DashMenu.Data
{
    public interface IGaugeFieldExtension : IDataFieldExtension
    {
        /// <summary>
        /// Field data.
        /// </summary>
        new IGaugeField Data { get; set; }
    }
}
