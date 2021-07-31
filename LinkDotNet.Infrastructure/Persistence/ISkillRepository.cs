using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Domain;

namespace LinkDotNet.Infrastructure.Persistence
{
    public interface ISkillRepository
    {
        Task<IList<Skill>> GetAllAsync();

        Task StoreAsync(Skill skill);

        Task DeleteAsync(string id);
    }
}