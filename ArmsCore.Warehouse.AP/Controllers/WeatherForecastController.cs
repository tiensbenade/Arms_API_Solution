using Microsoft.AspNetCore.Mvc;
using ArmsCore.Warehouse.AP;
namespace ArmsCore.Warehouse.AP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public c.lass WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        public int Get() { return 1; }

        [HttpPost]
        public ActionResult<int> SubmitBatchDetails([FromBody] ReceivingHeader header)
        {
            if (header == null)
                return BadRequest("BatchDetails is null");

            ArnsCoreContext context = new ArnsCoreContext();
            context.Add(new ReceivingHeader()
            {
                InternalReference = "",
                ExternalReference = ""
            });
            //using (SqlConnection connection = _dbConnection.con)
            //{
            //    //connection.Open();
            //    //using SqlCommand command = new SqlCommand("INSERT INTO BatchDetails (InternalSerialnumber, ExternalSerialNumber) VALUES (@InternalSerialnumber, @ExternalSerialNumber)", connection);
            //    //command.Parameters.AddWithValue("@SerialNumber", BatchDetails.InternalSerialnumber);
            //    //command.Parameters.AddWithValue("@FirstSimID", BatchDetails.ExternalSerialNumber);

            //    int result = command.ExecuteNonQuery();

            //    // Check Error
            //    if (result < 0)
            //        return BadRequest(new { message = "Error adding BatchDetails" });
            //}

            return Ok(new { message = "BatchDetails added successfully!" });
            //Replace xthis code with your code

        }

        [HttpPost]
        public ActionResult<bool> AddTerminalToBatch([FromBody] ReceivingTerminals terminal)
        {
            return Ok(new { message = "BatchDetails added successfully!" });
        }
        [HttpPost]
        public ActionResult<bool> AddWayBill([FromBody] WayBill wayBill)
        {
            return Ok(new { message = "BatchDetails added successfully!" });
        }
    }
}