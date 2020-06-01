using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : IdentityDbContext<User,Role,int,IdentityUserClaim<int>,UserRole,IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Value> Values { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message>  Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>().HasKey(u => new { u.UserId, u.RoleId });
            modelBuilder.Entity<UserRole>().HasOne(u => u.Role).WithMany(r => r.UserRoles).HasForeignKey(f => f.RoleId).IsRequired();
            modelBuilder.Entity<UserRole>().HasOne(u => u.User).WithMany(r => r.UserRoles).HasForeignKey(f=>f.UserId).IsRequired();
            modelBuilder.Entity<Like>().HasKey(x => new { x.LikerId, x.LikeeId });
            modelBuilder.Entity<Like>().HasOne(x => x.Likee).WithMany(x => x.Likers).HasForeignKey(u => u.LikeeId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Like>().HasOne(x => x.Liker).WithMany(x => x.Likees).HasForeignKey(u => u.LikerId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>().HasOne(x => x.Sender).WithMany(x => x.MessagesSent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>().HasOne(x => x.Reciever).WithMany(x => x.MessageRecieved).OnDelete(DeleteBehavior.Restrict);



        }
    }
}