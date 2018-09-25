//ready
namespace TracerLibrary
{
     public interface ITracer
     {
          void StartTrace();

          void StopTrace();

          TraceResult GetTraceResult();
     }
}
