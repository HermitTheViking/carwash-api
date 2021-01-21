using Domain;
using Domain.CommandHandlers;
using Domain.Databse;
using Domain.Databse.Models;
using Domain.Events;
using Domain.Events.Wash;
using Domain.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Utility.Logging;
using Utility.Messaging;
using Utility.Security;
using Utility.Time;
using Api.Mappers;
using Api.Models;
using Api.UserContext;

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
            services.AddScoped<DatabaseEntities>();
            services.AddScoped<WashEventFactory>();

            services.AddScoped<IStringHash, StringHash>();
            services.AddScoped<IMessageBus, FakeBus>();
            services.AddScoped<ITimeService, TimeService>();
            services.AddScoped<ILogFactory, LogFactory>();

            services.AddScoped<IEventStore, EventStore>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IWashRepository, WashRepository>();
            services.AddScoped<ICommandHandler, WashCommandHandler>();

            services.AddScoped<ICurrentContext, UserApiContext>();
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

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}