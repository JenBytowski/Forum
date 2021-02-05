using ForumAPI.Filters;
using ForumAPI.Services.BoardService;
using ForumAPI.Services.BoardService.BoardDataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ForumAPI
{
    internal sealed class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BoardContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("BoardContext")));
            
            services.AddControllers(conf => conf.Filters.Add<RequestCheckAsyncFilter>());
            services.AddMvc();

            services.AddHttpClient();
            services.AddScoped<IBoardService,BoardService>();
            
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();
            
            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1.0.0");
            });
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}