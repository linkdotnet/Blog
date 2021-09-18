using System;
using System.Diagnostics;

namespace LinkDotNet.Blog.Domain
{
    [DebuggerDisplay("{Content} with sort order {SortOrder}")]
    public class ProfileInformationEntry : Entity
    {
        private ProfileInformationEntry()
        {
        }

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