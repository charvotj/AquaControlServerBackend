namespace AquaControlServerBackend.Helpers;

public class AppSettings
{
    public string Secret { get; set; }
    public int Port { get; set; }
    public int Hostname { get; set; }
    public int Scheme { get; set; }
}