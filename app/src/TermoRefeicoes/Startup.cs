using System;
using System.IO;
using System.Text;
using termoRefeicoes.Helper.Connection;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Interfaces.Services.Security;
using termoRefeicoes.Models;
using termoRefeicoes.Services;
using termoRefeicoes.Services.Report;
using termoRefeicoes.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.Extensions.FileProviders;

namespace TermoRefeicoes
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddSpaStaticFiles(configuration =>
           {
               configuration.RootPath = "ClientApp/build";
           });

            services.Configure<LdapConfig>(this.Configuration.GetSection("Ldap"));
            services.AddScoped<ILogin, LoginService>();
            services.AddScoped<IAuthentication, LdapAuthService>();

            var logger = new LoggerConfiguration()
              .MinimumLevel.Verbose()
              .WriteTo.Console()
              .WriteTo.File("logs" + Path.DirectorySeparatorChar + "app.log", rollingInterval: RollingInterval.Month)
              .CreateLogger();

            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<IConnectionFactory, ConnectionFactory>();
            services.AddHttpContextAccessor();//permite o acesso de dados via token jwt, em qlqer lugar da aplicação (IHttpContextAccessor _httpContextAccessor;)

            services.AddScoped<IUserSenior, UserSeniorService>();
            services.AddScoped<IRefeicoes, RefeicoesService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITermo, TermoService>();
            services.AddScoped<ITermosAceitosReport, TermosAceitosService>();
            services.AddScoped<IConsumosAceitosReport, ConsumosAceitosService>();
            services.AddScoped<ISetor, SetorService>();

            services.AddCors();


            var key = Encoding.ASCII.GetBytes((Configuration["Jwt:Key"]));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(x =>
              {
                  x.RequireHttpsMetadata = false;
                  x.SaveToken = true;
                  x.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(key),
                      ValidateIssuer = false,
                      ValidateAudience = false
                  };
              });


            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "termoRefeicoes", Version = "v1" });
            // });

            services.AddSwaggerGen(c =>
                  {

                      c.SwaggerDoc("v1",
                  new OpenApiInfo
                  {
                      Title = "API de integração com apliativos Portal Unimed",
                      Version = "v1",
                      Description = "API de integração com o aplicativo do beneficiário do Portal Unimed",
                      Contact = new OpenApiContact
                      {
                          Name = "Unimed Chapecó",
                          Url = new Uri("https://www.unimedchapeco.coop.br")
                      }
                  });
                      c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                      {
                          Description = "Favor inserir um token JWT válido",
                          Name = "Authorization",
                          Type = SecuritySchemeType.Http,
                          BearerFormat = "JWT",
                          In = ParameterLocation.Header,
                          Scheme = "bearer"
                      });
                      c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                  new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                      Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                  },
                  new string[] { }
                }
                    });

                  });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(x => _ = true)
            );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSerilogRequestLogging();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "termoRefeicoes v1"));
            }

            // app.UseHttpsRedirection();

            // app.UseStaticFiles(
            //     new StaticFileOptions
            //     {
            //         FileProvider = new PhysicalFileProvider(
            // Path.Combine(env.ContentRootPath, "wwwroot")),
            //         RequestPath = "/template"
            //     });
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapControllers();
            // });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
