using System.ComponentModel.DataAnnotations;

namespace FlightPricesAPI.Models
{
    public class FlightRequest
    {
        [Required]
        public string Departure { get; set; }

        [Required]
        public string Arrival { get; set; }

        [Required]
        [RegularExpression(@"\d{4}-\d{2}-\d{2}")]
        public string DepartureDate { get; set; }
    }
}
