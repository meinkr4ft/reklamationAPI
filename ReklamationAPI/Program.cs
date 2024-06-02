using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReklamationAPI.Authentication;
using ReklamationAPI.data;
using ReklamationAPI.Services;
using ReklamationAPI.Swagger;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManager<IdentityUser>, UserManagerWrapper>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddHostedService<OutboxProcessor>();
builder.Services.AddSingleton(sp =>
{
    var emailSettings = builder.Configuration["EmailSettings:EmailDirectory"];
    ArgumentNullException.ThrowIfNull(emailSettings);
    return new EmailService(emailSettings);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Include Filters for Example Request / Response Objects
    c.ExampleFilters();

    // Include XML Comments for Request Documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Include annotations for parameter description etc.
    c.EnableAnnotations();
    c.OperationFilter<ParameterDefaults>();

    // Define the security scheme
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Bearer",
        Description = "JWT Authorization header using the Bearer scheme. Use the token you received from /api/login.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // Case sensitive string
        BearerFormat = "JWT"
    };

    // Add the security scheme to the Swagger document
    c.AddSecurityDefinition("Bearer", securityScheme);

    // Configure the security requirements
    var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        };

    // Add the security requirements to the Swagger document
    c.AddSecurityRequirement(securityRequirement);
});

// Add Providing Examples
builder.Services.AddSwaggerExamplesFromAssemblyOf<ComplaintDtoExmaple>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ComplaintResponseExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ComplaintResponseArrayExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LoginExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LoginResponseExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<SearchResponseExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<FilterResponseExample>();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = AuthConfig.ApiJwtIssuer,
        ValidAudience = AuthConfig.ApiJwtAudience,
        IssuerSigningKey = AuthConfig.ApiJwtSigningKey
    };
});

// Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        await SeedData.Initialize(services, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

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

app.Run();
