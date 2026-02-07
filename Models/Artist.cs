using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcertEvent.Models
{
    public class Artist
    {
        public string? Name;

        public object Id { get; internal set; }

        public override string ToString()
        {
            // warning CS8603: Possible null reference return. for return Name;
            return Name ?? string.Empty;
        }
    }
}