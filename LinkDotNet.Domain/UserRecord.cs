using System;

namespace LinkDotNet.Domain
{
    public class UserRecord : Entity
    {
        public int IpHash { get; set; }

        public DateTime DateTimeUtcClicked { get; set; }

        public string UrlClicked { get; set; }
    }
}