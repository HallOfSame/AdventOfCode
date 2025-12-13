using System.Net;
using Helpers;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using PuzzleDays;
using PuzzleRunner;
using PuzzleRunner.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.Configure<AoCSettings>(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    // Make sure even large text (inputs) get through
    .AddHubOptions(opt => opt.MaximumReceiveMessageSize = null);
builder.Services.AddMudServices();
builder.Services.AddPuzzleStructure();
builder.Services.AddPuzzlesFromAssembly(typeof(Day01).Assembly);
builder.Services.AddLogging(logging => logging.AddConsole());
builder.Services
    .AddHttpClient<IAoCHttpClient, AoCHttpClient>(client =>
                                                      client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "https://github.com/HallOfSame/AdventOfCode by GitHub user HallOfSame"))
    .ConfigurePrimaryHttpMessageHandler((provider) =>
    {
        var options = provider.GetRequiredService<IOptions<AoCSettings>>();
        var cookies = new CookieContainer();
        cookies.Add(new Uri("https://adventofcode.com"), new Cookie("session", options.Value.SessionId));
        return new HttpClientHandler
        {
            CookieContainer = cookies
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
