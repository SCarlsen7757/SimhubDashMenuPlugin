namespace DashMenu.Data
{
    public interface IGaugeFieldComponent : IDataFieldComponent
    {
        /// <summary>
        /// Field data.
        /// </summary>
        new IGaugeField Data { get; set; }
    }
}
