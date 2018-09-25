using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
//ready
namespace TracerLibrary
{
     [Serializable]
     [DataContract]
     public class TraceResult
     {
          private ConcurrentDictionary<int, ThreadInfo> threadsList;

          [DataMember(Name = "threads")]
          public List<ThreadInfo> threads
          {
               get
               {
                    SortedDictionary<int, ThreadInfo> sortedDictionary =
                         new SortedDictionary<int, ThreadInfo>();
                    return new List<ThreadInfo>(sortedDictionary.Values);
               }
          } 

          public TraceResult()
          {
               threadsList = new ConcurrentDictionary<int, ThreadInfo>();
          }

          public void StartTrace(int id, MethodBase method)
          {
               ThreadInfo threadInfo = threadsList.GetOrAdd(id, new ThreadInfo(id));
               threadInfo.StartTrace(new MethodInfo(method));
          }

          public void StopTrace(int id)
          {
               ThreadInfo threadInfo;
               if (! threadsList.TryGetValue(id, out threadInfo))
               {
                    throw new ArgumentException("Invalid thread ID");
               }
               threadInfo.StopTrace();
          }

     }
}
