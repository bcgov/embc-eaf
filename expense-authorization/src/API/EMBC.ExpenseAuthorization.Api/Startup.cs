using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EMBC.ExpenseAuthorization.Api
{
    public class Startup
    {
        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<Startup>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            // send header Access-Control-Allow-Origin: *
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowAnyOrigin();
                });
            });


            // add all the handlers in this assembly
            services.AddMediatR(GetType().Assembly);

            AddSwaggerGen(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // uncomment RequireAuthorization once we have keycload
                endpoints.MapControllers()/*.RequireAuthorization()*/;
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // name here shows up in the 'Select a definition' drop down
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EMBC Expense Authorization API V1");
            });

        }


        private void AddSwaggerGen(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // name here shows up in the OpenAPI title
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EMBC Expense Authorization",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Email = "apiteam@example.org",
                        Name = "API Team",
                        Url = new Uri("https://github.com/bcgov/embc-expense-authorization")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Apache 2.0",
                        Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                    }
                });

#if KEYCLOAK
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
#endif

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.OperationFilter<ConsumesAndProductOnlyJSonContentTypeFilter>();
            });
        }

        internal class ConsumesAndProductOnlyJSonContentTypeFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                // feels like there should be a better way than doing this on each operation
                RemoveNonJsonResponseTypes(operation);
            }

            private void RemoveNonJsonResponseTypes(OpenApiOperation operation)
            {
                foreach (var response in operation.Responses)
                {
                    var content = response.Value.Content;

                    var keys = content.Keys;
                    foreach (var key in keys)
                    {
                        if (key != "application/json")
                        {
                            content.Remove(key);
                        }
                    }
                }
            }
        }
    }
}
