using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasProject {
    class CBCargos {

        public string fscodigo { get; set; }
        public string fsdesc { get; set; }

        public override string ToString() {
            return fsdesc;
        }
    }
}
