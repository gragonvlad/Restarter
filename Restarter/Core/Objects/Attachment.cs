using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restarter.Core.Objects
{
    public class Attachment
    {
        public string fallback { get; set; }
        public string text { get; set; }
        public List<Field> fields { get; set; }
        public string color { get; set; }
    }
}
