using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TracerLibrary
{
    public class Tracer : ITracer
    {
          private TraceResult traceResult;

          public Tracer()
          {
               traceResult = new TraceResult();
          }

          public void StartTrace()
          {
               MethodBase nameOfItemMethdod = new StackTrace().GetFrame(1).GetMethod();
          }

          public void StopTrace()
          {

          }

          public TraceResult GetTraceResult()
          {
               return traceResult;
          }
    }
}
