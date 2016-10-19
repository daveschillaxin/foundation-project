using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundationProject.Reporting
{

    public struct Parameter
    {

        public string Name;
        public object Value;

        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

    }

}
