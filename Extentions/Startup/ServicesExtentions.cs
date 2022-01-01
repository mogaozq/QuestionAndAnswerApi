using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using QuestionAndAnswerApi.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Extentions.Startup
{
    public static class ServicesExtentions
    {
        public static void Add_Identity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["Auth0:Authority"];
                    options.Audience = configuration["Auth0:Audience"];
                });
        }

        public static void Add_Authorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("QuestionAuthor", policyBuilder => policyBuilder.AddRequirements(new QuestionAuthorRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, QuestionAuthorAuthorizationHandler>();
        }
        


        public static void Add_Swagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "QuestionAndAnswerApi", Version = "v1" });
                ////documentation xml file of current project(Tadena.App.xml)
                ////multiple project can be added like above
                //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlFullPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                //opt.IncludeXmlComments(xmlFullPath);

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Add 'Bearer {token}' string into params",
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                        },
                        new List<string>()
                    }
                });
            });

        }
    }
}
