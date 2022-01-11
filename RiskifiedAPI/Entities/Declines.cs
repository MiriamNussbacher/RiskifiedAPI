using System;
using System.Collections.Generic;

namespace RiskifiedAPI.Entities
{

    public class CacheItem
    {
       public List<DeclineRecords> items { get; set; }
    }

    public class DeclineRecords
    {
        public string merchatIdentifier { get; set; }

        public List<DeclineSum> declinesPerUser { get; set; }

    }

    public class DeclineSum
    {
        public string reason { get; set; }
        public int count { get; set; }
    }
}
