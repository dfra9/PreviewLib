using APIWebPreview.EndPoints;
using APIWebPreview.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(o =>
{
    // http://localhost:5104
    o.ListenLocalhost(5104);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("dev", p =>
        p.WithOrigins("http://localhost:3000", "http://192.168.0.101:3000")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// ambil Storage & LibreOffice dari appsettings.json
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<LibreOfficeOptions>(builder.Configuration.GetSection("LibreOffice"));

// service converter
builder.Services.AddTransient<LibreOfficeConvert>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Comment UseHttpsRedirection karena kita pakai HTTP saja (localhost:5104)
// app.UseHttpsRedirection();

app.UseCors("dev");

var storageBase = builder.Configuration["Storage:BasePath"]!;
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storageBase),
    RequestPath = "/static",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type");
        ctx.Context.Response.Headers.Append("X-Frame-Options", "ALLOWALL");
        ctx.Context.Response.Headers.Remove("X-Frame-Options");
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// endpoint upload
app.MapPost("/files", FileEndpoints.UploadFile)
   .DisableAntiforgery();

// endpoint info (belum dipakai)
app.MapGet("/preview/{id}", FileEndpoints.GetPreviewInfo);

app.Run();


// --------------------------------------------------
// records
// --------------------------------------------------
public record StorageOptions
{
    public string BasePath { get; init; } = default!;
    public string Uploads { get; init; } = default!;
    public string Previews { get; init; } = default!;
}

public record LibreOfficeOptions
{
    // mode Windows (kalau nanti mau balik ke install lokal)
    public string? SofficePath { get; init; }

    // mode Docker
    public bool UseDocker { get; init; }
    public string DockerCliPath { get; init; } = "docker";
    public string DockerContainer { get; init; } = "libre-preview";
    public string DockerWorkDir { get; init; } = "/data";
    public string? HostStoragePath { get; init; } // path WSL2 untuk volume mapping
}
