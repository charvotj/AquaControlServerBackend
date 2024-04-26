namespace AquaControlServerBackend.Helpers;

public class AppSettings
{
    public string Secret { get; set; }
    public int Port { get; set; }
    public string Hostname { get; set; }
    public string Scheme { get; set; }
}