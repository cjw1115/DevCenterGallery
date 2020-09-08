using DevCenterGallary.Common.Models;
using Microsoft.EntityFrameworkCore;
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

        public DevCenterContext(DbContextOptions<DevCenterContext> options)
            : base(options)
        {
        }
    }
}
