using AutoFixture.Xunit2;
using Fitness.Domain.Models;
using Fitness.Tests.Utils;
using Fitness.WebApi.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fitness.Tests.IntegrationTests
{
    public class WorkoutIntegrationTests : WorkoutTests
    {
        private WorkoutViewModel MapToViewModel(Workout workout)
        {
            return new WorkoutViewModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                Exercises = workout.Exercises?.Select(ex => new ExerciseViewModel
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

        [Theory, AutoData]
        public async Task Gets_all_workouts(Workout[] workoutSeeds)
        {
            await workoutService.Seed(workoutSeeds);

            var response = await httpClient.GetAsync("/api/workout/");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var downloadedWorkouts = JsonConvert.DeserializeObject<WorkoutViewModel[]>(responseContent);

            var expectedWorkouts = workoutSeeds.Select(MapToViewModel).ToArray();
            Assert.Equivalent(expectedWorkouts, downloadedWorkouts);
        }

        [Theory, AutoData]
        public async Task Finds_a_workout_by_its_id(Workout workoutSeed)
        {
            await workoutService.Seed(workoutSeed);

            var response = await httpClient.GetAsync($"/api/workout/{workoutSeed.Id}");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var workout = JsonConvert.DeserializeObject<WorkoutViewModel>(responseContent);

            var expectedWorkout = MapToViewModel(workoutSeed);
            Assert.Equivalent(expectedWorkout, workout);
        }

        [Theory, AutoData]
        public async Task Creates_new_workouts(Workout workoutSeed)
        {
            var workoutViewModel = MapToViewModel(workoutSeed);
            var requestContent = new StringContent(JsonConvert.SerializeObject(workoutViewModel), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/workout/", requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdWorkout = JsonConvert.DeserializeObject<WorkoutViewModel>(responseContent);

            var workoutInDb = await workoutService.GetById(createdWorkout.Id);
            var expectedWorkout = MapToViewModel(workoutInDb);
            Assert.Equivalent(expectedWorkout, createdWorkout);
        }

        [Theory, AutoData]
        public async Task Updates_existing_workouts(Workout originalWorkout, Workout workoutUpdate)
        {
            workoutUpdate.Id = originalWorkout.Id;
            await workoutService.Seed(originalWorkout);

            var workoutViewModel = MapToViewModel(workoutUpdate);
            var requestContent = new StringContent(JsonConvert.SerializeObject(workoutViewModel), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/api/workout/{originalWorkout.Id}", requestContent);
            response.EnsureSuccessStatusCode();

            var workoutInDb = await workoutService.GetById(originalWorkout.Id);
            var expectedWorkout = MapToViewModel(workoutInDb);
            Assert.Equivalent(workoutViewModel, expectedWorkout);
        }

        [Theory, AutoData]
        public async Task Deletes_workouts(Workout existingWorkout)
        {
            await workoutService.Seed(existingWorkout);

            var response = await httpClient.DeleteAsync($"/api/workout/{existingWorkout.Id}");
            response.EnsureSuccessStatusCode();

            var workout = await workoutService.GetById(existingWorkout.Id);
            Assert.Null(workout);
        }
    }
}
