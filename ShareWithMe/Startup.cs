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
using Microsoft.IdentityModel.Tokens;
using ShareWithMe.Hubs;
using SWM.Core.Files;
using SWM.Core.Repositories;
using SWM.Core.SharedFiles;
using SWM.Core.Users;
using SWM.EFCore;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using SWM.Helpers;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.Extensions.Hosting;
using SWM.Core;
using SWM.EFCore.Repositories;
using SWM.EFCore.UnitOfWork;

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
            services.AddDbContextPool<SWMDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddControllersWithViews(opt => opt.Filters.Add<ExceptionFilter>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddApplicationPart(Assembly.Load(new AssemblyName("SWM.Application")))
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory())));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.WithOrigins(Configuration["App:CorsOrigins"].Split(','))
                       .AllowAnyMethod()
                       .AllowAnyHeader());
            });


            services.AddSignalR();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IUserManager), typeof(UserManager));
            services.AddScoped(typeof(ISharedFileManager), typeof(SharedFileManager));
            services.AddScoped(typeof(IFileManager), typeof(FileManager));
            services.AddSingleton(typeof(Logger<>));
            services.AddSingleton<Mapper>();
            services.AddSingleton<ProgressHub>();

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

            services.AddTransient<SmtpClient>((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var client = new SmtpClient
                {
                    ServerCertificateValidationCallback = delegate { return true; }
                };
                client.Connect(Configuration["Email:Smtp:Host"], int.Parse(Configuration["Email:Smtp:Port"]), false);
                client.Authenticate(Configuration["Email:Smtp:Username"], Configuration["Email:Smtp:Password"]);
                return client;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = ".Net Core Web APi", Version = "v1.0" });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                var scheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "Bearer",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                };

                c.AddSecurityDefinition("Bearer", scheme);
                scheme.Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" };
                c.AddSecurityRequirement(new OpenApiSecurityRequirement() { { scheme, new List<string>() } });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }



        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }



            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Share With Me Api v1.0");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<ProgressHub>("/progressHub");
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
