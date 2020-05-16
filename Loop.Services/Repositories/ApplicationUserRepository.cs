﻿using System.Linq;
using System.Data.Entity;
using Loop.Database;
using Loop.Entities.Concrete;
using Loop.Services.Interfaces.Repositories;
using System.Collections.Generic;
using Loop.Entities;

namespace Loop.Services.Repositories
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationDbContext Database
        {
            get { return Context as ApplicationDbContext; }
        }

        public ApplicationUserRepository(ApplicationDbContext context)
           : base(context)
        {
            
        }
        public ApplicationUser GetUserById(string id)
        {
            return Database.ApplicationUsers.Find(id);
        }
        public override IEnumerable<ApplicationUser> GetAll()
        {
            return Database.ApplicationUsers.Include(x => x.Images)
                                            .Include(x=>x.Orders)
                                            .Include(x=>x.Posts)
                                            .Include(x=>x.Replies)
                                            .AsEnumerable();
        }
     
        public void UpdateUserWithImage(ApplicationUser applicationUser, Image image)
        {
            Database.ApplicationUsers.Attach(applicationUser);
            Database.Entry(applicationUser).Collection("Images").Load();
            applicationUser.Images.Clear();
            applicationUser.Images = new List<Image>() { image };
        }

        public override void Remove(ApplicationUser applicationUser)
        {
            var q = Database.ApplicationUsers.Include(x => x.Images)
                                             .Include(x => x.Orders)
                                             .Include(x => x.Posts)
                                             .Include(x => x.Replies).FirstOrDefault(x => x.Id == applicationUser.Id);

            Database.ApplicationUsers.Remove(applicationUser);
        }
    }
}
