using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static System.Convert;

namespace CancellingTasks
{
    class Program
    {
        public static async Task Main()
        {
            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            var t1 = Task.Run(() =>
            {
                ct.ThrowIfCancellationRequested();                              //in case cancellation was already requested before

                bool doNotClose = true;
                while (doNotClose)
                {
                    WriteLine("Executing Thread...");

                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                }
            }, tokenSource.Token);

            for (int i = 0; i < 5; i++)
            {
                if (i == 4)
                {
                    tokenSource.Cancel();
                }
                Thread.Sleep(1000);
            }

            try
            {
                await t1;
            }
            catch(OperationCanceledException)
            {
                WriteLine("Operation was cancelled.");
            }
            catch(Exception ex)
            {
                WriteLine("Unpredicted exception has occured, closing app.");
                Thread.Sleep(2000);
                Environment.Exit(0);
            }
            finally
            {
                tokenSource.Dispose();
            }
            
            ReadKey();
        }
    }
}
