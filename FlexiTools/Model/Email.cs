using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexiTools.Model
{
    internal class Email
    {
        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public List<Anexos> Anexos { get; set; }
    }
}
