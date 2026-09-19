using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public sealed class AboutMeRepository(
    IRepository<ProfileInformationEntry> profileInformationEntryRepository,
    IRepository<Skill> skillRepository,
    IRepository<Talk> talkRepository) : IAboutMeRepository
{
    public ValueTask<IPagedList<ProfileInformationEntry>> GetProfileInformationEntriesAsync() =>
        profileInformationEntryRepository.GetAllAsync(orderBy: entry => entry.SortOrder, descending: false);

    public ValueTask StoreProfileInformationEntryAsync(ProfileInformationEntry entry) =>
        profileInformationEntryRepository.StoreAsync(entry);

    public ValueTask DeleteProfileInformationEntryAsync(string id) =>
        profileInformationEntryRepository.DeleteAsync(id);

    public ValueTask<IPagedList<Skill>> GetSkillsAsync() => skillRepository.GetAllAsync();

    public ValueTask StoreSkillAsync(Skill skill) => skillRepository.StoreAsync(skill);

    public ValueTask DeleteSkillAsync(string id) => skillRepository.DeleteAsync(id);

    public ValueTask<IPagedList<Talk>> GetTalksAsync() =>
        talkRepository.GetAllAsync(orderBy: talk => talk.PublishedDate, descending: true);

    public ValueTask StoreTalkAsync(Talk talk) => talkRepository.StoreAsync(talk);

    public ValueTask DeleteTalkAsync(string id) => talkRepository.DeleteAsync(id);
}
