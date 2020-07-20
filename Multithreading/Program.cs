using System;
using System.Threading;

namespace MultiThreadingTasks
{
    public class BeesAndBear
    {
        private Mutex beeLock = new Mutex();
        private Mutex bearLock = new Mutex();
        private int H { get; set; }
        private int h { get; set; }

        private void Bee(object threadNum)
        {
            beeLock.WaitOne();
            lock (beeLock)
            {
                h++;
                Console.WriteLine("Bee " + h + " " + threadNum);
                bearLock.WaitOne();
                if (h == H)
                {
                    lock (bearLock)
                    {
                    
                        Console.WriteLine("Bear " + h + " " + threadNum);
                        h = 0;
                        bearLock.ReleaseMutex();
                        beeLock.ReleaseMutex();
                        Thread.CurrentThread.Abort();
                    }
                }
                bearLock.ReleaseMutex();
            }
            beeLock.ReleaseMutex();
        }

        private void BeeLife(object threadNum)
        {
            while (true)
            {
                Bee(threadNum);
            }
        }

        public void ExecuteBeesAndBear(int _H, int _n)
        {
            int n = _n;
            h = 0;
            H = _H;
            try
            {
                for (int i = 0; i < n; i++)
                {
                    new Thread(new ParameterizedThreadStart(BeeLife)).Start(i);
                }
            }
            catch (Exception e)
            {

            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Задача о медведе и пчелах
            Есть N пчел и медведь. Они пользуются одним горшком меда, 
            вмещающим H порций меда. Сначала горшок пустой. Пока горшок 
            не наполнится, медведь спит, потом съедает весь мед и засыпает. 
            Каждая пчела многократно собирает по одной порции меда и 
            кладет ее в горшок. Пчела, которая приносит последнюю порцию 
            меда и заполняет горшок, будит медведя. Представьте медведя и 
            пчел процессами, разработайте код, моделирующий их действия.

            Доп. условие: во избежание бесконечности процесса, медведь жрет пчел :)
            */
            var task1 = new BeesAndBear();
            task1.ExecuteBeesAndBear(7, 10);
        }
    }
}