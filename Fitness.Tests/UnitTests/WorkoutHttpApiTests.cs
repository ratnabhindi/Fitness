using AutoFixture.Xunit2;
using Fitness.Application.Services.Implementation;
using Fitness.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Tests.UnitTests
{
    public class WorkoutHttpApiTests : WorkoutTests
    {
        [Fact]
        public async Task GetAll_Returns200OK()
        {
            var response = await httpClient.GetAsync("/api/workout/");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory, AutoData]
        public async Task GetById_Returns200OK(Workout workoutSeed)
        {
            await workoutService.Seed(workoutSeed);

            var response = await httpClient.GetAsync($"/api/workout/{workoutSeed.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory, AutoData]
        public async Task GetById_WithNonExistingWorkoutId_Returns404NotFound(Workout NonExistentWorkout)
        {
            var response = await httpClient.GetAsync($"/api/workout/{NonExistentWorkout.Id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }



        [Theory, AutoData]
        public async Task Create_Returns201Created(Workout workoutSeed)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(workoutSeed), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/workout/", requestContent);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory, AutoData]
        public async Task Create_Returns400BadRequest(Workout invalidWorkout)
        {          
            var requestContent = new StringContent(JsonConvert.SerializeObject(invalidWorkout), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/workout", requestContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Theory, AutoData]
        public async Task Update_Returns200OK(Workout workoutSeed)
        {
            await workoutService.Seed(workoutSeed);
            var requestContent = new StringContent(JsonConvert.SerializeObject(workoutSeed), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/api/workout/{workoutSeed.Id}", requestContent);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }


        [Theory, AutoData]
        public async Task Update_WithWrongId_Returns400BadRequest(Workout dummyWorkout, Guid wrongWorkoutId)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(dummyWorkout), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/api/workout/{wrongWorkoutId}", requestContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Theory, AutoData]
        public async Task Update_NonExisting_Returns404NotFound(Workout nonExistingWorkout)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(nonExistingWorkout), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/api/workout/{nonExistingWorkout.Id}", requestContent);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Theory, AutoData]
        public async Task Delete_Returns200OK(Workout workoutSeed)
        {
            await workoutService.Seed(workoutSeed);
            var response = await httpClient.DeleteAsync($"/api/workout/{workoutSeed.Id}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
