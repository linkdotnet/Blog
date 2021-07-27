using System;
using LinkDotNet.Domain;

namespace LinkDotNet.Blog.TestUtilities
{
    public class ProfileInformationEntryBuilder
    {
        private string content = "Content";
        private DateTime? createdDate;

        public ProfileInformationEntryBuilder WithContent(string key)
        {
            this.content = key;
            return this;
        }

        public ProfileInformationEntryBuilder WithCreatedDate(DateTime createdDate)
        {
            this.createdDate = createdDate;
            return this;
        }

        public ProfileInformationEntry Build()
        {
            return ProfileInformationEntry.Create(content, createdDate);
        }
    }
}