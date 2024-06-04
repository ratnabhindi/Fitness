using System.ComponentModel.DataAnnotations;

namespace Fitness.WebApi.ViewModels
{
    public class ExerciseViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Exercise name is required")]
        [StringLength(100, ErrorMessage = "Exercise name cannot be longer than 100 characters")]
        public string Name { get; set; }

       // [Range(1, 10, ErrorMessage = "Sets must be between 1 and 10")]
        public int Sets { get; set; }

      //  [Range(1, 50, ErrorMessage = "Repetitions must be between 1 and 50")]
        public int Repetitions { get; set; }

      //  [Range(0, 1000, ErrorMessage = "Weight must be between 0 and 1000")]
        public int Weight { get; set; }

       // [Range(0, 3600, ErrorMessage = "Duration must be between 0 and 3600 seconds")]
        public int Duration { get; set; }
    }
}
