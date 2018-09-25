using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
//ready
namespace TracerLibrary
{
     public class JsonConverter : IConverter
     {
          private DataContractJsonSerializer jsonFormatter;

          public JsonConverter()
          {
               jsonFormatter = new DataContractJsonSerializer(typeof(TraceResult));
          }

          public void Convert(TraceResult traceResult, Stream stream)
          {
               using (XmlDictionaryWriter jsonWrite =
                    JsonReaderWriterFactory.CreateJsonWriter(
                         stream,
                         Encoding.UTF8, 
                         ownsStream: true,
                         indent : true, 
                         indentChars : "/t" 
                         ))
               {
                    jsonFormatter.WriteObject(jsonWrite, traceResult);
               }
          }
     }
}
