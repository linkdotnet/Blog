using LinkDotNet.Domain;

namespace LinkDotNet.Blog.TestUtilities
{
    public class ProfileInformationEntryBuilder
    {
        private string content = "Content";
        private int sortOrder;

        public ProfileInformationEntryBuilder WithContent(string key)
        {
            this.content = key;
            return this;
        }

        public ProfileInformationEntryBuilder WithSortOrder(int sortOrder)
        {
            this.sortOrder = sortOrder;
            return this;
        }

        public ProfileInformationEntry Build()
        {
            return ProfileInformationEntry.Create(content, sortOrder);
        }
    }
}