using System.IO;
//ready
namespace TracerLibrary
{
     public interface IConverter
     {
          void Convert(TraceResult traceResulkt, Stream stream);
     }
}
