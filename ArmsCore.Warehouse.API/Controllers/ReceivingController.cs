using ArmsCore.Warehouse.API.Data;
using ArmsCore.Warehouse.API.Models;
using ArmsCore.Warehouse.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using ArmsCore.Warehouse.API.Util;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ArmsCore.Warehouse.API.Controllers
{
    //[Route("[controller]")]
    [ApiController]
    [Route("wa/fnb/[controller]")]
    public class ReceivingController : ControllerBase
    {
        
        private readonly ILogger<ReceivingController> _logger;
        private ArmsCoreContext _context = new ArmsCoreContext();

        public ReceivingController(ILogger<ReceivingController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("AddHeader")]
        public string AddHeader([FromBody] AddReceivingHeaderDTO header)
        {            
            if (header == null)
                return "AddReceivingHeaderDTO is null";

            try
            {
                WayBill wayBill = new WayBill()
                {
                    WayBillNumber = header.externalreference,
                    CreateUser = "Sidney",
                    CreateDate = DateTime.Now
                };
                _context.Add(wayBill);

                WayBillDocument wayBillDocument = new WayBillDocument()
                {
                    Image =header.image,
                    Filename = "test.png",
                    Extension = "png",
                    CreateUser = "Sidney",
                    CreateDate = DateTime.Now
                };
                _context.Add(wayBillDocument);

                ReceivingHeader receivingHeader = new ReceivingHeader()
                {
                    InternalReference = Util.Random.CreateNewInternalRefNumber(),
                    ExternalReference = header.externalreference,
                    BatchCompleted = false,
                    BatchQuantity = 0,
                    CourierId = header.courierid,
                    IsNewDevices = true,
                    CreateDate = DateTime.Now,
                    CreateUser = "Sidney",
                    WayBillId = wayBill.Id
                };
                _context.Add(receivingHeader);
                _context.SaveChanges();

                return receivingHeader.InternalReference;
            }
            catch(Exception ex)
            {
                //Logging here

                return ex.Message;
            }              
       }

        [HttpPost]
        [Route("AddTerminal")]
        public string AddTerminal([FromBody] AddTerminalDTO? terminal)
        {
            if (terminal == null)
                return "AddTerminalDTO is null";

            try
            {
                var serialNumber = terminal.terminalserialnumber;
                var lookup = _context.Terminals.Where(x => x.SerialNumber == serialNumber).FirstOrDefault();
                var simId1 = _context.Simcards.Where(x => x.ICCID == terminal.sim1serialnumber).FirstOrDefault();
                var simId2 = _context.Simcards.Where(x => x.ICCID == terminal.sim2serialnumber).FirstOrDefault();
                var header = _context.ReceivingHeaders.Where(x => x.InternalReference == terminal.internalreference).FirstOrDefault();

                if(lookup == null)
                {
                    lookup = _context.Terminals.ToList()[0];
                    return "Terminal serial number not found.";
                }
                if(simId1 == null)
                {
                    return "Simcard1 not found.";
                }
                if (simId2 == null)
                {
                    return "Simcard2 not found.";
                }

                var addTerminal = new ReceivingTerminal()
                {
                    TerminalId = lookup.RecordId.Value,
                    BaseSerialNumberId = terminal.baseSerialnumber,
                    CreateDate = DateTime.Now,
                    CreateUser = "Sidney",
                    HasCharger = terminal.hascharger,
                    ProvisionalSerialNumber = terminal.provisionalserialnumber,
                    Sim1Id = simId1.RecordId,
                    Sim2Id = simId2.RecordId,
                    ValidationSuccess = true,
                    ReceivingHeaderId = header.Id
                };

                _context.Add(addTerminal);
                _context.SaveChanges();

            }
            catch(Exception ex)
            {
                return ex + "Error adding terminal";
                //Logging here
            }

            return "Success adding terminal";
        }        

        [HttpGet]
        [Route("LookupSimSerial")]
        public string LookupSimSerial(string sim)
        {
            if (string.IsNullOrEmpty(sim))
            {
                return "error";
            }

            if(sim.Length != 4)
            {
                return "Please enter 4 digits only.";
            }

            try
            {
                var item = _context.Simcards.Where(x => x.ICCID.EndsWith(sim)).FirstOrDefault();
                return item.ICCID;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

        }

        [HttpGet]
        [Route("TestDatabaseConnection")]
        public string TestDatabaseConnection()
        {
            var check = false;
            try
            {
                check = _context.Terminals.Count() > 0;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return check.ToString();
        }

        [HttpGet]
        [Route("GetOriginList")]
        public List<WA_OriginList> GetOriginList()
        {
            try
            {
                return _context.Couriers.ToList();
            }
            catch(Exception ex)
            {
                //Some logging here
                return null;
            }
        }
    }

}