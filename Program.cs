using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.IO;
using System.IO.Compression;
namespace _15_1laba
{
   
    class Program
    {
        static FileInfo file = new FileInfo("file.txt");
        static AutoResetEvent waitHandler = new AutoResetEvent(false);//??
        static AutoResetEvent waitHandler2 = new AutoResetEvent(false);
        static int w = 0;
        static void Procecs()
        {
            foreach (Process process in Process.GetProcesses())
            {
                using (StreamWriter sw = new StreamWriter("info.txt", true, System.Text.Encoding.Default))
                {
                    sw.WriteLine($"ID: {process.Id}  Name: {process.ProcessName}   Приоритет: {process.BasePriority} Объем памяти: {process.VirtualMemorySize64}");
                    sw.Close();
                    Console.WriteLine($"ID: {process.Id}  Name: {process.ProcessName}   Приоритет: {process.BasePriority} Объем памяти: {process.VirtualMemorySize64}");

                }
            }
        }
        static void Domain()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            Console.WriteLine("Name: "+ domain.FriendlyName);
            Console.WriteLine("Configuration: " + domain.ShadowCopyFiles);
            Console.WriteLine("Assemblies: ");
            Assembly[] assemblies = domain.GetAssemblies();
            foreach(Assembly asm in assemblies)
            {
                Console.WriteLine(asm.GetName().Name);
            }
            Console.WriteLine();
            Console.WriteLine("Create Domain");
            AppDomain my_domain = AppDomain.CreateDomain("My_domain");
            my_domain.Load(new AssemblyName("15_1laba"));
            Console.WriteLine("Name: " + my_domain.FriendlyName);
            Assembly[] assemblies_1 = my_domain.GetAssemblies();
            foreach (var x in assemblies_1)
            {
                Console.WriteLine("name " + x.GetName().Name);
                Thread.Sleep(20);

            }   
            AppDomain.Unload(my_domain);
        }
        public static void ToConsoleFile(object x)
        {
            FileInfo file = new FileInfo("file.txt");
            int n = (int)x;
            for (int i = 1; i <= n; i++)
            {

                Console.Write("Второстепенный поток: " + i + "\n");
                using (StreamWriter writer = new StreamWriter(file.ToString(), true, System.Text.Encoding.Default))
                {
                    writer.Write(i + " ");
                }
                Thread.Sleep(300);//priostanavlivaet potok na zadannyy millisekunt
            }
        }
        public static void Chet(object x)//why object
        {
            int n = (int)x;
            for (int i = 1; i < n; i++)
            {
                if ((i % 2) != 0)
                {
                    Console.WriteLine("First Thread: " + i);
                    Thread.Sleep(300);

                    using (StreamWriter writer = new StreamWriter(file.ToString(), true, System.Text.Encoding.Default))
                    {
                        writer.Write("First Thread: " + i + " ");
                    }
                   waitHandler.WaitOne();
                    waitHandler2.Set();
                } 
            }
           // waitHandler.Set(); // 4.1)
        }

        public static void Nechet(object x)
        {   
            //waitHandler.WaitOne(); // 4.1) 
            int n = (int)x;
            for (int i = 1; i < n; i++)
            {
                if ((i % 2) == 0)
                {
                    Console.WriteLine("Second Thread: " + i);
                    Thread.Sleep(500);
                    using (StreamWriter writer = new StreamWriter(file.ToString(), true, System.Text.Encoding.Default))
                    {
                        writer.Write(" Second Thread: " + i + " ");
                    }
                    waitHandler.Set();
                   waitHandler2.WaitOne();
                }

            }
        }
        public static void TimerSec(object obj)
        {
            w++;
            int x = (int)obj;
            Console.WriteLine("Таймер: {0}", w);
            if (w == x)
            {
                Console.WriteLine("Время вышло!");
                
                Console.Beep();

            }
        }
        static void Main(string[] args)
        {
           
            Domain();

            Console.WriteLine("\nЗАДАНИЕ 3");
            //работаем с потоками
            Console.WriteLine("Введите n: ");
            int n = int.Parse(Console.ReadLine());
            Thread mythread = new Thread(new ParameterizedThreadStart(ToConsoleFile));//делегат threadstart
            mythread.Start(n);
            mythread.Name = "MyThread00";
            Console.WriteLine("Поток: " + mythread.Name + " Состояние: " + mythread.ThreadState + " Приоритет: " + mythread.Priority);
            for (int i = 1; i <= n; i++)
            {

                Console.WriteLine("Главный поток: " + i);//glavnyy potok eto main
                Thread.Sleep(500);//priostanavlivaem glavn potok na 500millisekunt
                mythread.Suspend();//priostanavlivaem nash potok
            }
            mythread.Resume();//kak zakoncirsya gl potok , nash potok vozobnavlyaetsya
            Thread.Sleep(1500);
            Console.WriteLine("ЗАДАНИЕ 4");
            Console.WriteLine("Введите x: ");
            int x = int.Parse(Console.ReadLine());
            using (FileStream fs = new FileStream(file.ToString(), FileMode.Create, FileAccess.Write))
            { }
            Thread first = new Thread(new ParameterizedThreadStart(Chet));//thread-sazdaet potok
            Thread second = new Thread(new ParameterizedThreadStart(Nechet));

            second.Priority = ThreadPriority.Highest;//Поменяйте приоритет одного из потоков.
            Console.WriteLine("приоритет второго потока : ");
            Console.WriteLine(second.Priority);
            Console.WriteLine("приоритет первого потока :");
            Console.WriteLine(first.Priority);
            //последовательно выводились одно четное, другое нечетное.
            first.Start(x);
            second.Start(x);

            Thread.Sleep(1500);
            Console.WriteLine("ЗАДАНИЕ 5");
            Console.WriteLine("Введите количество секунд(таймер): ");
            int ch;
            ch = int.Parse(Console.ReadLine());
            ///////////////////////////////////////////////////////////////////////////////////////
            TimerCallback tm = new TimerCallback(TimerSec); // метод обратного вызова
            Timer tmr = new Timer(tm, ch, 500, 1000);
            Thread.Sleep(ch * 1000);
            tmr.Dispose();
            Console.ReadLine();
        }
        
           
        
    }
}
