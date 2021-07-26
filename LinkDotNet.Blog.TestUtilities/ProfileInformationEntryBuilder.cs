using System;
using LinkDotNet.Domain;

namespace LinkDotNet.Blog.TestUtilities
{
    public class ProfileInformationEntryBuilder
    {
        private string key = "Key";
        private string value = "Value";
        private DateTime? createdDate;

        public ProfileInformationEntryBuilder WithKey(string key)
        {
            this.key = key;
            return this;
        }

        public ProfileInformationEntryBuilder WithValue(string value)
        {
            this.value = value;
            return this;
        }

        public ProfileInformationEntryBuilder WithCreatedDate(DateTime createdDate)
        {
            this.createdDate = createdDate;
            return this;
        }

        public ProfileInformationEntry Build()
        {
            return ProfileInformationEntry.Create(key, value, createdDate);
        }
    }
}