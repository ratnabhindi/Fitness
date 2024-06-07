using Fitness.Application.Services.Implementation;
using Fitness.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using Fitness.WebApi.Configurations;

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
                    policy.WithExposedHeaders("X-Custom-Header"); // Exposing custom headers for debugging
                });

                options.AddPolicy("AllowOnlyGoogle", policy =>
                {
                    policy.WithOrigins("http://google.com").AllowAnyHeader().AllowAnyMethod();
                });
            });

            // Configure Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:5001"; // URL of your IdentityServer
                    options.Audience = "api1";
                    options.RequireHttpsMetadata = false;
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
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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
