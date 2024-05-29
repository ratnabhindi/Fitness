using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.Domain.Models;

namespace Fitness.Application.Services.Interfaces
{
    public interface IFitnessService
    {
        Task<IEnumerable<Workout>> GetAll();
        Task<Workout?> GetById(Guid id);
        Task Create(Workout fitness);
        Task Update(Workout fitness);
        Task Delete(Guid id);
    }
}
