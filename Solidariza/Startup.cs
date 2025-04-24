using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.Extensions.Configuration;

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hrjendj372849fnwyd7349299kjdu8nbcoabe99"))
                };
            });

            //var connectiondb = "Server=solidarizadb.mysql.database.azure.com;Database=solidarizadb;Uid=adminSolidariza;Pwd=ADM@Solid#0424;";
            var connectiondb = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ConnectionDB>(options =>
             options.UseMySql(connectiondb,
                ServerVersion.AutoDetect(connectiondb)));

            services.AddCors(options => options.AddPolicy("PolicyCors", builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            services.AddHttpClient("http-client");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

            app.UseHttpsRedirection();
        }
    }
}

