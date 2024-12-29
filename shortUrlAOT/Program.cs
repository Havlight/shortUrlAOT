using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using shortUrlAOT;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<Db>();
builder.Services.AddHostedService<DbBackgroundService>();
builder.Services.AddScoped<IUserService, UserSerivce>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin() // 允許所有來源
                .AllowAnyHeader() // 允許任何標頭
                .AllowAnyMethod(); // 允許任何方法（GET, POST, DELETE等）
        });
});

var app = builder.Build();
app.UseCors("AllowAll");

app.MapGet("/", () =>
{
    
    string[] tutorialLines = new string[]
    {
        "使用教學",
        "users節點可對使用者增刪改查",
        "users/ 返回所有使用者名稱",
        "users/add/{你的名稱} 創建名稱為 你的名稱 的user",
        "user/del/{name} 刪除 user",
        "map節點提供短網址服務",
        "map/list/{name} 返回name下的網址對",
        "map/set/{name}?surl=sss&lurl=https://aot.dapperlib.dev/gettingstarted.html 設定name下的網址映射 sss 到 長網址",
        "map/re/{name}/{surl} 重導向網址到surl對應的網址",
        "map/del/{name}/?surl={surl} 刪除 {surl}"
    };

    return Results.Ok(tutorialLines);
});

var userGroup = app.MapGroup("users");
userGroup.MapGet("/", (IUserService userService) =>
{
    var users = userService?.GetUsers();
    if (users != null && users.Any())
    {
        return Results.Ok(users);
    }

    return Results.NotFound("No users found");
});
userGroup.MapGet("/check/{name}/",
    (string name, IUserService userService) =>
    {
        return userService.CheckUser(name) ? Results.Ok("live") : Results.NotFound("user not found");
    });
userGroup.MapGet("/add/{name}",
    (string name, IUserService userService) =>
    {
        return userService.AddUser(name) ? Results.Ok($"added {name}") : Results.BadRequest("duplicate name");
    });
userGroup.MapGet("/del/{name}",
    (string name, IUserService userService) =>
    {
        return userService.DeleteUser(name) ? Results.Ok($"delete {name}") : Results.NotFound("user not found");
    });
var mapGroup = app.MapGroup("map");
mapGroup.MapGet("list/{username}",
    (string username, IUserService userService) => { return userService.GetUrlMaps(username); });
mapGroup.MapGet("set/{username}",
    (string username, string surl, string lurl, IUserService userService) =>
    {
        return userService.MapUrl(username, surl, lurl) ? Results.Ok("map success") : Results.BadRequest("error");
    });
mapGroup.MapGet("/re/{username}/{surl}", (string username, string surl, IUserService userService) =>
{
    string lurl = userService.GetLongtUrl(username, surl);
    return Results.Redirect(lurl);
});
mapGroup.MapGet("/del/{username}",
    (string username, string surl, IUserService userService) =>
    {
        return userService.DeleteShortUrl(username, surl) ? Results.Ok($"delete {surl}") : Results.BadRequest("error");
    });

app.Run();

[JsonSerializable(typeof(Dictionary<string, List<UrlMap>>))]
[JsonSerializable(typeof(IEnumerable<UrlMap>))]
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(string[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}