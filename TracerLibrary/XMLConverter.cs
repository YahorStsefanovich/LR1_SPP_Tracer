using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
//ready
namespace TracerLibrary
{
     public class XMLConverter : IConverter
     {
          private DataContractSerializer xmlConverter;
          private XmlWriterSettings xmlWriterSettings;

          public XMLConverter()
          {
               xmlConverter = new DataContractSerializer(typeof(TraceResult));
               xmlWriterSettings = new XmlWriterSettings
               {
                    Indent = true,
                    IndentChars = "\t"
               };
          }

          public void Convert(TraceResult traceResult, Stream stream)
          {
               using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
               {
                    xmlConverter.WriteObject(xmlWriter, traceResult);
               }
          }
     }
}
