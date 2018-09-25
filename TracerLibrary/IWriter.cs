using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//ready
namespace TracerLibrary
{
     public interface IWriter
     {
          void Write(TraceResult traceResult, IConverter converter);
     }
}
