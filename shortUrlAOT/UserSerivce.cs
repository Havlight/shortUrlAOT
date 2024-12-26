namespace shortUrlAOT;

public interface IUserService
{
    IEnumerable<string> GetUsers();
    bool CheckUser(string username);
    bool AddUser(string username);
    bool DeleteUser(string username);
    IEnumerable<UrlMap> GetUrlMaps(string username);
    string GetLongtUrl(string username, string shortUrl);
    bool MapUrl(string username, string shortUrl, string longUrl);
    bool DeleteShortUrl(string username, string shortUrl);
}

public class UserSerivce : IUserService
{
    private readonly Db _dbContext;

    public UserSerivce(Db dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<string> GetUsers()
    {
        return _dbContext.Users.Keys.ToList()?? new List<string>();
    }

    public bool CheckUser(string username)
    {
        return _dbContext.Users.ContainsKey(username);
    }

    public bool AddUser(string username)
    {
        if (_dbContext.Users.ContainsKey(username))
        {
            return false;
        }

        _dbContext.Users[username] = new List<UrlMap>();

        return true;
    }

    public bool DeleteUser(string username)
    {
        if (_dbContext.Users.ContainsKey(username))
        {
            _dbContext.Users.Remove(username);
            return true;
        }

        return false;
    }

    public IEnumerable<UrlMap> GetUrlMaps(string username)
    {
        if (_dbContext.Users.ContainsKey(username))
        {
            return _dbContext.Users[username];
        }

        throw new KeyNotFoundException();
    }

    public string GetLongtUrl(string username, string shortUrl)
    {
        if (_dbContext.Users.ContainsKey(username))
        {
            if (_dbContext.Users[username].Any(u => u.ShortUrl == shortUrl))
            {
                return _dbContext.Users[username].First(u => u.ShortUrl == shortUrl).LongUrl;
            }

            throw new KeyNotFoundException("ShortUrl not found");
        }

        throw new KeyNotFoundException("User not found");
    }

    public bool MapUrl(string username, string shortUrl, string longUrl)
    {
        if (_dbContext.Users.ContainsKey(username))
        {
            var userUrlMaps = _dbContext.Users[username];
            var urlMap = userUrlMaps.FirstOrDefault(um => um.ShortUrl == shortUrl);

            if (urlMap is not null)
            {
                // 若找到，則修改其 long url
                _dbContext.Users[username].Remove(urlMap);
                _dbContext.Users[username].Add(new UrlMap(shortUrl, longUrl));
            }
            else
            {
                // 若沒找到，則新增 UrlMap 到 list
                userUrlMaps.Add(new UrlMap
                (
                    shortUrl,
                    longUrl
                ));
            }

            return true;
        }

        throw new KeyNotFoundException("User not found");
    }

    public bool DeleteShortUrl(string username, string shortUrl)
    {
        if (_dbContext.Users.ContainsKey(username))
        {
            var userUrlMaps = _dbContext.Users[username];
            var urlMap = userUrlMaps.FirstOrDefault(um => um.ShortUrl == shortUrl);

            if (urlMap is not null)
            {
                userUrlMaps.Remove(urlMap);
                return true; // 成功刪除
            }

            return false; // 沒有找到指定的短網址
        }

        throw new KeyNotFoundException("User not found");
    }
}