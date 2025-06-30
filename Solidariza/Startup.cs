using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Solidariza.Common;
using Solidariza.Interfaces.Services;
using Solidariza.Services;
using System.Text;

namespace Solidariza
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Solidariza API",
                    Version = "v.1.0.0"
                });

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Description = "Insira seu Token JWT. Exemplo: \"Bearer [token]\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });

            // Pega chave JWT do ambiente/configuração (NUNCA HARD CODE!)
            string? jwtKey = Configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JwtKey não encontrada nas configurações do ambiente!");
            }

            services.AddAuthentication(options =>
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
                    ValidIssuer = "Solidariza",
                    ValidAudience = "Application",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            services.AddAuthorization();

            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<ICampaignVolunteerService, CampaignVolunteerService>();
            services.AddScoped<ILinkService, LinkService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IOrganizationInfoService, OrganizationInfoService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IValidateOrganizationService, ValidateOrganizationService>();

            // Pega string de conexão por configuração/variável de ambiente
            var connectiondb = Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectiondb))
                throw new InvalidOperationException("DefaultConnection não encontrada nas configurações do ambiente!");

            services.AddDbContext<ConnectionDB>(options =>
                options.UseMySql(connectiondb, ServerVersion.AutoDetect(connectiondb)));

            services.AddCors(options => options.AddPolicy("PolicyCors", builder => builder
                .WithOrigins(
                    "http://localhost:4200",
                    "https://solidariza-web-ctd5dpbjauchgufp.centralus-01.azurewebsites.net"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()));

            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));

            services.AddHttpClient("http-client");
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("PolicyCors");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}