//ready
namespace TracerLibrary
{
     public interface IWriter
     {
          void Write(TraceResult traceResult, IConverter converter);
     }
}
