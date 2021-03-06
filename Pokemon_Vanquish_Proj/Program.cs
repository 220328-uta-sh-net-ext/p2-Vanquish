global using Serilog;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using DL;
using BL;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().MinimumLevel.Information().CreateLogger(); ;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager Config = builder.Configuration;
var pokePolicy = "allowedOrigins";
builder.Services.AddCors(options =>
{
options.AddPolicy(
    name: pokePolicy,
    policy => {
        policy.WithOrigins("'http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        }
    );
});
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o => {
    var key = Encoding.UTF8.GetBytes(Config["JWT:Key"]);
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidIssuer = Config["JWT:Key"],
        ValidAudience = Config["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = Config.GetConnectionString("SQLDatabase");

builder.Services.AddDbContext<VanquishDBContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUserRepoNew, UserRepoNew>();
builder.Services.AddScoped<IArtworkRepo, ArtworkRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(pokePolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
