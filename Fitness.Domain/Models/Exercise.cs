using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Domain.Models
{
    public class Exercise
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public int Weight { get; set; }
        public int Duration { get; set; }
    }
}
