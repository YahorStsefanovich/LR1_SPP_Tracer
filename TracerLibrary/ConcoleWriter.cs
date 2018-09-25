using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TracerLibrary
{
     class ConcoleWriter : IWriter
     {
          public void Write(TraceResult traceResult, IConverter converter)
          {
               using (Stream consoleStream = Console.OpenStandardOutput())
               {
                    converter.Convert(traceResult, consoleStream);
               }

          }
     }
}
