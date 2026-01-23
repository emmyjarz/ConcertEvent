using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcertEvent
{
    public class Venue
    {
        public string? Name;

        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}