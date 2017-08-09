using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasProject {
    class ComboBoxBucketsAccounts {
        public string fsid { get; set; }
        public string fsacctnumber { get; set; }
        public string fsacctdesc { get; set; }
        public string fsdepto { get; set; }
        public string fsdeptoid { get; set; }

        public override string ToString() {
            return fsacctnumber + " - " + fsacctdesc + " - " + fsdepto;
        }
    }
}
