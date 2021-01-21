using Api.Mappers;
using Api.Models;
using Api.UserContext;
using Domain;
using Domain.CommandHandlers;
using Domain.Databse;
using Domain.Databse.Models;
using Domain.Events;
using Domain.Events.Wash;
using Domain.Implementations;
using Domain.Messaging;
using Domain.Security;
using Domain.Time;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;

namespace Api
{
    public class Startup
    {
        private readonly string AllowSpecificOrigins = "_allowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Configuration.GetSection("GOOGLE_APPLICATION_CREDENTIALS").Value);

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") ?? "GOOGLE_APPLICATION_CREDENTIALS")
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{Configuration.GetSection("ProjectId").Value}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{Configuration.GetSection("ProjectId").Value}",
                    ValidateAudience = true,
                    ValidAudience = $"{Configuration.GetSection("ProjectId").Value}",
                    ValidateLifetime = true
                };
            });

            services.AddScoped<DatabaseEntities>();
            services.AddScoped<WashEventFactory>();

            services.AddScoped<ICryptographic, Cryptographic>();
            services.AddScoped<IMessageBus, FakeBus>();
            services.AddScoped<ITimeService, TimeService>();

            services.AddScoped<IEventStore, EventStore>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IWashRepository, WashRepository>();
            services.AddScoped<ICommandHandler, WashCommandHandler>();

            services.AddScoped<ICurrentContext, UserApiContext>();
            services.AddScoped<IMapper<UserDbModel, UserDto>, UserMapper>();
            services.AddScoped<IMapper<WashDbModel, WashDto>, WashMapper>();

            services.AddCors(options =>
            {
                options.AddPolicy(AllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                                  });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UseCors(AllowSpecificOrigins);

            //app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}