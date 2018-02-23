using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace _02DataSharingAndSynchronization
{
    class Program
    {
        //one is to use lock(){}
        class BankAccount1
        {
            private object padlock = new object();
            public int Balance { get; private set; }

            public void Deposit(int amount)
            {

                lock (padlock)
                {
                    // += is really two operations
                    // op1 is temp <- get_Balance() + amount
                    // op2 is set_Balance(temp)
                    // something can happen _between_ op1 and op2

                    Balance += amount;
                }
            }

            public void Withdraw(int amount)
            {
                lock (padlock)
                {
                    Balance -= amount;
                }
            }
        }

        //the second way is to use Interlocked, only useful for atomically changing low-level primitives
        class BankAccount2
        {
            private int balance;

            public int Balance
            {
                get { return balance; }
                private set { balance = value; }
            }

            // interlocked class contains atomic operations on variables
            // atomic = cannot be interrupted
            public void Deposit(int amount)
            {
                Interlocked.Add(ref balance, amount);
            }

            public void Withdraw(int amount)
            {
                Interlocked.Add(ref balance, -amount);
                //balance -= amount;
            }
        }

        //the third way is to use more powerful tool, Mutex
        class BankAccount3
        {
            public int Balance { get; private set; }

            public BankAccount3(int balance)
            {
                Balance = balance;
            }

            public void Deposit(int amount)
            {
                Balance += amount;
            }

            public void Withdraw(int amount)
            {
                Balance -= amount;
            }

            public void Transfer(BankAccount3 where, int amount)
            {
                where.Balance += amount;
                Balance -= amount;
            }
        }

        static void Main(string[] args)
        {
            var tasks0 = new List<Task>();
            var ba0 = new BankAccount2();

            for (int i = 0; i < 10; ++i)
            {
                tasks0.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                        ba0.Deposit(100);
                }));
                tasks0.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                        ba0.Withdraw(100);
                }));
            }

            Task.WaitAll(tasks0.ToArray());

            Console.WriteLine($"Final balance is {ba0.Balance}.");

            // show interlocked methods here

            // Interlocked.MemoryBarrier is a wrapper for Thread.MemoryBarrier
            // only required on memory systems that have weak memory ordering (e.g., Itanium)
            // prevents the CPU from reordering the instructions such that those before the barrier
            // execute after those after

            Console.WriteLine("All done here.");

            Console.WriteLine();

            #region Mutex
            var tasks = new List<Task>();
            var ba = new BankAccount3(0);
            var ba2 = new BankAccount3(0);

            // many synchro types deriving from WaitHandle
            // Mutex = mutual exclusion

            // two types of mutexes
            // this is a _local_ mutex
            Mutex mutex = new Mutex();
            Mutex mutex2 = new Mutex();

            for (int i = 0; i < 10; ++i)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            ba.Deposit(1); // deposit 10000 overall
                        }
                        finally
                        {
                            if (haveLock) mutex.ReleaseMutex();
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool haveLock = mutex2.WaitOne();
                        try
                        {
                            ba2.Deposit(1); // deposit 10000
                        }
                        finally
                        {
                            if (haveLock) mutex2.ReleaseMutex();
                        }
                    }
                }));

                // transfer needs to lock both accounts
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = Mutex.WaitAll(new[] { mutex, mutex2 });
                        try
                        {
                            ba.Transfer(ba2, 1); // transfer 10k from ba to ba2
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                                mutex2.ReleaseMutex();
                            }
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Final balance is: ba={ba.Balance}, ba2={ba2.Balance}.");
            #endregion
        }
    }
}


