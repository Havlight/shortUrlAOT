namespace shortUrlAOT;

public class Db
{
    public Dictionary<string, List<UrlMap>> Users { get; set; } = new Dictionary<string, List<UrlMap>>();
}