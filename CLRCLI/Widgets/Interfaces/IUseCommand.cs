using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    interface IUseCommand
    {
        [XmlAttribute]
        string Command { get; set; }
    }
}
