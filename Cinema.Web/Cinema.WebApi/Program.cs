using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Cinema.DataAccess.Config;
using Cinema.DataAccess.Models;
using Cinema.DataAccess;
using Cinema.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));

})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cinema API",
        Version = "v1",
        Description = "ELTE Cinema API"
    });
    var xfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xpath = Path.Combine(AppContext.BaseDirectory, xfile);
    c.IncludeXmlComments(xpath);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddAuthorization();

var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSection.Get<JwtSettings>() ?? throw new ArgumentNullException(nameof(JwtSettings));
builder.Services.Configure<JwtSettings>(jwtSection);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidAudience = jwtSettings.Audience,
        ValidIssuer = jwtSettings.Issuer,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services.AddAutomapper();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

var app = builder.Build();


app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();

    var imageSource = app.Configuration.GetSection("SeedSettings").GetValue<string>("ImageSource");
    if (string.IsNullOrEmpty(imageSource))
    {
        Console.Error.WriteLine("Missing configuration Seed:ImageSource");
        Environment.Exit(1);
    }
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    DbInitializer.Initialize(context, imageSource, roleManager, userManager);
}


app.Run();