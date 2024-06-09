using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Microsoft.OpenApi.Models;
using Fitness.Application.Services.Implementation;
using Fitness.Application.Services.Interfaces;
using Fitness.WebApi.Configurations;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Fitness.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IWorkoutService, WorkoutService>();

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });

                options.AddPolicy("AllowLocalHost", policy =>
                {
                    policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
                    policy.WithExposedHeaders("X-Custom-Header");
                });

                options.AddPolicy("AllowOnlyGoogle", policy =>
                {
                    policy.WithOrigins("http://google.com").AllowAnyHeader().AllowAnyMethod();
                });
            });

            // Configure API versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Use URL segment versioning
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Configure Authentication
            var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecret"));
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    //options.Authority = "https://localhost:5001";
                    //options.Audience = "api1";
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            builder.Services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(new List<ApiResource> {
                    new ApiResource("api1", "My API")
                })
                .AddInMemoryClients(new List<Client> {
                    new Client
                    {
                        ClientId = "client",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },
                        AllowedScopes = { "api1" }
                    }
                })
                .AddTestUsers(new List<TestUser> {
                    new TestUser
                    {
                        SubjectId = "1",
                        Username = "alice",
                        Password = "password"
                    }
                });

            builder.Services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
            }).ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                            context.HttpContext,
                            context.ModelState);

                    problemDetails.Detail = "See the errors field for details.";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    var actionExecutingContext =
                         context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    if (context.ModelState.ErrorCount > 0 &&
                        (context is ControllerContext ||
                         actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "One or more validation errors occurred.";

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    }

                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more errors on input occurred.";
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the baerer scheme. Enter Bearer [space] add your token in the text input."

                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });


                // To enable xml comments
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                // Configure Swagger to support API versioning
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fitness Planning API",
                    Version = "v1.0",
                    Description = "A ASP.NET Web API implementation of RESTful Fitness Planning API",
                    TermsOfService = new Uri("https://fitnessplanning.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact Information",
                        Url = new Uri("https://fitnessplanning.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License Information",
                        Url = new Uri("https://fitnessplanning.com/license")
                    }
                });

                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Fitness Planning API",
                    Version = "v2.0",
                    Description = "A ASP.NET Web API implementation of RESTful Fitness Planning API",
                    TermsOfService = new Uri("https://fitnessplanning.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact Information",
                        Url = new Uri("https://fitnessplanning.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License Information",
                        Url = new Uri("https://fitnessplanning.com/license")
                    }
                });

                options.OperationFilter<SwaggerDefaultValues>();
            });

            builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fitness Planning API v1.0");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Fitness Planning API v2.0");
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
