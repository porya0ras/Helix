using helix;
using helix.Data;
using helix.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using static IdentityModel.OidcConstants.Algorithms;
using helix.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using helix.Services.interfaces;
using helix.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);


var log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
Log.Logger = log;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentity<User, IdentityRole>()
     .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;
    })
    .AddIdentityServerJwt()
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),
            ValidateIssuer=true,
            ValidIssuer=builder.Configuration["JWT:Issuer"],
            ValidateAudience=true,
            ValidAudience=builder.Configuration["JWT:Audience"]
            
        };
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddTransient<IBufferedFileUploadService, BufferedFileUploadLocalService>();

builder.Services.AddDirectoryBrowser();


builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

Task.Run(async () =>
    {
        await Task.Delay(1000);

        using (var scope = builder.Services.BuildServiceProvider().CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                await SeedData.InitialAsync(services);
            }
            catch (Exception ex)
            {
                var Logger = services.GetRequiredService<ILogger<Program>>();
                Logger.LogError(ex, "Error in SeedData.");
            }
        }
    });
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseCors("corsapp");
//app.UseHttpsRedirection();

var provider = new FileExtensionContentTypeProvider();
// Add new mappings
provider.Mappings[".fits"] = "application/fits-image";
provider.Mappings[".jpeg"] = "image/jpeg";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "UploadedFiles")),
    RequestPath = "/UploadedFiles",

});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//ExceptionHanddler
app.ConfigureBuiltInExceptionHandler();

app.MapControllers();
app.MapRazorPages();
app.MapFallbackToFile("index.html"); ;


bool MaintenanceMode = (bool)builder.Configuration.GetValue(typeof(bool), "MaintenanceMode") == true;

if (MaintenanceMode)
{
    app.UseWelcomePage();
}
app.Run();
