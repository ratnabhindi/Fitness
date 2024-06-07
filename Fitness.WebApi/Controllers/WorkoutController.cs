using AutoMapper;
using Fitness.Application.Services.Interfaces;
using Fitness.Domain.Models;
using Fitness.WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace Fitness.WebApi.Controllers
{    
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [EnableCors("AllowLocalHost")]
    [Authorize]
    public class WorkoutController : ControllerBase
    {

        private readonly IWorkoutService _workoutService;
        private readonly IMapper _mapper;
        public WorkoutController(IWorkoutService workoutService, IMapper mapper)
        {
            _workoutService = workoutService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<WorkoutViewModel>>> GetAll()
        {
            var workouts = await _workoutService.GetAll();
            var workoutDTO = _mapper.Map<List<WorkoutViewModel>>(workouts);
            
            return Ok(workoutDTO);
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
            var workoutDTO = _mapper.Map<WorkoutViewModel>(workout);           

            return Ok(workoutDTO);
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
        public async Task<IActionResult> Create([FromBody] WorkoutViewModel workoutViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workoutDTO = _mapper.Map<Workout>(workoutViewModel);

            try
            {
                await _workoutService.Create(workoutDTO);
                return CreatedAtAction(nameof(GetById), new { id = workoutDTO.Id }, workoutViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

            var workoutDTO = _mapper.Map<Workout>(workoutViewModel);           

            try
            {
                await _workoutService.Update(workoutDTO);
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
