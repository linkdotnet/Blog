using System;

namespace LinkDotNet.Domain
{
    public class ProfileInformationEntry
    {
        private ProfileInformationEntry()
        {
        }

        public string Id { get; set; }

        public string Content { get; private set; }

        public int SortOrder { get; set; }

        public static ProfileInformationEntry Create(string key, int sortOrder)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            return new ProfileInformationEntry
            {
                Content = key.Trim(),
                SortOrder = sortOrder,
            };
        }
    }
}