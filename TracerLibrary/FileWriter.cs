using System.IO;
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
