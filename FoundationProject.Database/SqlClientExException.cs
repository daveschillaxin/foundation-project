using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FoundationProject.Database
{

    [Serializable]
    public class SqlClientExException
        : Exception
    {

        public SqlClientExException()
            : base()
        {
        }

        public SqlClientExException(
            String message)
            : base(message)
        {
        }

        public SqlClientExException(
            String message, 
            Exception innerException)
            : base(message, innerException)
        {
        }

        protected SqlClientExException(
            SerializationInfo info, 
            StreamingContext context)
            : base(info, context)
        {
        }

    }

}
