using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seer.DTO
{
    public class BagStore
    {
        public string? label { get; set; }
        public List<Pet>? pets { get; set; }

        public BagStore() { }


        public BagStore(string? label, List<Pet>? pets)
        {
            this.label = label;
            this.pets = pets;
        }
    }
}
