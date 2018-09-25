using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//ready
namespace TracerLibrary
{
     public interface IConverter
     {
          void Convert(TraceResult traceResulkt, Stream stream);
     }
}
