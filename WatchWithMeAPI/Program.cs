using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Polly;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.DTOs.Room.Requests;
using WatchWithMeAPI.Hubs;
using WatchWithMeAPI.Model;
using WatchWithMeAPI.Services;
using WatchWithMeAPI.Services.NekoService;
using WatchWithMeAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Adding Swagger 
builder.Services.AddSwaggerGen( c => {

    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo {
    
        Title = "WathWithMeManagement API",
        Version = "v1"

    });

    // Define the security scheme 
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme { 
    
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your valid JWT token below."

    });

    // Apply the security requirement globally ( attaches it to your request )
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme{

                Reference = new Microsoft.OpenApi.Models.OpenApiReference{ 
                    
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"

                }

            },
            Array.Empty<string>() // No specific scopes required, just the token
        }
    });


});

// Connecting to DataBase
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<WatchWithMeContext>(options => {
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});

// Setting up the Identity for DataBase
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Optional: You can put password requirements here later
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<WatchWithMeContext>() // Tells Identity to use your SQL database
    .AddDefaultTokenProviders();

builder.Services.AddCors(options => {

    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});


// Configuration of 0Auth 2.0 
var google = builder.Configuration.GetSection("Authentication:Google");

// Sets the rules for the middleware 
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(options => // Handle the actual login logic
    {
        options.ClientId = google["ClientId"]!;
        options.ClientSecret = google["ClientSecret"]!;
        options.CallbackPath = "/signin-google";
    })
    .AddJwtBearer(options => { // Adds to support for JWT 

        options.MapInboundClaims = false; // Tells Microsoft to not auto-translate our jwt claims
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        
        // This validates the JWT 
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Secret"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

        };
        
        // Setup for WebSockets (SignalR)
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                // Get the JWT from the query string
                var accessToken = context.Request.Query["access_token"];
                
                // Get the path that the user is trying to access 
                var path = context.HttpContext.Request.Path;
                
                // If it has the token and the path is SignalR Hub, use the token
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/room"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };

    });

builder.Services.AddAuthorization();

// Inject the services
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<IRoomService, RoomService>();

// Inject the custom validators
builder.Services.AddScoped<IValidator<ProfilePictureUpdateRequestDTO>, ProfilePictureUpdateRequestValidator>();
builder.Services.AddScoped<IValidator<EmailUpdateRequestDTO>, EmailUpdateRequestValidator>();
builder.Services.AddScoped<IValidator<PasswordUpdateRequestDTO>, PasswordUpdateRequestValidator>();
builder.Services.AddScoped<IValidator<UserNameUpdateRequestDTO>, UserNameUpdateRequestValidator>();
builder.Services.AddScoped<IValidator<StatusUpdateRequestDTO>, StatusUpdateRequestValidator>();
builder.Services.AddScoped<IValidator<DisplayStatusUpdateRequestDTO>, DisplayStatusUpdateRequestValidator>();
builder.Services.AddScoped<IValidator<NotificationOptionsUpdateRequestDTO>, NotificationOptionsRequestValidator>();
builder.Services.AddScoped<IValidator<CreateNewRoomRequestDTO>, CreateNewRoomRequestValidator>();
builder.Services.AddScoped<IValidator<JoinRoomWithShareCodeRequestDTO>, JoinRoomWithShareCodeRequestValidator>();

// Turn the SignalR engine on and expose the End points
builder.Services.AddSignalR();

// Service for n.eko
builder.Services.AddScoped<INekoService, NekoService>();

// Ignore SSL Certificate Validation
var httpClientHandler = new HttpClientHandler();
httpClientHandler.ServerCertificateCustomValidationCallback =
    (message, cert, chain, errors) => { return true; };

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    // Generate the swagger.json file
    app.UseSwagger();

    // Build the visual UI
    app.UseSwaggerUI( c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WatchWithMeManagement API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Look for the JWT set up before and identifies the user 
app.UseAuthentication();

// Checks if the identified user has the right permissions to access the requested URL 
app.UseAuthorization();

// Allows the use of "wwwroot" folder
app.UseStaticFiles();

// Routes the request to your controller classes
app.MapControllers();

// Register and Map the Hubs
app.MapHub<RoomHub>("hubs/room");

app.Run();
