using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seer.DTO
{
    public class Pet
    {
        public int id { get; set; }
        public int catchTime { get; set; }
        public int level { get; set; }
        public int effectID { get; set; }
        public List<int>? marks { get; set; }
        public List<Skill>? skillArray { get; set; }
        public Skill? hideSkill { get; set; }
        public int state { get; set; }
        public List<Mark>? bindMarks { get; set; }
        


        public class Skill
        {
            public int _id { get; set; }
            public int pp { get; set; }
        }

        public class Mark
        {
            public int _markID { get; set; }
            public int _bindMoveID { get; set; }
            public int _obtainTime { get; set; }
        }
    }
}
