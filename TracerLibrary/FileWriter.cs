using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//ready
namespace TracerLibrary
{
     public class FileWriter : IWriter
     {
          private string fileName;

          public FileWriter(string FileName)
          {
               this.fileName = FileName;
          }

          public void Write(TraceResult result, IConverter converter)
          {
               using (FileStream fs = new FileStream(fileName, FileMode.Create))
               {
                    converter.Convert(result, fs);
               }
          }
     }
}
