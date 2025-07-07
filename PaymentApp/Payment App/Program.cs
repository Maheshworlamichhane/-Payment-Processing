//using Application.Common.Events;
//using Application.IPaymentService;
//using Application.ITokenService;
//using Application.Validators;
//using Confluent.Kafka;
//using FluentValidation;
//using FluentValidation.AspNetCore;
//using Infrastructure;
//using Infrastructure.Messaging;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// Add controllers
//builder.Services.AddControllers();

//// FluentValidation setup
//builder.Services.AddValidatorsFromAssemblyContaining<PaymentRequestValidator>();
//builder.Services.AddFluentValidationAutoValidation();

//// ✅ Swagger with manual Bearer token input (no auto-prepend)
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment API", Version = "v1" });

//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "Enter JWT token with **Bearer** prefix. Example: `Bearer eyJhb...`",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey, // 👈 Prevents double Bearer
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//// Add DbContext with Identity support
//builder.Services.AddDbContext<PaymentDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("Default"),
//        sql => sql.MigrationsAssembly("Infrastructure")
//    )
//);

//// Add IdentityCore without UI
//builder.Services.AddIdentityCore<IdentityUser>(options =>
//{
//    options.Password.RequireDigit = true;
//    options.Password.RequireUppercase = false;
//    options.Password.RequireNonAlphanumeric = false;
//})
//.AddEntityFrameworkStores<PaymentDbContext>();

//// Register services
//builder.Services.AddScoped<IPayment, PaymentService>();
//builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
//builder.Services.AddSingleton<IPublisher, KafkaPublisher>();
//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssembly(typeof(GetTransactionsQueryHandler).Assembly);
//});

////builder.Services.AddHostedService<KafkaConsumerService>();

//// Kafka configuration
//builder.Services.AddSingleton(new ProducerConfig
//{
//    BootstrapServers = "localhost:9092",
//    BrokerAddressFamily = BrokerAddressFamily.V4,
//});
////builder.Services.AddHostedService<PaymentConsumerService>();

//builder.Services.AddSingleton(new ConsumerConfig
//{
//    BootstrapServers = "localhost:9092",
//    GroupId = "payment-group",
//    AutoOffsetReset = AutoOffsetReset.Earliest
//});
//builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ConsumerConfig>>().Value);
//builder.Services.AddHostedService<PaymentConsumerService>();
////var config = new ProducerConfig
////{
////    BootstrapServers = "localhost:9092",
////    //ClientId = "my-app",
////    BrokerAddressFamily = BrokerAddressFamily.V4,
////};
//// JWT Authentication
//var jwtSettings = builder.Configuration.GetSection("Jwt");
//var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings["Issuer"],
//        ValidAudience = jwtSettings["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ClockSkew = TimeSpan.FromMinutes(5)
//    };

//    options.Events = new JwtBearerEvents
//    {
//        OnAuthenticationFailed = context =>
//        {
//            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
//            logger.LogError("JWT Authentication failed: {Exception}", context.Exception.Message);
//            return Task.CompletedTask;
//        },
//        OnTokenValidated = context =>
//        {
//            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
//            logger.LogInformation("JWT Token validated for {User}", context.Principal.Identity?.Name);
//            return Task.CompletedTask;
//        }
//    };
//});

//var app = builder.Build();

//// Middleware pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();
//app.Run();


using Application.Common.Events;
using Application.IPaymentService;
using Application.ITokenService;
using Application.Validators;
using Confluent.Kafka;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------
builder.Services.AddControllers();

// ✅ FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<PaymentRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

// ✅ Swagger with JWT Bearer Token support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT token with **Bearer** prefix. Example: `Bearer eyJhb...`",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    });
});

// ✅ EF Core + Identity
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        sql => sql.MigrationsAssembly("Infrastructure")
    )
);

builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<PaymentDbContext>();

// ✅ Application & Messaging Services
builder.Services.AddScoped<IPayment, PaymentService>();
builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IPublisher, KafkaPublisher>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetTransactionsQueryHandler).Assembly);
});

// Register Email settings and service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();

// ✅ Kafka
builder.Services.AddSingleton(new ProducerConfig
{
    BootstrapServers = "localhost:9092",
    BrokerAddressFamily = BrokerAddressFamily.V4,
});
builder.Services.AddSingleton(new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "payment-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
});
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ConsumerConfig>>().Value);
builder.Services.AddHostedService<PaymentConsumerService>();

// ✅ JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("JWT Authentication failed: {Exception}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("JWT Token validated for {User}", context.Principal.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});
// ✅ Session Configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // only if needed
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularClient");

app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); 

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapControllers();

app.Run();


