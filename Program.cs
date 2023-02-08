using MyGameBackend;

var builder = WebApplication.CreateBuilder(args);

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:3987",
                "http://localhost",
                "http://192.168.0.113",
                "http://79.136.237.76:3333",
                "http://79.136.237.76:7070")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
builder.Services.AddSingleton<GameBroadcaster>();
builder.Services.AddSignalR();

var app = builder.Build();

//app.UseRouting();

app.UseCors(myAllowSpecificOrigins);

//app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapHub<GameHub>("/game");

app.Run();