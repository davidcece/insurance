using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.AddHttpClient("insurance", (provider, client) =>
{
    using var scope = provider.CreateScope();
    var baseUrl = builder.Configuration.GetValue<string>("InsuranceDataBaseUrl");
    client.BaseAddress = new Uri(baseUrl);
}).ConfigurePrimaryHttpMessageHandler((c) => new HttpClientHandler()
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
    {
        if (builder.Environment.IsDevelopment()) return true;
        return sslPolicyErrors == SslPolicyErrors.None;
        //Store a list of valid certificates and check if they contain certhashstring of the incoming cert
    }
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapDefaultControllerRoute();

app.Run();