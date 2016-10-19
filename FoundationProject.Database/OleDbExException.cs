using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FoundationProject.Database
{

    [Serializable]
    public class OleDbExException
        : Exception
    {

        public OleDbExException()
            : base()
        {
        }

        public OleDbExException(
            String message)
            : base(message)
        {
        }

        public OleDbExException(
            String message, 
            Exception innerException)
            : base(message, innerException)
        {
        }

        protected OleDbExException(
            SerializationInfo info, 
            StreamingContext context)
            : base(info, context)
        {
        }

    }

}
