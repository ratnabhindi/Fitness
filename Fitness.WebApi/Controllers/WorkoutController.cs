using Fitness.Application.Services.Interfaces;
using Fitness.Domain.Models;
using Fitness.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Fitness.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class WorkoutController : ControllerBase
    {

        private readonly IWorkoutService _workoutService;

        public WorkoutController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<WorkoutViewModel>>> GetAll()
        {
            var workouts = await _workoutService.GetAll();
            var workoutViewModels = workouts.Select(workout => new WorkoutViewModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                Exercises = workout.Exercises.Select(ex => new ExerciseViewModel
                {
                    Id = ex.Id,
                    Name = ex.Name,
                    Sets = ex.Sets,
                    Repetitions = ex.Repetitions,
                    Weight = ex.Weight,
                    Duration = ex.Duration
                }).ToList()
            }).ToList();
            return Ok(workoutViewModels);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<WorkoutViewModel>> GetById(Guid id)
        {
            var workout = await _workoutService.GetById(id);
            if (workout == null)
            {
                return NotFound();
            }

            var workoutViewModel = new WorkoutViewModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
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

            return Ok(workoutViewModel);
        }

        /// <summary>
        /// Creates a workout.
        /// </summary>
        /// <param name="plan"></param>
        /// <returns>A newly created workout</returns>
        /// <remarks>
        /// Request Example:
        ///
        ///     POST /api/workout
        ///     {
        ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "name": "string",
        ///       "description": "string",
        ///       "date": "2023-01-26T17:33:50.275Z",
        ///       "exercises": [
        ///         {
        ///           "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///           "name": "string",
        ///           "sets": 0,
        ///           "repetitions": 0,
        ///           "weight": 0,
        ///           "duration": 0
        ///         }
        ///       ]
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created plan</response> 
        /// <response code="400">If the posted plan is null</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Affects /swagger => the responses section
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(WorkoutViewModel workoutViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workout = new Workout
            {
                Id = Guid.NewGuid(),
                Name = workoutViewModel.Name,
                Description = workoutViewModel.Description,
                Exercises = workoutViewModel.Exercises.Select(ex => new Exercise
                {
                    Id = Guid.NewGuid(),
                    Name = ex.Name,
                    Sets = ex.Sets,
                    Repetitions = ex.Repetitions,
                    Weight = ex.Weight,
                    Duration = ex.Duration
                }).ToList()
            };

            await _workoutService.Create(workout);

            workoutViewModel.Id = workout.Id;

            return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workoutViewModel);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid id, [FromBody] WorkoutViewModel workoutViewModel)
        {
            if (id != workoutViewModel.Id)
            {
                return BadRequest();
            }

            var workout = new Workout
            {
                Id = workoutViewModel.Id,
                Name = workoutViewModel.Name,
                Description = workoutViewModel.Description,
                Exercises = workoutViewModel.Exercises.Select(ex => new Exercise
                {
                    Id = ex.Id,
                    Name = ex.Name,
                    Sets = ex.Sets,
                    Repetitions = ex.Repetitions,
                    Weight = ex.Weight,
                    Duration = ex.Duration
                }).ToList()
            };

            try
            {
                await _workoutService.Update(workout);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _workoutService.Delete(id);
            return NoContent();
        }

    }
}
