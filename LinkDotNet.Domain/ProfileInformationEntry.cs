using System;

namespace LinkDotNet.Domain
{
    public class ProfileInformationEntry
    {
        private ProfileInformationEntry()
        {
        }

        public string Id { get; set; }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public DateTime CreatedDate { get; private set; }

        public static ProfileInformationEntry Create(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            return new ProfileInformationEntry
            {
                Key = key.Trim(),
                Value = value.Trim(),
                CreatedDate = DateTime.Now,
            };
        }
    }
}