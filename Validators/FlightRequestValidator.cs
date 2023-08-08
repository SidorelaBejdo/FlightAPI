

using FluentValidation;
using FlightPricesAPI.Models;

namespace FlightPricesAPI.Validators
{
    public class FlightRequestValidator : AbstractValidator<FlightRequest>
    {
        public FlightRequestValidator()
        {
            RuleFor(x => x.Departure)
                .NotEmpty().WithMessage("Departure code cannot be empty.")
                .Length(3).WithMessage("Departure code must be a 3-letter code.");

            RuleFor(x => x.Arrival)
                .NotEmpty().WithMessage("Arrival code cannot be empty.")
                .Length(3).WithMessage("Arrival code must be a 3-letter code.");

            RuleFor(x => x.DepartureDate)
                .NotEmpty().WithMessage("Departure date cannot be empty.")
                .Matches(@"\d{4}-\d{2}-\d{2}").WithMessage("Invalid date format. Use 'YYYY-MM-DD'.");
        }
    }
}



