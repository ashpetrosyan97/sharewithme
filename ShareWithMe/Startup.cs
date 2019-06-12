using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ShareWithMe.Hubs;
using Swashbuckle.AspNetCore.Swagger;
using SWM.Core;
using SWM.Core.Files;
using SWM.Core.Repositories;
using SWM.Core.SharedFiles;
using SWM.Core.Users;
using SWM.EFCore;
using System.Collections.Generic;
using System.IO;
using Serilog.Extensions.Logging.File;
using System.Net;
using System.Reflection;
using System.Text;

namespace ShareWithMe
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
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddApplicationPart(Assembly.Load(new AssemblyName("SWM.Application")))
                .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory())));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.WithOrigins(Configuration["App:CorsOrigins"].Split(','))
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .AllowAnyHeader());
            });

            services.AddSignalR();

            services.AddDbContext<SWMDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddTransient<IRepository<UserEntity, long>, Repository<UserEntity, long>>();

            services.AddTransient(typeof(IUserManager), typeof(UserManager));

            services.AddTransient<IRepository<SharedFileEntity, long>, Repository<SharedFileEntity, long>>();

            services.AddTransient(typeof(ISharedFileManager), typeof(SharedFileManager));

            services.AddTransient<IRepository<FileEntity, long>, Repository<FileEntity, long>>();

            services.AddTransient(typeof(IFileManager), typeof(FileManager));

            services.AddTransient<ProgressHub>();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = Configuration["Authentication:Audience"],

                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Authentication:Issuer"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"])),
                };
            });

            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

           /* services.AddTransient<SmtpClient>((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var client = new SmtpClient();
                client.ServerCertificateValidationCallback = delegate { return true; };
                client.Connect(Configuration["Email:Smtp:Host"], int.Parse(Configuration["Email:Smtp:Port"]), false);
                client.Authenticate(
                   Configuration["Email:Smtp:Username"],
                    Configuration["Email:Smtp:Password"]
                    );
                return client;
            });*/

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info() { Title = ".Net Core Web APi", Version = "v1.0" });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            loggerFactory.AddFile("Logs/SWM-{Date}.txt");

            app.UseDeveloperExceptionPage();

            app.UseDatabaseErrorPage();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ProgressHub>("/progressHub");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Share With Me Api v1.0");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
