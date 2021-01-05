using System.Collections.Generic;

namespace DouBanSpider
{
    public class DouBanOptions
    {
        public string SaveFolder { get; set; }
        
        public IEnumerable<int> Categories { get; set; } 
    }
}