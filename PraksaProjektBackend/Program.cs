using PraksaProjektBackend.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using PraksaProjektBackend.Settings;
using PraksaProjektBackend.Services;
using System.Text.Json.Serialization;
using PraksaProjektBackend.Filter;
using Microsoft.AspNetCore.OData;
using PraksaProjektBackend.ExternalLogin;
using PraksaProjektBackend.ExternalLogin.Facebook;
using System.Net.Mail;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.


// For Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnStr")));

// For Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddGoogle(options =>
{
    options.ClientId = "115115841938-siocnn1d7h9cuvs209t0j1s3avnrmepm.apps.googleusercontent.com";
    options.ClientSecret = "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";
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["token"];
            return Task.CompletedTask;
        }
    };
});
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddCors();

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddOData(options => options.Select().Filter().OrderBy().Expand().Count().SkipToken());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<JwtHandler>();
//fluentmail
builder.Services
        .AddFluentEmail("tttest203@gmail.com", "Dev")
        .AddRazorRenderer()
        .AddSmtpSender(new SmtpClient("smtp.gmail.com")
        {
            UseDefaultCredentials = false,
            Port = 587,
            Credentials = new NetworkCredential("tttest203@gmail.com", "Test123!"),
            EnableSsl = true,
        });
//Facebook login
var facebookAuthSettings = new FacebookAuthSettings();
configuration.Bind(nameof(FacebookAuthSettings), facebookAuthSettings);
builder.Services.AddSingleton(facebookAuthSettings);
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IFacebookAuthService, FacebookAuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
                          builder =>
                          {
                              builder.SetIsOriginAllowed(_ => true).AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials();
                          });
}); 
//Swagger JWT Token
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "PraksaProjekt-Backend", Version = "v1" });
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
    option.SchemaFilter<SwaggerIgnoreFilter>();
    option.OperationFilter<IgnorePropertyFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("MyPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
