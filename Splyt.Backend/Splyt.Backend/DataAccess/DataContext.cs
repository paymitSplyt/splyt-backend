using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Backend.DataAccess.Models;

namespace Backend.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            Database.SetInitializer(new DbInitializer());
        }

        public IDbSet<CartItem> CartItems { get; set; }
        public IDbSet<CartItem_User> CartItemUsers { get; set; }
        public IDbSet<Cart> Carts { get; set; }
        public IDbSet<Merchant> Merchants { get; set; }

        public IDbSet<Product> Products { get; set; }
        public IDbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}