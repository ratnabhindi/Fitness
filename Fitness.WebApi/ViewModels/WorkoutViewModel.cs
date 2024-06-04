using Fitness.Domain.Models;
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

        [Required(ErrorMessage = "At least one exercise is required")]
        public List<ExerciseViewModel> Exercises { get; set; }
    }
}
