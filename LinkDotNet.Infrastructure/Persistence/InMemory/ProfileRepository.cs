using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Domain;

namespace LinkDotNet.Infrastructure.Persistence.InMemory
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly List<ProfileInformationEntry> profileInformation = new();

        public Task<IList<ProfileInformationEntry>> GetAllAsync()
        {
            return Task.FromResult((IList<ProfileInformationEntry>)profileInformation.ToList());
        }

        public Task StoreAsync(ProfileInformationEntry entry)
        {
            if (string.IsNullOrEmpty(entry.Id))
            {
                entry.Id = profileInformation.Max(b => b.Id) + 1;
            }

            profileInformation.Add(entry);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var entry = profileInformation.SingleOrDefault(p => p.Id == id);
            if (entry != null)
            {
                profileInformation.Remove(entry);
            }

            return Task.CompletedTask;
        }
    }
}