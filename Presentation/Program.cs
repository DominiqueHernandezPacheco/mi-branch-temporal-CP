using Application.Core;
using Presentacion;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Project", "Sistema Default")
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    // .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
// Agrega los servicios configurados en los métodos de extensión al contenedor de IoC.

builder.Services.AddPresentationServices(builder.Configuration);
builder.Services.AddAplicationCore();
builder.Services.AddCors();
builder.Services.AddInfraestructure(builder.Configuration);
builder.Services.AddSecurity(builder.Configuration);

/*builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
    options.HttpsPort = 5000;
});*/

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwaggerUI(options =>
    {
        app.UseSwagger();
        options.DisplayRequestDuration();
        options.DefaultModelsExpandDepth(-1);
    });
    
    app.UseReDoc(options =>
    {
        options.RoutePrefix = "docs";
        options.DocumentTitle = "Sistema Default API";
        options.HideDownloadButton();
        
    });
}

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();

app.Run();