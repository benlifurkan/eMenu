using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace eMenu.Models
{
    public partial class eMenuContext : DbContext
    {
        public eMenuContext()
            : base("name=eMenuContext")
        {
        }

        public virtual DbSet<Authority> Authority { get; set; }
        public virtual DbSet<Foods> Foods { get; set; }
        public virtual DbSet<Menus> Menus { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Foods>()
                .Property(e => e.price)
                .HasPrecision(19, 4);
        }
    }
}
