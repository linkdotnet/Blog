using System.Collections.Generic;
using LinkDotNet.Domain;

namespace LinkDotNet.Blog.Web.Shared
{
    public class SortOrderCalculator : ISortOrderCalculator
    {
        public int GetSortOrder(ProfileInformationEntry target, IEnumerable<ProfileInformationEntry> all)
        {
            var linkedEntries = new LinkedList<ProfileInformationEntry>(all);
            var targetNode = linkedEntries.Find(target);
            var next = targetNode!.Next;

            if (next == null)
            {
                var prev = targetNode.Previous;
                return (target.SortOrder + prev!.Value.SortOrder) / 2;
            }

            return (target.SortOrder + next.Value.SortOrder) / 2;
        }
    }
}