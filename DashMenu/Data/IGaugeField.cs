namespace DashMenu.Data
{
    public interface IGaugeField : IDataField
    {
        string Maximum { get; set; }
        string Minimum { get; set; }
        string Step { get; set; }
    }
}
