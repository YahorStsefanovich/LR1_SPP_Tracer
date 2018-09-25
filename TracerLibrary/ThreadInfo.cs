using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
//ready
namespace TracerLibrary
{
     [Serializable]
     [DataContract]
     public sealed class ThreadInfo
     {
          private int id;
          private long time;
          private List<MethodInfo> methods;
          private Stack<MethodInfo> callMethods;

          [DataMember (Name = "id", Order = 0)]
          public int Id
          {
               get { return id; }
          }

          [DataMember(Name = "id", Order = 1)]
          public string Time
          {
               get { return time.ToString() + "ms"; }
          }

          [XmlIgnore]
          public long TimeInt
          {
               get { return time; }
          }

          [DataMember(Name = "methods", Order =2)]
          public List<MethodInfo> Methods
          {
               get { return methods; }
          }

          public ThreadInfo()
          {
               time = 0;
               methods = new List<MethodInfo>();
               callMethods = new Stack<MethodInfo>();
          }

          public ThreadInfo(int threadID)
          {
               id = threadID;
               time = 0;
               methods = new List<MethodInfo>();
               callMethods = new Stack<MethodInfo>();
          }

          public void StartTrace(MethodInfo method)
          {
               if (callMethods.Count == 0)
               {
                    methods.Add(method);
               }
               else
               {
                    callMethods.Peek().AddNestedMethod(method);
               }

               callMethods.Push(method);
               method.StartTrace();

          }

          public void StopTrace()
          {
               MethodInfo lastMethod = callMethods.Peek();
               lastMethod.StopTrace();
               if (callMethods.Count == 1)
               {
                    time += lastMethod.TimeInt;
               }

               callMethods.Pop();
          }
     }
}
