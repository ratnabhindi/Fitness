using Fitness.Domain.Models;
using Fitness.WebApi.Utils;
using System.ComponentModel.DataAnnotations;

namespace Fitness.WebApi.ViewModels
{
    public class WorkoutViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Workout name is required")]
        [StringLength(100, ErrorMessage = "Workout name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [DateCheck("01/01/2020", ErrorMessage = "Workout date must be on or after January 1, 2020")]
        public DateTime WorkoutDate { get; set; }

        [Required(ErrorMessage = "At least one exercise is required")]
        public List<ExerciseViewModel> Exercises { get; set; }

       
    }
}
