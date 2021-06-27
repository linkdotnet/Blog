using System;
using System.Collections.Generic;

namespace LinkDotNet.Domain
{
    public class BlogPost
    {
        public string Id { get; set; }
        
        public string Title { get; set; }

        public string ShortDescription { get; set; }
        

        public string Content { get; set; }

        public DateTime UpdatedDate { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}