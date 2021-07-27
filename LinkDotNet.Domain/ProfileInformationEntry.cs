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

        public DateTime CreatedDate { get; private set; }

        public static ProfileInformationEntry Create(string key, DateTime? createdDate = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            return new ProfileInformationEntry
            {
                Content = key.Trim(),
                CreatedDate = createdDate ?? DateTime.Now,
            };
        }
    }
}