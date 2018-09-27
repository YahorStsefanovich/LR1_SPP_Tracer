using Microsoft.VisualStudio.TestTools.UnitTesting;
using TracerLibrary;
using System.Threading;
using System.Collections.Generic;

namespace TracerTest
{
     [TestClass]
     public class TracerTest
     {
          private Tracer tracer;
          private int sleepTime;
          private TraceResult traceResult;
          private const int threadsCount = 3;
          private const int threadsNestedCount = 2;

          public void TestMethod(int sleepTime)
          {
               tracer.StartTrace();
               Thread.Sleep(sleepTime);
               tracer.StopTrace();
          }

          public void TestMethod2(int sleepTime)
          {
               tracer.StartTrace();
               Thread.Sleep(sleepTime * 2);
               tracer.StopTrace();
          }

          public void InnerMethod(int sleepTime)
          {
               tracer.StartTrace();
               Thread.Sleep(sleepTime);
               TestMethod(sleepTime);
               tracer.StopTrace();
          }

          public void NestedMethod(int sleepTime)
          {
               tracer.StartTrace();
               Thread.Sleep(sleepTime);
               InnerMethod(sleepTime);
               tracer.StopTrace();
          }

          public void MultipleThreadMethod(int sleepTime)
          {
               tracer.StartTrace();
               var threads = new List<Thread>();
               for (int i = 0; i < threadsNestedCount; i++)
               {
                    Thread thread = new Thread(()=>TestMethod(sleepTime));
                    threads.Add(thread);
                    thread.Start();
               }
               foreach (var thread in threads)
               {
                    thread.Join();
               }
               TestMethod(sleepTime);
               Thread.Sleep(sleepTime);
               tracer.StopTrace();
          }

          //time of single thread
          [TestMethod]
          public void TimeTestSingleThread()
          {
               tracer = new Tracer();
               sleepTime = 228;
               TestMethod(sleepTime);
               traceResult = tracer.GetTraceResult();
               Assert.IsTrue(traceResult.threads[0].TimeInt >= sleepTime);
          }

          //time of several threads
          [TestMethod]
          public void TimeTestMultiThread()
          {
               tracer = new Tracer();
               sleepTime = 111;

               var threads = new List<Thread>();
               for (int i = 0; i < threadsCount; i++)
               {
                    Thread thread = new Thread(()=>TestMethod(sleepTime));
                    threads.Add(thread);
                    thread.Start();
               }
               foreach (var thread in threads)
               {
                    thread.Join();
               }

               traceResult = tracer.GetTraceResult();
               long actualtime = 0;
               for (int i = 0; i < traceResult.threads.Count; i++)
               {
                    actualtime += traceResult.threads[i].TimeInt;
               }
               Assert.IsTrue(actualtime >= sleepTime * threadsCount);
          }

          //time of nested methods
          [TestMethod]
          public void TimeTestNestedMethods()
          {
               tracer = new Tracer();
               sleepTime = 50;

               NestedMethod(sleepTime);
               traceResult = tracer.GetTraceResult();

               Assert.IsTrue(traceResult.threads[0].TimeInt >= sleepTime * 3);
          }

          //nested threads
          [TestMethod]
          public void TestNestedThreads()
          {
               tracer = new Tracer();
               sleepTime = 80;
               int singlemethods = 0, nestedmethods = 0;

               var threads = new List<Thread>();
               for (int i = 0; i < threadsCount; i++)
               {
                    Thread thread = new Thread(()=>MultipleThreadMethod(sleepTime));
                    threads.Add(thread);
                    thread.Start();
               }
               foreach (var thread in threads)
               {
                    thread.Join();
               }

               traceResult = tracer.GetTraceResult();
               Assert.AreEqual(threadsCount * threadsNestedCount + threadsCount, traceResult.threads.Count);
               for (int i = 0; i < traceResult.threads.Count; i++)
               {
                    Assert.AreEqual(1, traceResult.threads[i].Methods.Count);
                    Assert.AreEqual(nameof(TracerTest), traceResult.threads[i].Methods[0].ClassName);
                    if (traceResult.threads[i].Methods[0].Methodlist.Count != 0)
                    {
                         nestedmethods++;
                         Assert.AreEqual(nameof(MultipleThreadMethod), traceResult.threads[i].Methods[0].MethodName);
                         Assert.AreEqual(nameof(TestMethod), traceResult.threads[i].Methods[0].Methodlist[0].MethodName);
                    }
                    else
                         singlemethods++;
               }
               Assert.AreEqual(threadsCount, nestedmethods);
               Assert.AreEqual(threadsNestedCount * threadsCount, singlemethods);
          }

          //several methods in single thread
          [TestMethod]
          public void TestMultipleMethodsInSingleThread()
          {
               tracer = new Tracer();
               sleepTime = 400;

               TestMethod(sleepTime);
               TestMethod2(sleepTime);
               traceResult = tracer.GetTraceResult();
               Assert.AreEqual(1, traceResult.threads.Count);
               Assert.AreEqual(2, traceResult.threads[0].Methods.Count);
               Assert.IsTrue(traceResult.threads[0].TimeInt >= sleepTime * 3);
               Assert.AreEqual(nameof(TestMethod), traceResult.threads[0].Methods[0].MethodName);
               Assert.AreEqual(nameof(TestMethod2), traceResult.threads[0].Methods[1].MethodName);
          }

          //single method in single thread
          [TestMethod]
          public void TestSingleNestedMethod()
          {
               tracer = new Tracer();
               sleepTime = 1000;

               TestMethod(sleepTime);
               traceResult = tracer.GetTraceResult();

               Assert.AreEqual(1, traceResult.threads.Count);
               Assert.AreEqual(1, traceResult.threads[0].Methods.Count);
               Assert.IsTrue(traceResult.threads[0].TimeInt >= sleepTime);
               Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, traceResult.threads[0].Id);
               Assert.AreEqual(0, traceResult.threads[0].Methods[0].Methodlist.Count);
               Assert.IsTrue(traceResult.threads[0].Methods[0].TimeInt >= sleepTime);
               Assert.AreEqual(nameof(TestMethod), traceResult.threads[0].Methods[0].MethodName);
          }
     }
}

