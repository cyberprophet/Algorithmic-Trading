using System.Diagnostics;

namespace ShareInvest
{
    public class Delay
    {
        public static Delay GetInstance(int milliseconds)
        {
            if (Request is null)
                Request = new Delay();

            Milliseconds = milliseconds;

            return Request;
        }
        public void Run()
        {
            if (System.Threading.ThreadState.Unstarted.Equals(worker.ThreadState))
                worker.Start();
        }
        public void Dispose()
        {
            task.Clear();
            source.Cancel();
        }
        public void RequestTheMission(Task task) => this.task.Enqueue(task);
        Delay()
        {
            task = new Queue<Task>();
            source = new CancellationTokenSource();
            worker = new Thread(delegate ()
            {
                while (source.Token.IsCancellationRequested is false)
                {
                    while (this.task.TryDequeue(out Task? task))
                        try
                        {
                            task.RunSynchronously();
                            Thread.Sleep(Milliseconds);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    Thread.Sleep(0xA);
                }
            });
        }
        static int Milliseconds
        {
            get; set;
        }
        static Delay? Request
        {
            get; set;
        }
        readonly Thread worker;
        readonly Queue<Task> task;
        readonly CancellationTokenSource source;
    }
}