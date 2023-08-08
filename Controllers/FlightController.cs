using FlightPricesAPI.Models;
using FluentValidation;
using HtmlAgilityPack;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc.Html;

namespace FlightPricesAPI.Controllers
{

    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiController]
    
    public class FlightController : ControllerBase
    {
        private readonly ILogger<FlightController> _logger;
        private IValidator<FlightRequest> _flifhtRequetValidator;

        public FlightController(ILogger<FlightController> logger, IValidator<FlightRequest> flifhtRequetValidator)
        {
            _logger = logger;
            _flifhtRequetValidator = flifhtRequetValidator;
        }


        /// <summary>
        /// Retrieves available flights and their prices.
        /// </summary>

        [HttpPost]
        public async Task<IActionResult> GetFlights([FromBody] FlightRequest request)
        {

            var result = await _flifhtRequetValidator.ValidateAsync(request);

                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }

                    return BadRequest(ModelState);
                }

            try
            {
                var flights = await ScrapeFlights(request.Departure, request.Arrival, request.DepartureDate);
                return Ok(flights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to fetch flight data" });
            }
        }

        private async Task<List<FlightInfo>> ScrapeFlights(string departure, string arrival, string departureDate)
        {
            var baseUrl = "http://example-travel-company.com/flights"; // Replace this with the actual travel company's website
            var queryString = $"departure={departure}&arrival={arrival}&departure_date={departureDate}";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{baseUrl}?{queryString}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var flightsData = ParseHtmlContent(content);
                return flightsData;
            }
        }

        private List<FlightInfo> ParseHtmlContent(string htmlContent)
        {
            var flightsData = new List<FlightInfo>();
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var flightInfoNodes = doc.DocumentNode.SelectNodes("//div[@class='flight-info']");
            if (flightInfoNodes != null)
            {
                foreach (var node in flightInfoNodes)
                {
                    var flightName = node.SelectSingleNode(".//span[@class='flight-name']").InnerText;
                    var price = node.SelectSingleNode(".//span[@class='flight-price']").InnerText;

                    flightsData.Add(new FlightInfo { FlightName = flightName, Price = price });
                }
            }

            return flightsData;
        }
    }

}
