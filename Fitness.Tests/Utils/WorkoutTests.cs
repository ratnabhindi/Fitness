using AutoMapper;
using Fitness.Application.Services.Interfaces;
using Fitness.Domain.Models;
using Fitness.WebApi.Configurations;
using Fitness.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Tests.Utils
{
    public abstract class WorkoutTests
    {
        protected readonly HttpClient httpClient;
        protected readonly IWorkoutService workoutService;
        protected readonly IMapper mapper;

        public WorkoutTests()
        {
            var factory = new WebApplicationFactory<WebApi.Program>();
            httpClient = factory.CreateClient();
            workoutService = factory.Services.GetService(typeof(IWorkoutService))
                                as IWorkoutService
                                ?? throw new SystemException(nameof(IWorkoutService)
                                                                    + " is not registered.");

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            });
            mapper = config.CreateMapper();
        }

        protected WorkoutViewModel MapToViewModel(Workout workout)
        {
            return mapper.Map<WorkoutViewModel>(workout);
        }

        protected async Task SeedDataAsync()
        {
            var workout = new Workout
            {
                Id = Guid.NewGuid(),
                Name = "Test Workout",
                Description = "This is a test workout",
                WorkoutDate = DateTime.UtcNow,
                Exercises = new List<Exercise>
            {
                new Exercise
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Exercise",
                    Sets = 3,
                    Repetitions = 10,
                    Weight = 50,
                    Duration = 30
                }
            }
            };

            await workoutService.Seed(workout);
        }
    }    
}
