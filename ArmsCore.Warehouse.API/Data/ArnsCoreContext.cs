using ArmsCore.Warehouse.API.Models;
using Microsoft.EntityFrameworkCore;


namespace ArmsCore.Warehouse.API.Data
{
    public class ArmsCoreContext : DbContext
    {
        //"Server=tcp:sqlserv-nova-uat.database.windows.net,1433;Initial Catalog=sqldb-nova-portal-uat;Persist Security Info=False;User ID=dustyn;Password=Password01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //Username=TS_FNB;Password = dGN47 >
        private string conn = "Server=192.168.102.16,1433;Initial Catalog=ArmscoreFNB;Persist Security Info=False;User ID=TS_FNB;Password=dGN47>;MultipleActiveResultSets=False;TrustServerCertificate=True;Connection Timeout=30;";
        public ArmsCoreContext() : base()
        {

        }

        public DbSet<ReceivingHeader> ReceivingHeaders { get; set; }
        public DbSet<ReceivingTerminal> ReceivingTerminals { get; set; }
        public DbSet<WayBill> WayBills { get; set; }
        public DbSet<WayBillDocument> WayBillDocuments { get; set; }   
        public DbSet<GL_MasterTerminals> Terminals { get; set; }
        public DbSet<GL_MasterSimcards> Simcards { get; set; }
        public DbSet<WA_OriginList> Couriers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var configuation = GetConfiguration();
            optionsBuilder.UseSqlServer(configuation.GetSection("ConnectionString").Value);
            //optionsBuilder.UseSqlServer(conn);
            
        }

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GL_MasterTerminals>().HasNoKey();
        }


    }
}
