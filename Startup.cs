using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using QuestionAndAnswerApi.Data;
using QuestionAndAnswerApi.Data.Cache;
using QuestionAndAnswerApi.Data.Dapper.Repositories;
using QuestionAndAnswerApi.Data.EntityFrameworkCore;
using QuestionAndAnswerApi.Data.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi
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
            services.AddDbContext<EfDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")).UseLowerCaseNamingConvention());
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QuestionAndAnswerApi", Version = "v1" });
            });

            services.AddScoped<IQuestionRepository, EfQuestionRepository>();
            services.AddScoped<IAnswerRepository, EfAnswerRepository>();

            services.AddScoped<IQuestionRepository, DpQuestionRepository>();
            services.AddScoped<IAnswerRepository, DpAnswerRepository>();

            services.AddMemoryCache();
            services.AddSingleton<IQuestionCache, QuestionCache>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuestionAndAnswerApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
