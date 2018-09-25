using TracerLibrary;
//ready
namespace LR1_SPP
{
     class Program
     {
          static Tracer tracer = new Tracer(); 

          static void Main(string[] args)
          {
               TraceResult result;
               Examples examples = new Examples(tracer);
               examples.StartTest();
               result = tracer.GetTraceResult();

               string jsonFileName = "jsonFile.dat";
               string xmlFileName = "xmlFile.dat";

               LoadToFile(new JsonConverter(), result, jsonFileName);
               LoadToFile(new XMLConverter(), result, xmlFileName);
               LoadToConsole(new JsonConverter(), result);
          }

          static void LoadToFile(IConverter converter, 
               TraceResult result, string fileName)
          {
               IWriter writer = new FileWriter(fileName);
               writer.Write(result, converter);
          }

          static void LoadToConsole(IConverter converter, TraceResult result)
          {
               IWriter writer = new ConsoleWriter();
               writer.Write(result, converter);
          }
     }
}
