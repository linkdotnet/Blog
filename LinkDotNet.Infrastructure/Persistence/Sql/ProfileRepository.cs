using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Infrastructure.Persistence.Sql
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly BlogDbContext dbContext;

        public ProfileRepository(BlogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IList<ProfileInformationEntry>> GetAllAsync()
        {
            return await dbContext.ProfileInformationEntries.ToListAsync();
        }

        public async Task AddAsync(ProfileInformationEntry entry)
        {
            await dbContext.ProfileInformationEntries.AddAsync(entry);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entryToDelete = await dbContext.ProfileInformationEntries.SingleOrDefaultAsync(d => d.Id == id);
            if (entryToDelete == null)
            {
                return;
            }

            dbContext.ProfileInformationEntries.Remove(entryToDelete);
            await dbContext.SaveChangesAsync();
        }
    }
}