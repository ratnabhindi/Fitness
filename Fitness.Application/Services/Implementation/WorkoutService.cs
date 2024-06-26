﻿using Fitness.Application.Services.Interfaces;
using Fitness.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Application.Services.Implementation
{
    public class WorkoutService : IWorkoutService
    {
        private readonly List<Workout> workouts;

        public WorkoutService()
        {
            workouts = new List<Workout>();
        }

        public Task<IEnumerable<Workout>> GetAll()
        {
            return Task.FromResult(workouts.AsEnumerable());
        }

        public Task<Workout?> GetById(Guid id)
        {
            return Task.FromResult(workouts.FirstOrDefault(w => w.Id == id));
        }

        public Task Create(Workout workout)
        {
            workouts.Add(workout);
            return Task.CompletedTask;
        }

        public Task Update(Workout updatedWorkout)
        {
            var oldWorkout = workouts.FirstOrDefault(w => w.Id == updatedWorkout.Id);
            if (oldWorkout == null)
            {
                throw new ArgumentException("Workout not found.");
            }
            //workouts.Remove(oldWorkout);
            //workouts.Add(updatedWorkout);

            oldWorkout.Name = updatedWorkout.Name;
            oldWorkout.Description = updatedWorkout.Description;
            oldWorkout.WorkoutDate = updatedWorkout.WorkoutDate;
            oldWorkout.Exercises = updatedWorkout.Exercises;

            return Task.CompletedTask;
        }

        public Task Delete(Guid id)
        {
            var existingWorkout = workouts.FirstOrDefault(w => w.Id == id);
            if (existingWorkout != null)
            {
                workouts.Remove(existingWorkout);
            }
            return Task.CompletedTask;
        }
    }
}
