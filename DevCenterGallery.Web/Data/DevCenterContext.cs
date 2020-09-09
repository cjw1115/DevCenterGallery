using DevCenterGallary.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCenterGallery.Web.Data
{
    public class DevCenterContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Package> Packages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=devcenter.db");
    }
}
