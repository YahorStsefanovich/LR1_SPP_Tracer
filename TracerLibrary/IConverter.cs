using System.IO;
//ready
namespace TracerLibrary
{
     public interface IConverter
     {
          void Convert(TraceResult traceResult, Stream stream);
     }
}
