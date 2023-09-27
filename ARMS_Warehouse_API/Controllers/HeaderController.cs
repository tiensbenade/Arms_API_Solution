using Microsoft.AspNetCore.Mvc;
using ARMS_Warehouse_API.Models;
using ARMS_Warehouse_API.Data;

namespace ARMS_Warehouse_API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class HeaderController : ControllerBase
    {
        //private readonly db_Conn _dbConnection;

        public HeaderController()
        {
            //_dbConnection = new db_Conn();
            //_dbConnection.db(); // Initialize the DB connection
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
        public ActionResult<bool>AddTerminalToBatch([FromBody] ReceivingTerminals terminal)
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
