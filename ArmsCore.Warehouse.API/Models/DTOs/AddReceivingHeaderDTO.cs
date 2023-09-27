using Microsoft.Data.SqlClient.DataClassification;

namespace ArmsCore.Warehouse.API.Models.DTOs
{
    public class AddReceivingHeaderDTO
    {
        public string externalreference { get; set; } = string.Empty;
        public int batchquantity { get; set; }
        public string image { get; set; } = string.Empty;
        public int courierid { get; set; }
        public bool isnewdevice { get; set; }
    }
}
