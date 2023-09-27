using ARMS_Warehouse_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ARMS_Warehouse_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RandomController : ControllerBase
    {
        //private readonly db_Conn _dbConnection;

        private readonly Random _random = new Random();

        [HttpGet(Name ="GenerateNewInternalReference")]
        public ActionResult<int> GenerateRandomNumber(string CustomerName)
        {
            //Replace with your Code
            int randomNumber = _random.Next();
            return Ok(randomNumber);
        }
    }
}
