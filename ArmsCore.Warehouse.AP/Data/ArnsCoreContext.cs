using ARMS_Warehouse_API.Models;
using Microsoft.EntityFrameworkCore;


namespace ARMS_Warehouse_API.Data
{
    public class ArnsCoreContext : DbContext
    {
        DbSet<ReceivingHeader> receivingHeaders { get; set; }
        DbSet<ReceivingTerminals> terminals { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var configuation = GetConfiguration();
            optionsBuilder.UseSqlServer(configuation.GetSection("ConnectionString").Value);
        }

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
    }
}
