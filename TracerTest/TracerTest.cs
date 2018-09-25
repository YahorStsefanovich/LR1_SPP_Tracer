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
          private TraceResult traceresult;
          private const int threadsCount = 3;
          private const int threadsNestedCount = 2;

          public void TestMethod()
          {
               tracer.StartTrace();
               Thread.Sleep(sleepTime);
               tracer.StopTrace();
          }

          public void TestMethod2()
          {
               tracer.StartTrace();
               Thread.Sleep(sleepTime * 2);
               tracer.StopTrace();
          }

          public void Innermethod()
          {
               tracer.StartTrace();
               Thread.Sleep(sleepTime);
               TestMethod();
               tracer.StopTrace();
          }

          public void NestedMethod()
          {
               tracer.StartTrace();
               Thread.Sleep(sleepTime);
               Innermethod();
               tracer.StopTrace();
          }

          public void MultipleThreadMethod()
          {
               tracer.StartTrace();
               var threads = new List<Thread>();
               for (int i = 0; i < threadsNestedCount; i++)
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
               Thread.Sleep(sleepTime);
               tracer.StopTrace();
          }

          //время одного потока
          [TestMethod]
          public void TimeTestSingleThread()
          {
               tracer = new Tracer();
               sleepTime = 228;
               TestMethod();
               traceresult = tracer.GetTraceResult();
               Assert.IsTrue(traceresult.threads[0].TimeInt >= sleepTime);
          }

          //время нескольких потоков
          [TestMethod]
          public void TimeTestMultiThread()
          {
               tracer = new Tracer();
               sleepTime = 111;

               var threads = new List<Thread>();
               for (int i = 0; i < threadsCount; i++)
               {
                    Thread thread = new Thread(TestMethod);
                    threads.Add(thread);
                    thread.Start();
               }
               foreach (var thread in threads)
               {
                    thread.Join();
               }

               traceresult = tracer.GetTraceResult();
               long actualtime = 0;
               for (int i = 0; i < traceresult.threads.Count; i++)
               {
                    actualtime += traceresult.threads[i].TimeInt;
               }
               Assert.IsTrue(actualtime >= sleepTime * threadsCount);
          }

          //время вложенных методов
          [TestMethod]
          public void TimeTestNestedMethods()
          {
               tracer = new Tracer();
               sleepTime = 50;

               NestedMethod();
               traceresult = tracer.GetTraceResult();

               Assert.IsTrue(traceresult.threads[0].TimeInt >= sleepTime * 3);
          }

          //вложенные потоки
          [TestMethod]
          public void TestNestedThreads()
          {
               tracer = new Tracer();
               sleepTime = 80;
               int singlemethods = 0, nestedmethods = 0;

               var threads = new List<Thread>();
               for (int i = 0; i < threadsCount; i++)
               {
                    Thread thread = new Thread(MultipleThreadMethod);
                    threads.Add(thread);
                    thread.Start();
               }
               foreach (var thread in threads)
               {
                    thread.Join();
               }

               traceresult = tracer.GetTraceResult();
               Assert.AreEqual(threadsCount * threadsNestedCount + threadsCount, traceresult.threads.Count);
               for (int i = 0; i < traceresult.threads.Count; i++)
               {
                    Assert.AreEqual(1, traceresult.threads[i].Methods.Count);
                    Assert.AreEqual(nameof(TracerTest), traceresult.threads[i].Methods[0].ClassName);
                    if (traceresult.threads[i].Methods[0].MethodList.Count != 0)
                    {
                         nestedmethods++;
                         Assert.AreEqual(nameof(MultipleThreadMethod), traceresult.threads[i].Methods[0].MethodName);
                         Assert.AreEqual(nameof(TestMethod), traceresult.threads[i].Methods[0].MethodList[0].MethodName);
                    }
                    else
                         singlemethods++;
               }
               Assert.AreEqual(threadsCount, nestedmethods);
               Assert.AreEqual(threadsNestedCount * threadsCount, singlemethods);
          }

          //несколько методов в одном потоке(не вложенных)
          [TestMethod]
          public void TestMultipleMethodsInSingleThread()
          {
               tracer = new Tracer();
               sleepTime = 400;

               TestMethod();
               TestMethod2();
               traceresult = tracer.GetTraceResult();

               Assert.AreEqual(1, traceresult.threads.Count);
               Assert.AreEqual(2, traceresult.threads[0].Methods.Count);
               Assert.IsTrue(traceresult.threads[0].TimeInt >= sleepTime * 3);
               Assert.AreEqual(nameof(TestMethod), traceresult.threads[0].Methods[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceresult.threads[0].Methods[0].ClassName);
               Assert.AreEqual(nameof(TestMethod2), traceresult.threads[0].Methods[1].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceresult.threads[0].Methods[1].ClassName);
          }

          //один метод в одном потоке
          [TestMethod]
          public void TestSingleNestedMethod()
          {
               tracer = new Tracer();
               sleepTime = 666;

               TestMethod();
               traceresult = tracer.GetTraceResult();

               Assert.AreEqual(1, traceresult.threads.Count);//количесво потоков
               Assert.AreEqual(1, traceresult.threads[0].Methods.Count);//количесво методов в потоке
               Assert.IsTrue(traceresult.threads[0].TimeInt >= sleepTime);//время потока
               Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, traceresult.threads[0].Id);
               Assert.AreEqual(0, traceresult.threads[0].Methods[0].MethodList.Count);//количесво методов в Testmethod
               Assert.IsTrue(traceresult.threads[0].Methods[0].TimeInt >= sleepTime);//время NestedMethod
               Assert.AreEqual(nameof(TestMethod), traceresult.threads[0].Methods[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceresult.threads[0].Methods[0].ClassName);
          }

          //нескольоко вложенных методов в одном потоке
          [TestMethod]
          public void TestMultipleNestedMethods()
          {
               tracer = new Tracer();
               sleepTime = 312;

               NestedMethod();
               traceresult = tracer.GetTraceResult();

               Assert.AreEqual(1, traceresult.threads.Count);//количесво потоков
               Assert.AreEqual(1, traceresult.threads[0].Methods.Count);//количесво методов в потоке
               Assert.IsTrue(traceresult.threads[0].TimeInt >= sleepTime * 3);//время потока
               Assert.AreEqual(1, traceresult.threads[0].Methods[0].MethodList.Count);//количесво методов в NestedMethod
               Assert.IsTrue(traceresult.threads[0].Methods[0].TimeInt >= sleepTime * 3);//время NestedMethod
               Assert.AreEqual(nameof(NestedMethod), traceresult.threads[0].Methods[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceresult.threads[0].Methods[0].ClassName);
               Assert.AreEqual(1, traceresult.threads[0].Methods[0].MethodList[0].MethodList.Count);//количесво методов в InnerMethod
               Assert.IsTrue(traceresult.threads[0].Methods[0].MethodList[0].TimeInt >= sleepTime * 2);//время InnerMethod
               Assert.AreEqual(nameof(Innermethod), traceresult.threads[0].Methods[0].MethodList[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceresult.threads[0].Methods[0].MethodList[0].ClassName);
               Assert.AreEqual(0, traceresult.threads[0].Methods[0].MethodList[0].MethodList[0].MethodList.Count);//количесво методов в TestMethod
               Assert.IsTrue(traceresult.threads[0].Methods[0].MethodList[0].MethodList[0].TimeInt >= sleepTime);//время TestMethod
               Assert.AreEqual(nameof(TestMethod), traceresult.threads[0].Methods[0].MethodList[0].MethodList[0].MethodName);
               Assert.AreEqual(nameof(TracerTest), traceresult.threads[0].Methods[0].MethodList[0].MethodList[0].ClassName);
          }
     }

}

