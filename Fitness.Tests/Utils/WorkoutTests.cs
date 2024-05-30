using Fitness.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
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
            var factory = new WebApplicationFactory<Program>();
            httpClient = factory.CreateClient();
            workoutService = factory.Services.GetService(typeof(IWorkoutService))
                                as IWorkoutService
                                ?? throw new SystemException(nameof(IWorkoutService)
                                                                    + " is not registered.");
        }
    }
}
