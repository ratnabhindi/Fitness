using Fitness.Application.Services.Interfaces;
using Fitness.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Tests.Utils
{
    public static class WorkoutServiceTestExtensions
    {
        public static async Task Seed(this IWorkoutService service, params Workout[] workouts)
        {
            foreach (var workout in workouts)
                await service.Create(workout);
        }
    }
}
