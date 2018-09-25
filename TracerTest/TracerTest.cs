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
          private int sleeptime;
          private TraceResult traceResult;
          private const int threadscount = 3;
          private const int threadsnestedcount = 2;

          public void TestMethod()
          {
               tracer.StartTrace();
               Thread.Sleep(sleeptime);
               tracer.StopTrace();
          }

          public void TestMethod2()
          {
               tracer.StartTrace();
               Thread.Sleep(sleeptime * 2);
               tracer.StopTrace();
          }

          public void Innermethod()
          {
               tracer.StartTrace();
               Thread.Sleep(sleeptime);
               TestMethod();
               tracer.StopTrace();
          }

          public void NestedMethod()
          {
               tracer.StartTrace();
               Thread.Sleep(sleeptime);
               Innermethod();
               tracer.StopTrace();
          }

          public void MultipleThreadMethod()
          {
               tracer.StartTrace();
               var threads = new List<Thread>();
               for (int i = 0; i < threadsnestedcount; i++)
               {
                    Thread thread = new Thread(TestMethod);
                    threads.Add(thread);
                    thread.Start();
               }
               foreach (var thread in threads)
               {
                    thread.Join();
               }
               TestMethod();
               Thread.Sleep(sleeptime);
               tracer.StopTrace();
          }

          //время одного потока
          [TestMethod]
          public void TimeTestSingleThread()
          {
               tracer = new Tracer();
               sleeptime = 228;
               TestMethod();
               traceResult = tracer.GetTraceResult();
               Assert.IsTrue(traceResult.threads[0].TimeInt >= sleeptime);
          }

          //время нескольких потоков
          [TestMethod]
          public void TimeTestMultiThread()
          {
               tracer = new Tracer();
               sleeptime = 111;

               var threads = new List<Thread>();
               for (int i = 0; i < threadscount; i++)
               {
                    Thread thread = new Thread(TestMethod);
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
               Assert.IsTrue(actualtime >= sleeptime * threadscount);
          }

          //время вложенных методов
          [TestMethod]
          public void TimeTestNestedMethods()
          {
               tracer = new Tracer();
               sleeptime = 50;

               NestedMethod();
               traceResult = tracer.GetTraceResult();

               Assert.IsTrue(traceResult.threads[0].TimeInt >= sleeptime * 3);
          }

          //вложенные потоки
          [TestMethod]
          public void TestNestedThreads()
          {
               tracer = new Tracer();
               sleeptime = 80;
               int singlemethods = 0, nestedmethods = 0;

               var threads = new List<Thread>();
               for (int i = 0; i < threadscount; i++)
               {
                    Thread thread = new Thread(MultipleThreadMethod);
                    threads.Add(thread);
                    thread.Start();
               }
               foreach (var thread in threads)
               {
                    thread.Join();
               }

               traceResult = tracer.GetTraceResult();
               Assert.AreEqual(threadscount * threadsnestedcount + threadscount, traceResult.threads.Count);
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
               Assert.AreEqual(threadscount, nestedmethods);
               Assert.AreEqual(threadsnestedcount * threadscount, singlemethods);
          }

          //несколько методов в одном потоке(не вложенных)
          [TestMethod]
          public void TestMultipleMethodsInSingleThread()
          {
               tracer = new Tracer();
               sleeptime = 400;

               TestMethod();
               TestMethod2();
               traceResult = tracer.GetTraceResult();

               Assert.AreEqual(1, traceResult.threads.Count);
               Assert.AreEqual(2, traceResult.threads[0].Methods.Count);
               Assert.IsTrue(traceResult.threads[0].TimeInt >= sleeptime * 3);
               Assert.AreEqual(nameof(TestMethod), traceResult.threads[0].Methods[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceResult.threads[0].Methods[0].ClassName);
               Assert.AreEqual(nameof(TestMethod2), traceResult.threads[0].Methods[1].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceResult.threads[0].Methods[1].ClassName);
          }

          //один метод в одном потоке
          [TestMethod]
          public void TestSingleNestedMethod()
          {
               tracer = new Tracer();
               sleeptime = 666;

               TestMethod();
               traceResult = tracer.GetTraceResult();

               Assert.AreEqual(1, traceResult.threads.Count);//количесво потоков
               Assert.AreEqual(1, traceResult.threads[0].Methods.Count);//количесво методов в потоке
               Assert.IsTrue(traceResult.threads[0].TimeInt >= sleeptime);//время потока
               Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, traceResult.threads[0].Id);
               Assert.AreEqual(0, traceResult.threads[0].Methods[0].Methodlist.Count);//количесво методов в Testmethod
               Assert.IsTrue(traceResult.threads[0].Methods[0].TimeInt >= sleeptime);//время NestedMethod
               Assert.AreEqual(nameof(TestMethod), traceResult.threads[0].Methods[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceResult.threads[0].Methods[0].ClassName);
          }

          //нескольоко вложенных методов в одном потоке
          [TestMethod]
          public void TestMultipleNestedMethods()
          {
               tracer = new Tracer();
               sleeptime = 312;

               NestedMethod();
               traceResult = tracer.GetTraceResult();

               Assert.AreEqual(1, traceResult.threads.Count);//количесво потоков
               Assert.AreEqual(1, traceResult.threads[0].Methods.Count);//количесво методов в потоке
               Assert.IsTrue(traceResult.threads[0].TimeInt >= sleeptime * 3);//время потока
               Assert.AreEqual(1, traceResult.threads[0].Methods[0].Methodlist.Count);//количесво методов в NestedMethod
               Assert.IsTrue(traceResult.threads[0].Methods[0].TimeInt >= sleeptime * 3);//время NestedMethod
               Assert.AreEqual(nameof(NestedMethod), traceResult.threads[0].Methods[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceResult.threads[0].Methods[0].ClassName);
               Assert.AreEqual(1, traceResult.threads[0].Methods[0].Methodlist[0].Methodlist.Count);//количесво методов в InnerMethod
               Assert.IsTrue(traceResult.threads[0].Methods[0].Methodlist[0].TimeInt >= sleeptime * 2);//время InnerMethod
               Assert.AreEqual(nameof(Innermethod), traceResult.threads[0].Methods[0].Methodlist[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceResult.threads[0].Methods[0].Methodlist[0].ClassName);
               Assert.AreEqual(0, traceResult.threads[0].Methods[0].Methodlist[0].Methodlist[0].Methodlist.Count);//количесво методов в TestMethod
               Assert.IsTrue(traceResult.threads[0].Methods[0].Methodlist[0].Methodlist[0].TimeInt >= sleeptime);//время TestMethod
               Assert.AreEqual(nameof(TestMethod), traceResult.threads[0].Methods[0].Methodlist[0].Methodlist[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceResult.threads[0].Methods[0].Methodlist[0].Methodlist[0].ClassName);
          }
     }
}

