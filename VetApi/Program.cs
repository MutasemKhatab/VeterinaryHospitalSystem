using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Vet.DataAccess.Data;
using Vet.DataAccess.Repository;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;
using VetApi.Utils;

namespace VetApi;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(policy => {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers();

        #region Configure Db Contexts

// Simple Database
/*
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite("Data Source=../Vet.db;Cache=Shared"));
    */

// MySQL Database
        builder.Services.AddDbContext<AuthDbContext>(options =>
            options.UseMySql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                new MariaDbServerVersion(new Version(8, 0, 21))
            ));

        #endregion

        #region Configure Identity

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        #endregion

        #region Configure Authentication

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
        builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        builder.Services.AddAuthorization();

        #endregion

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        var app = builder.Build();
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        #region Apply Migration

        using (var scope = app.Services.CreateScope()) {
            var services = scope.ServiceProvider;

            var authContext = services.GetRequiredService<AuthDbContext>();
            try {
                authContext.Database.Migrate();
            }
            catch (Exception ex) {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }

        #endregion

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}