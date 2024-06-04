using System.ComponentModel.DataAnnotations;

namespace Fitness.WebApi.Utils
{
    public class DateCheckAttribute : ValidationAttribute
    {
        private readonly DateTime _minDate;

        public DateCheckAttribute(string minDate)
        {
            _minDate = DateTime.Parse(minDate);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime dateValue;
                bool isDate = DateTime.TryParse(value.ToString(), out dateValue);
                if (isDate && dateValue >= _minDate)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult($"The date must be on or after {_minDate.ToShortDateString()}.");
        }
    }
}
