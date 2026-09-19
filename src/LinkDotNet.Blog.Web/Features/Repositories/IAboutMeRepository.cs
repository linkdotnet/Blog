using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public interface IAboutMeRepository
{
    ValueTask<IPagedList<ProfileInformationEntry>> GetProfileInformationEntriesAsync();

    ValueTask StoreProfileInformationEntryAsync(ProfileInformationEntry entry);

    ValueTask DeleteProfileInformationEntryAsync(string id);

    ValueTask<IPagedList<Skill>> GetSkillsAsync();

    ValueTask StoreSkillAsync(Skill skill);

    ValueTask DeleteSkillAsync(string id);

    ValueTask<IPagedList<Talk>> GetTalksAsync();

    ValueTask StoreTalkAsync(Talk talk);

    ValueTask DeleteTalkAsync(string id);
}
