using AutoMapper;
using Fitness.Domain.Models;
using Fitness.WebApi.ViewModels;

namespace Fitness.WebApi.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<WorkoutViewModel, Workout>().ReverseMap();
            CreateMap<ExerciseViewModel, Exercise>().ReverseMap();
        }
    }
}
