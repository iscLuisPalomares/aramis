using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasProject {
    class ComboBoxVendors {
        public string fsid { get; set; }
        public string fssuppname { get; set; }
        public string fssuppdesc { get; set; }
        public string fssuppcity { get; set; }
        public string fsexportflag { get; set; }
        public string fscontactname { get; set; }
        public string fscontactemail { get; set; }
        public string fscontactphone { get; set; }
        public string fscountry { get; set; }
        public string fsflag { get; set; }
        
        public override string ToString() {
            return fssuppname;
        }
    }
}
