using ArmsCore.Warehouse.API.Data;
using ArmsCore.Warehouse.API.Models;
using ArmsCore.Warehouse.API.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ArmsCore.Warehouse.API.Controllers
{
    //[Route("[controller]")]
    [ApiController]
    [Route("wa/fnb/[controller]")]
    public class ReceivingController : ControllerBase
    {

        private readonly ILogger<ReceivingController>? _logger;
        private ArmsCoreContext _context = new ArmsCoreContext();
        private ArmsCoreGlobalContext _globalContext = new ArmsCoreGlobalContext();
        
        public ReceivingController(ILogger<ReceivingController> logger)
        {
            _logger = logger;            
        }

        [HttpPost]
        [Route("AddHeader")]
        public ActionResult<string> AddHeader([FromBody] AddReceivingHeaderDTO header)
        {
            if (header == null)
                return "AddReceivingHeaderDTO is null";

            if (_context.ReceivingHeaders.Where(x => x.ExternalReference == header.externalreference).Count() > 0)
            {
                return "External reference already exists. Would you like to add terminals?";
            }

            try
            {
                //var loggedInUser = JsonSerializer.Deserialize<User>(_httpContextAccessor.HttpContext.Session.GetString("User"));

                ReceivingHeader receivingHeader = new ReceivingHeader()
                {
                    InternalReference = Util.Random.CreateNewInternalRefNumber(header.courierid),
                    ExternalReference = header.externalreference,
                    BatchCompleted = false,
                    BatchQuantity = header.batchquantity,
                    CourierId = header.courierid,
                    IsNewDevices = true,
                    CreateDate = DateTime.Now,
                    CreateUser = "Sidney"//loggedInUser.UserName?? "Sidney",
                };
                _context.Add(receivingHeader);
                _context.SaveChanges();

                WayBill wayBill = new WayBill()
                {
                    HeaderId = receivingHeader.Id,
                    WayBillNumber = header.externalreference,
                    CreateUser = "Sidney",//loggedInUser.UserName ?? "Sidney",
                    CreateDate = DateTime.Now,
                };
                _context.Add(wayBill);
                _context.SaveChanges();

                WayBillDocument wayBillDocument = new WayBillDocument()
                {
                    ReceivingWayBillId = wayBill.Id,
                    Image = header.image,
                    Filename = header.filename,
                    Extension = Path.GetExtension(header.filename),
                    CreateUser = "Sidney",//loggedInUser.UserName ?? "Sidney",
                    CreateDate = DateTime.Now
             
                };

                _context.Add(wayBillDocument);
                _context.SaveChanges();

                return Ok(receivingHeader.InternalReference);
            }
            catch (Exception ex)
            {
                //Logging here

                return ex.Message;
            }
        }

        [HttpPost]
        [Route("AddTerminal")]
        public ActionResult AddTerminal([FromBody] AddTerminalDTO? terminal)
        {
            if (terminal == null)
                return BadRequest("AddTerminalDTO is null");

            try
            {
                var header = _context.ReceivingHeaders.Where(x => x.InternalReference == terminal.internalreference).FirstOrDefault();

                if (header == null)
                {
                    return BadRequest("Reference number not found");
                }

                //Add one sim
                if(terminal.sim1serialnumber.Length > 0 && terminal.sim2serialnumber.Length > 0)
                {
                    var simId1 = _context.Simcards.Where(x => x.ICCID == terminal.sim1serialnumber).FirstOrDefault();
                    var simId2 = _context.Simcards.Where(x => x.ICCID == terminal.sim2serialnumber).FirstOrDefault();
                    var addTerminal = new ReceivingTerminal()
                    {
                        TerminalId = 0,
                        BaseSerialNumberId = "",
                        CreateDate = DateTime.Now,
                        CreateUser = "Sidney",//loggedInUser.UserName,
                        HasCharger = false,
                        ProvisionalSerialNumber = "",
                        Sim1Id = simId1.RecordId,
                        Sim2Id = simId2.RecordId,
                        ValidationSuccess = true,
                        ReceivingHeaderId = Convert.ToInt64(header.Id)
                    };
                    _context.Add(addTerminal);
                    _context.SaveChanges();
                    return Ok("Successfully added");
                }
                else if(terminal.sim1serialnumber.Length > 0)
                {
                    var simId1 = _context.Simcards.Where(x => x.ICCID == terminal.sim1serialnumber).FirstOrDefault();

                    var addTerminal = new ReceivingTerminal()
                    {
                        TerminalId = 0,
                        BaseSerialNumberId = "",
                        CreateDate = DateTime.Now,
                        CreateUser = "Sidney",//loggedInUser.UserName,
                        HasCharger = false,
                        ProvisionalSerialNumber = "",
                        Sim1Id = simId1.RecordId,
                        Sim2Id = 0,
                        ValidationSuccess = true,
                        ReceivingHeaderId = Convert.ToInt64(header.Id)
                    };
                    _context.Add(addTerminal);
                    _context.SaveChanges();
                    return Ok("Successfully added");
                }

                var term = _context.Terminals.Where(x => x.SerialNumber == terminal.terminalserialnumber).FirstOrDefault();
                if (term == null)
                {
                    return BadRequest("Terminal not found in [GL_MasterTerminals]");
                }

                var termData = _context.terminalImportData.Where(x => x.SerialNumber == term.SerialNumber).FirstOrDefault();

                if (termData.ItemStatus != 22 && termData.ItemStatus != 3 && termData.ItemStatus != 4 && termData.ItemStatus != 21)
                {
                    return BadRequest("Terminal cannot be added because it's state is " + termData.StatusDescription);
                }

                //var loggedInUser = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("User"));

                if (term != null)
                {
                    var tmTerminal = _context.TM_Terminals.Where(x => x.TerminalId == term.RecordId).FirstOrDefault();
                    if (tmTerminal == null)
                    {
                        return BadRequest("Terminal not found in [TM_Terminals]");
                    }

                    var simId1 = _context.Simcards.Where(x => x.ICCID == terminal.sim1serialnumber).FirstOrDefault();
                    var simId2 = _context.Simcards.Where(x => x.ICCID == terminal.sim2serialnumber).FirstOrDefault();

                    var addTerminal = new ReceivingTerminal()
                    {
                        TerminalId = Convert.ToInt64(term.RecordId),
                        BaseSerialNumberId = terminal.baseSerialnumber,
                        CreateDate = DateTime.Now,
                        CreateUser = "Sidney",//loggedInUser.UserName,
                        HasCharger = terminal.hascharger,
                        ProvisionalSerialNumber = terminal.provisionalserialnumber,
                        Sim1Id = simId1.RecordId != 0 ? Convert.ToInt64(simId1.RecordId) : 0,
                        Sim2Id = simId2.RecordId is not 0 ? Convert.ToInt64(simId2.RecordId) : 0,
                        ValidationSuccess = true,
                        ReceivingHeaderId = Convert.ToInt64(header.Id)
                    };

                    _context.Add(addTerminal);
                }
                else if (!string.IsNullOrEmpty(terminal.baseSerialnumber))
                {
                    var addTerminal = new ReceivingTerminal()
                    {
                        TerminalId = 0,
                        BaseSerialNumberId = terminal.baseSerialnumber,
                        CreateDate = DateTime.Now,
                        CreateUser = "Sidney",//loggedInUser.UserName,
                        HasCharger = terminal.hascharger,
                        ProvisionalSerialNumber = terminal.provisionalserialnumber,
                        Sim1Id = 0,
                        Sim2Id = 0,
                        ValidationSuccess = true,
                        ReceivingHeaderId = Convert.ToInt64(header.Id)
                    };
                }
                else if (!string.IsNullOrEmpty(terminal.provisionalserialnumber))
                {
                    var addTerminal = new ReceivingTerminal()
                    {
                        TerminalId = 0,
                        BaseSerialNumberId = terminal.baseSerialnumber,
                        CreateDate = DateTime.Now,
                        CreateUser = "Sidney",//loggedInUser.UserName,
                        HasCharger = terminal.hascharger,
                        ProvisionalSerialNumber = terminal.provisionalserialnumber,
                        Sim1Id = 0,
                        Sim2Id = 0,
                        ValidationSuccess = true,
                        ReceivingHeaderId = Convert.ToInt64(header.Id)
                    };
                }

                var tmpTerminal = _context.terminalTmp.Where(x => x.SerialNumber == terminal.terminalserialnumber).FirstOrDefault();
                
                if (tmpTerminal != null)
                {
                    _context.terminalTmp.Remove(tmpTerminal);
                }

                _context.SaveChanges();

                return Ok("Successfully added");

            }
            catch (Exception ex)
            {
                return BadRequest(ex + " Error adding data");
                //Logging here
            }
        }

        [HttpPost]
        [Route("AddTerminalList")]
        public ActionResult AddTerminalList([FromBody] TerminalListDTO list)
        {
            if (list == null)
                return BadRequest("List is empty");

            var terminalListItems = new List<TerminalList>();

            try
            {
                var header = _context.ReceivingHeaders.Where(x => x.ExternalReference == list.externalReference).FirstOrDefault();

                if (header == null)
                {
                    return BadRequest("Header not found");
                }

                foreach (var item in list.serialnumbers)
                {
                    _context.Add(new ReceivingTerminalTmp()
                    {                       
                        InternalReference = list.externalReference,
                        SerialNumber = item.terminalserialnumber
                    });
                    var lookup = _context.Terminals.Where(x => x.SerialNumber == item.terminalserialnumber).FirstOrDefault();

                    if (lookup == null)
                    {
                        terminalListItems.Add(new TerminalList()
                        {
                            terminalserialnumber = item.terminalserialnumber,
                            status = "Not valid"
                        });
                    }
                    else
                    {
                        terminalListItems.Add(new TerminalList()
                        {
                            terminalserialnumber = item.terminalserialnumber,
                            status = "Valid"
                        });
                    }

                }
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return BadRequest(ex + " Error adding terminal");
                //Logging here
            }

            return Ok(terminalListItems);
        }


        private string RemoveDuplicateTerminals()
        {
            return string.Empty;
        }

        [HttpGet]
        [Route("SavedList")]
        public ActionResult SavedList()
        {
            var items = _context.ReceivingHeaders.ToList();
            var terminals = _context.ReceivingTerminals.ToList();
            var x = 0;

            foreach(var item in items)
            {
                var count = terminals.Where(x => x.ReceivingHeaderId == item.Id).Count();
                items[x++].ReceivedQuantity = count;
            }

            return Ok(items);

        }

        [HttpGet]
        [Route("GetUsers")]
        public ActionResult GetUsers()
        {
            var usersList = new List<User>();
            var header = _context.ReceivingHeaders.Where(x => x.CreateDate < DateTime.Now.AddDays(30)).ToList().Select(y => y.CreateUser).Distinct();
            var users = _globalContext.SystemUsers.ToList();

            foreach(var item in header)
            {
                usersList.Add(new User()
                {
                    UserId = users.Where(x => x.Username == item).FirstOrDefault().RecordId,
                    UserName = item,
                    UserEmail = users.Where(x => x.Username == item).FirstOrDefault().Description
                });

            }         
                
            return Ok(usersList);
        }
        
        [HttpGet]
        [Route("GetSavedBatch")]
        public ActionResult GetSavedBatch(string internalReference)
        {
            if (string.IsNullOrEmpty(internalReference))
                return BadRequest("InternalReference is null");

            try
            {
                var header = _context.ReceivingHeaders.Where(x => x.InternalReference == internalReference).FirstOrDefault();

                if (header == null)
                    return BadRequest("InternalReference is null");

                var wayBill = _context.WayBills.Where(x => x.HeaderId == header.Id).FirstOrDefault();
                var docs = _context.WayBillDocuments.Where(x => x.ReceivingWayBillId == wayBill.Id).FirstOrDefault();
                AddReceivingHeaderDTO? item = default;

                if (docs != null)
                {
                    item = new AddReceivingHeaderDTO()
                    {
                        externalreference = header.ExternalReference,
                        batchquantity = header.BatchQuantity,
                        courierid = header.CourierId,
                        filename = docs.Filename,
                        image = docs.Image

                    };
                }
                else
                {
                    item = new AddReceivingHeaderDTO()
                    {
                        externalreference = header.ExternalReference,
                        batchquantity = header.BatchQuantity,
                        courierid = header.CourierId,
                        filename = "",
                        image = ""

                    };
                }

                List<AddTerminalDTO> list = new List<AddTerminalDTO>();
                var terminals = _context.ReceivingTerminals.Where(x => x.ReceivingHeaderId == header.Id).ToList();

                foreach (var terminal in terminals)
                {
                    var t = _context.Terminals.Where(x => x.RecordId == terminal.Id).FirstOrDefault();
                    if (t == null)
                    { continue; }
                    var lookup = _context.terminalImportData.Where(x => x.SerialNumber == t.SerialNumber).FirstOrDefault();
                    if (lookup == null) { continue; }
                    var simId1 = _context.Simcards.Where(x => x.RecordId == terminal.Sim1Id).FirstOrDefault();
                    var simId2 = _context.Simcards.Where(x => x.RecordId == terminal.Sim2Id).FirstOrDefault();
                    if (simId1 == null || simId2 == null) { continue; }

                    AddTerminalDTO i = new AddTerminalDTO()
                    {
                        terminalserialnumber = lookup.SerialNumber,
                        sim1serialnumber = simId1.ICCID,
                        sim2serialnumber = simId2.ICCID,
                        baseSerialnumber = terminal.BaseSerialNumberId,
                        provisionalserialnumber =  terminal.ProvisionalSerialNumber,
                        hascharger = terminal.HasCharger
                    };
                }

                item.terminallist = list;

                var tmpList = _context.terminalTmp.Where(x => x.InternalReference == header.ExternalReference).ToList();
                List<TerminalItem> l = new List<TerminalItem>();

                foreach (var tmp in tmpList)
                {
                    TerminalItem i = new TerminalItem()
                    {
                        terminalserialnumber = tmp.SerialNumber,
                        status = "Not Valid"
                    };
                    l.Add(i);
                }
                
                item.tempList = l;

                return Ok(item);                

            }
            catch(Exception ex) { 
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("FinishBatch")]
        public ActionResult FinishBatch(string internalReference)
        {
            var header = _context.ReceivingHeaders.Where(x => x.InternalReference == internalReference).FirstOrDefault();
            if (header == null)
            {
                return BadRequest("Header not found");
            }

            var terminals = _context.ReceivingTerminals.Where(x => x.ReceivingHeaderId == header.Id).ToList();

            foreach(var term in terminals)
            {
                _context.SLAs.Add(new RE_SLA()
                {
                    TerminalId = term.TerminalId,
                    LimitHours = 120,
                    UserId = 4
                });

                using (var command = new SqlCommand("Update GL_MasterTerminals SET ProductionStateId=5 WHERE REcordId=" + term.TerminalId))
                {
                    using (var sql = new SqlConnection("Server=192.168.102.16;Database=ArmsCoreFNB;Integrated Security=True;TrustServerCertificate=True"))
                    {
                        sql.Open();
                        command.Connection = sql;
                        command.ExecuteNonQuery();
                        sql.Close();
                    }
                }
                
            }
            header.BatchCompleted = true;
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("LookupSimSerial")]
        public ActionResult LookupSimSerial(string terminalserialnumber, string sim)
        {
            if (string.IsNullOrEmpty(sim))
            {
                return BadRequest("Sim cannot be empty");
            }

            if (sim.Length != 4 && sim.Length != 19 && sim.Length != 20)
            {
                return BadRequest("Please enter 4 digits only or full sim number.");
            }

            try
            {
                var terminal = _context.Terminals.Where(x => x.SerialNumber == terminalserialnumber).FirstOrDefault();
                if (terminal == null)
                {
                    return BadRequest("Terminal serialnumber not found");
                }
                var tmTerminal = _context.TM_Terminals.Where(x => x.TerminalId == terminal.RecordId).ToList();
                if (tmTerminal == null)
                {
                    return BadRequest(string.Format("No record for terminal {0} found in terminals table.", terminalserialnumber));
                }

                if (sim.Length == 4)
                {
                    if (tmTerminal.FirstOrDefault().Sim1.EndsWith(sim))
                    {
                        return Ok(tmTerminal.FirstOrDefault().Sim1);
                    }

                    if (tmTerminal.FirstOrDefault().Sim2.EndsWith(sim))
                    {
                        return Ok(tmTerminal.FirstOrDefault().Sim2);
                    }
                }
                else
                {
                    if (tmTerminal.FirstOrDefault().Sim1 == sim)
                    {
                        return Ok(tmTerminal.FirstOrDefault().Sim1);
                    }

                    if (tmTerminal.FirstOrDefault().Sim2 == sim)
                    {
                        return Ok(tmTerminal.FirstOrDefault().Sim2);
                    }
                }

                return BadRequest("Simcard not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("GetTerminalListMetaData")]
        public ActionResult GetTerminalListMetaData(string externalReference)
        {
            try
            {
                var header = _context.ReceivingHeaders.Where(x => x.ExternalReference == externalReference).FirstOrDefault();
                if (header == null)
                {
                    return BadRequest("External reference was not found.");
                }

                var terminals = _context.ReceivingTerminals.Where(x => x.ReceivingHeaderId == header.Id).ToList();

                var terminalList = new List<TerminalImportData>();

                foreach (var item in terminals)
                {
                    var terminal = _context.Terminals.Where(x => x.RecordId == item.TerminalId).FirstOrDefault();
                    var lookup = _context.terminalImportData.Where(x => x.SerialNumber == terminal.SerialNumber).FirstOrDefault();

                    if (lookup != null)
                    {
                        terminalList.Add(new TerminalImportData()
                        {
                            SerialNumber = terminal.SerialNumber,
                            TerminalDescription = lookup.ItemTypeDescription,
                            Owner = lookup.UserName,
                            Allocation = "",
                            TerminalStatus = terminal.ProductionStateId.Value.ToString(),
                            StatusDescription = lookup.StatusDescription
                        });
                    }
                    else
                    {
                        terminalList.Add(new TerminalImportData()
                        {
                            SerialNumber = terminal.SerialNumber,
                            TerminalDescription = "No data",
                            Owner = "No data",
                            Allocation = ""
                        });
                    }
                }

                return Ok(terminalList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("GetSummary")]
        public ActionResult GetSummary(string externalReference)
        {
            var header = _context.ReceivingHeaders.Where(x => x.ExternalReference == externalReference).FirstOrDefault();
            if (header == null)
            {
                return BadRequest("Externalreference not found");
            }

            Summary summary = new Summary();
            summary.TerminalCount = _context.ReceivingTerminals.Where(x => x.ReceivingHeaderId == header.Id).Count().ToString();
            var baseCount = 0;
            var baseItems = _context.ReceivingTerminals.Where(x => x.ReceivingHeaderId == header.Id);
            var mtnCount = 0;
            var vodaCount = 0;
            var simList = _context.Simcards.ToList();

            foreach (var item in baseItems)
            {
                if (item.BaseSerialNumberId != "0" && item.BaseSerialNumberId != "")
                {
                    baseCount++;
                    var sim1 = simList.Where(x => x.RecordId == item.Sim1Id).FirstOrDefault();
                    var sim2 = simList.Where(x => x.RecordId == item.Sim2Id).FirstOrDefault();
                    if(sim1.ICCID.Length == 19)
                    {
                        mtnCount++;
                    }
                    else
                    {
                        vodaCount++;
                    }
                    if (sim2.ICCID.Length == 20)
                    {
                        vodaCount++;
                    }
                    else
                    {
                        mtnCount++;
                    }
                }
            }
            

            summary.BaseCount = baseCount.ToString();
            summary.SimCount = (int.Parse(summary.TerminalCount) * 2).ToString();
            summary.RepairAssessment = "0";
            summary.AssessmentApproval = "0";
            summary.SimManagement = "0";
            summary.Link2500 = "1";
            summary.Move3500 = "1";
            summary.TetraBase = "3";
            summary.ICT250 = "4";
            summary.SimMTNCount = mtnCount.ToString();
            summary.SimVodacomCount = vodaCount.ToString();
            summary.ChargerCount = _context.ReceivingTerminals.Where(x => x.ReceivingHeaderId == header.Id && x.HasCharger == true).Count().ToString();

            return Ok(summary);
        }

        [HttpGet]
        [Route("CancelSession")]
        public ActionResult CancelSession(string externalReference)
        {
            try
            {
                var header = _context.ReceivingHeaders.Where(x => x.ExternalReference == externalReference).FirstOrDefault();

                if (header == null)
                {
                    return BadRequest("Externalreference not found");
                }

                var terminals = _context.ReceivingTerminals.Where(x => x.ReceivingHeaderId == (Int64)header.Id).ToList();

                foreach (var item in terminals)
                {
                    _context.ReceivingTerminals.Remove(item);
                }
                _context.ReceivingHeaders.Remove(header);
                _context.SaveChanges();

                return Ok("Deleted success.");
            }
            catch (Exception ex)
            {
                return Ok();
                //return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Route("TestDatabaseConnection")]
        public string TestDatabaseConnection()
        {
            var x = this.ControllerContext.HttpContext;

            var check = false;
            try
            {
                check = _context.Terminals.Count() > 0;
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                //Some logging here
                var item = new WA_OriginList()
                {
                    Description = ex.Message
                };

                var list = new List<WA_OriginList>();
                list.Add(item);
                return list;
            }
        }

        [HttpPost]
        [Route("InitToken")]
        public ActionResult InitToken([FromBody] TokenDTO token)
        {
            var stream = token.Token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;

            var u = _globalContext.SystemUsers.Where(x => x.Username == tokenS.Payload["name"].ToString()).FirstOrDefault();
            User user = new User()
            {
                UserId = u.RecordId,
                UserName = tokenS.Payload["name"].ToString()?? "Not found",
                UserEmail = tokenS.Payload["emails"].ToString()?? "Not Found"
            };

            //_httpContextAccessor.HttpContext.Session.SetString("User", JsonSerializer.Serialize(user));
            //var test = JsonSerializer.Deserialize<User>(_httpContextAccessor.HttpContext.Session.GetString("User"));
            return Ok("User token received [");// + JsonSerializer.Deserialize<User>(_httpContextAccessor.HttpContext.Session.GetString("User")) + "]");
        }

        [HttpGet]
        [Route("CheckCurrentUser")]
        public ActionResult CheckCurrentUser()
        {
            var user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("User"));

            return Ok(user);
        }
    }
}
