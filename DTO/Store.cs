using Seer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seer.DTO
{
    public class Store
    {
        public string userid { get; set; } = "";
        public string passwd { get; set; } = "";
        public string? url_battle { get; set; } = Constant.Host;
        public string? url_match { get; set; } = Constant.MatchHost;

        public Dictionary<int, SeerAccount>? accounts { get; set; } = new();


        public class SeerAccount
        {
            public List<BagStore>? bagsStore {  get; set; } = new();
        }
    }
}
