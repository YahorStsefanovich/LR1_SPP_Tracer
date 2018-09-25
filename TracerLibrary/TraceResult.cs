using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TracerLibrary
{
     [Serializable]
     [DataContract]
     public class TraceResult
     {
          private ConcurrentDictionary<int, ThreadInfo> threadsList;


     }
}
