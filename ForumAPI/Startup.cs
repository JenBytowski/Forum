using ForumAPI.Filters;
using ForumAPI.Services.BoardService;
using ForumAPI.Services.BoardService.BoardDataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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

            services.AddControllers(conf =>
            {
                conf.Filters.Add<RequestCheckFilter>();
                conf.Filters.Add<ExceptionHandlingFilterAsync>();
            });
            
            services.AddMvc();

            services.AddHttpClient();
            services.AddScoped<IBoardService,BoardService>();
            
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("boards", new OpenApiInfo {Title = "Boards", Version = "1.0.0"});
                opt.SwaggerDoc("topics", new OpenApiInfo {Title = "Topics", Version = "1.0.0"});
                opt.SwaggerDoc("posts", new OpenApiInfo {Title = "Posts", Version = "1.0.0"});
            });
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
                opt.SwaggerEndpoint("/swagger/boards/swagger.json", "boards");
                opt.SwaggerEndpoint("/swagger/topics/swagger.json", "topics");
                opt.SwaggerEndpoint("/swagger/posts/swagger.json", "posts");
            });
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}