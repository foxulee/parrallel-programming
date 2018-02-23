using System;
using System.Threading.Tasks;

namespace ParallelProgramming
{
    class Program
    {
        public static void Write1(char c)
        {
            var i = 1000;
            while (i-- > 0)
            {
                Console.Write(c);
            }
        }
        public static void Write2(object o)
        {
            var i = 1000;
            while (i-- > 0)
            {
                Console.Write(o);
            }
        }

        public static int TextLength(object o)
        {
            Console.WriteLine($"\nTask with id {Task.CurrentId} processing object {o}");
            return o.ToString().Length;
        }
        static void Main(string[] args)
        {
            #region Creating and Starting Task
            //In two ways

            //Task.Factory.StartNew(() => Write1('.'));
            //Task.Factory.StartNew(Write2, '1');

            //var task1 = new Task(() => Write1('?'));
            //task1.Start();
            //var task2 = new Task(Write2, '?');
            //task2.Start();

            //string text1 = "test1";
            //string text2 = "tests2";
            //var task3 = Task<int>.Factory.StartNew(TextLength, text1);
            //var task4 = new Task<int>(TextLength, text2);
            //task4.Start();

            //Console.WriteLine($"Length of '{text1}' is {task3.Result}" );
            //Console.WriteLine($"Length of '{text2}' is {task4.Result}" ); 
            #endregion

            #region Canceling Task

            #region Simple cancellation
            //var cts = new CancellationTokenSource();
            //var token = cts.Token;

            ////callback, as soon as the task is canceled
            //token.Register(() =>
            //    {
            //        Console.WriteLine("Cancellation has been requested.");
            //    }
            //);

            //var t = new Task(() =>
            //{
            //    int i = 0;
            //    while (true)
            //    {
            //        //if (token.IsCancellationRequested)
            //        //    //break; //1. soft cancel
            //        //    throw new OperationCanceledException(); //2. hard cancel
            //        //3. combine if statement and throw
            //        token.ThrowIfCancellationRequested();
            //        Console.WriteLine($"{i++}\t");
            //    }
            //}, token);

            //t.Start();

            //Console.ReadLine();
            //cts.Cancel(); 
            #endregion

            #region Composite cancellation

            //var planned = new CancellationTokenSource();
            //var preventative = new CancellationTokenSource();
            //var emergency = new CancellationTokenSource();

            //var paranoid = CancellationTokenSource.CreateLinkedTokenSource(planned.Token, preventative.Token, emergency.Token);
            //var t = Task.Factory.StartNew(() =>
            //{
            //    int i = 0;
            //    while (true)
            //    {
            //        paranoid.Token.ThrowIfCancellationRequested();
            //        Console.WriteLine($"{i++}\t");
            //        Thread.Sleep(300);
            //    }
            //});

            //Console.ReadLine();
            ////any one of three is canceled will trigger paranoid
            //planned.Cancel();
            ////preventative.Cancel();
            ////emergency.Cancel();

            #endregion

            #endregion

            #region Waiting for Time to Pass

            //var cts = new CancellationTokenSource();
            //var token = cts.Token;
            //var t = new Task(() =>
            //{
            //    Console.WriteLine("Press any key to disarm; you have 5 seconds.");
            //    bool canceled = token.WaitHandle.WaitOne(5000);
            //    Console.WriteLine(canceled ? "Bomb disarm" : "BOOM!");
            //}, token);
            //t.Start();

            //Console.ReadLine();
            //cts.Cancel();

            #endregion

            #region Waiting for Tasks
            //var cts = new CancellationTokenSource();
            //var token = cts.Token;

            //var t = new Task(() =>
            //{
            //    Console.WriteLine("I take 5 seconds.");
            //    for (int i = 0; i < 5; i++)
            //    {
            //        token.ThrowIfCancellationRequested();
            //        Thread.Sleep(1000);
            //    }

            //    Console.WriteLine("I am done.");
            //}, token);
            //t.Start();

            //Task t2 = Task.Factory.StartNew(() =>
            //{
            //    Thread.Sleep(3000);
            //}, token);

            //Task.WaitAll(t, t2);

            //Console.WriteLine($"Task t status is {t.Status}");
            //Console.WriteLine($"Task t2 status is {t2.Status}");

            #endregion

            #region Exception Handling

            //An unobserved task exception will not get handled, unless task.Wait() will catch the exceptions
            //var t = Task.Factory.StartNew(() => throw new InvalidOperationException() { Source = "t" });
            //var t2 = Task.Factory.StartNew(() => throw new AccessViolationException() { Source = "t2" });

            //try
            //{
            //    Task.WaitAll(t, t2);
            //}
            //catch (AggregateException ae)
            //{
            //    foreach (var e in ae.InnerExceptions)
            //    {
            //        Console.WriteLine($"Exception {e.GetType()} from {e.Source}");
            //    }
            //}

            #endregion

            Console.WriteLine("Main program done.");
            Console.ReadLine();
        }
    }
}
