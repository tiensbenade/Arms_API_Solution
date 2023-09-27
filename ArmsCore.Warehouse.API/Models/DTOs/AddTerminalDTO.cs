namespace ArmsCore.Warehouse.API.Models.DTOs
{
    public class AddTerminalDTO
    {
        public string internalreference { get; set; } = string.Empty;
        public string terminalserialnumber { get; set; } = string.Empty;
        public string sim1serialnumber { get; set; } = string.Empty;
        public string sim2serialnumber { get; set; } = string.Empty;
        public string baseSerialnumber { get; set; } = string.Empty;
        public string provisionalserialnumber { get; set; } = string.Empty;
        public bool hascharger { get; set; }
    }
}
