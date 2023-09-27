using Microsoft.AspNetCore.Mvc;
using ARMS_Warehouse_API.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace ARMS_Warehouse_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class loadDeviceWithSimsController : ControllerBase
    {
        //private readonly db_Conn _dbConnection;

        public loadDeviceWithSimsController()
        {
            //_dbConnection = new db_Conn();
           // _dbConnection.db(); // Initialize the DB connection
        }

        //[HttpPost(Name = "LoadDeviceWithSims")]
        //public ActionResult<int> LoadDeviceWithSims([FromBody] TerminalAndSims terminalAndSims)
        //{
        //    if (terminalAndSims == null)
        //        return BadRequest("No Records to add");

        //    using (SqlConnection connection = _dbConnection.con)
        //    {
        //        connection.Open();
        //        using SqlCommand command = new SqlCommand("INSERT INTO TerminalDetails (SerialNumber, FirstSimID, SecondSimID) VALUES (@SerialNumber, @FirstSimID, @SecondSimID)", connection);
        //        command.Parameters.AddWithValue("@SerialNumber", terminalAndSims.SerialNumber);
        //        command.Parameters.AddWithValue("@FirstSimID", terminalAndSims.FirstSimID);
        //        command.Parameters.AddWithValue("@SecondSimID", terminalAndSims.SecondSimID);

        //        int result = command.ExecuteNonQuery();

        //        // Check Error
        //        if (result < 0)
        //            return BadRequest(new { message = "Error adding terminalAndSims" });
        //    }

        //    return Ok(new { message = "terminalAndSims added successfully!" });
        //}
    }
}
