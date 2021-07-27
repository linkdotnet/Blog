using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using Raven.Client.Documents;

namespace LinkDotNet.Infrastructure.Persistence.RavenDb
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IDocumentStore documentStore;

        public ProfileRepository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public async Task<IList<ProfileInformationEntry>> GetAllAsync()
        {
            using var session = documentStore.OpenAsyncSession();
            return await session.Query<ProfileInformationEntry>().ToListAsync();
        }

        public async Task AddAsync(ProfileInformationEntry entry)
        {
            using var session = documentStore.OpenAsyncSession();
            await session.StoreAsync(entry);
        }

        public async Task DeleteAsync(string id)
        {
            using var session = documentStore.OpenAsyncSession();
            session.Delete(id);
            await session.SaveChangesAsync();
        }
    }
}