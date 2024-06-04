using Fitness.Application.Services.Interfaces;
using Fitness.Domain.Models;
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

        public WorkoutTests()
        {
            var factory = new WebApplicationFactory<WebApi.Program>();
            httpClient = factory.CreateClient();
            workoutService = factory.Services.GetService(typeof(IWorkoutService))
                                as IWorkoutService
                                ?? throw new SystemException(nameof(IWorkoutService)
                                                                    + " is not registered.");
        }

        protected WorkoutViewModel MapToViewModel(Workout workout)
        {
            return new WorkoutViewModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                WorkoutDate = workout.WorkoutDate,
                Exercises = workout.Exercises.Select(ex => new ExerciseViewModel
                {
                    Id = ex.Id,
                    Name = ex.Name,
                    Sets = ex.Sets,
                    Repetitions = ex.Repetitions,
                    Weight = ex.Weight,
                    Duration = ex.Duration
                }).ToList()
            };
        }
    }    
}
