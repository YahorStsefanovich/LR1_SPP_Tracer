using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//ready
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
               MethodBase itemMethdod = new StackTrace().GetFrame(1).GetMethod();
               traceResult.StartTrace(Thread.CurrentThread.ManagedThreadId, itemMethdod);
          }

          public void StopTrace()
          {
               traceResult.StopTrace(Thread.CurrentThread.ManagedThreadId); 
          }

          public TraceResult GetTraceResult()
          {
               return traceResult;
          }
    }
}
