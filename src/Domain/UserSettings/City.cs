namespace TP.Domain.UserSettings;

public class City
{
    public short Id { get; set; }
    public string Name { get; set; }
    public short ProvinceId { get; set; }
    public Province Province { get; set; }
}
