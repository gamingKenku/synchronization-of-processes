using System;
using System.Threading;

namespace synchronization_of_processes
{
    class Program
    {
        static int sleepTimeProducer = 3500;
        static int sleepTimeConsumer = 3500;

        // мьютексы для каждого элемента буфера
        static Mutex[] mutexes = new Mutex[10];
        static int?[] buffer = new int?[10];

        static Random rnd = new Random();
        
        // флаги для продолжения работы потребителя, производителя и программы в целом
        static bool consumerOnFlag = true;
        static bool producerOnFlag = true;
        static bool keepTheJobDone = true;

        // методы для увеличения времени сна
        static void SleepTimeIncreaseProd() => sleepTimeProducer += 1000;
        static void SleepTimeIncreaseCons() => sleepTimeConsumer += 1000;
        static void SleepTimeDecreaseProd()
        {
            if (sleepTimeProducer < 1000) return;

            sleepTimeProducer -= 1000;
        }
        static void SleepTimeDecreaseCons()
        {
            if (sleepTimeConsumer < 1000) return;

            sleepTimeConsumer -= 1000;
        }

        static string ShowBuffer()
        {
            string res = "";
            foreach (int? item in buffer)
            {
                if (item == null)
                    res += "_";
                else res += Convert.ToString(item);

                res += " ";
            }
            return res;
        }

        // метод для потока-производителя
        static void WriteToBuff()
        { 
            int i = 0;
            while (keepTheJobDone)
            {
                // пропускать весь цикл, если работа производителя приостановления
                if (producerOnFlag == false)
                    continue;

                // заблокировать мьютекс
                mutexes[i].WaitOne();
                if (buffer[i] == null)
                {
                    Console.WriteLine($"i = {i} >> Производитель генерирует число...");
                    buffer[i] = rnd.Next(10);
                    Thread.Sleep(sleepTimeProducer);
                    Console.WriteLine($"i = {i} >> Производитель сгенерировал число: {buffer[i]} | Буффер в данный момент: {ShowBuffer()}");
                }
                // освободить мютекс
                mutexes[i].ReleaseMutex();

                i++;
                // вернуться в начало буфера
                if (i > 9)
                    i = 0;
            }
        }
        // метод для потока-потребителя
        static void ReadFromBuff()
        {
            int i = 0;
            while (keepTheJobDone)
            {
                // пропускать весь цикл, если работа потребителя приостановления
                if (consumerOnFlag == false)
                    continue;

                // заблокировать мьютекс
                mutexes[i].WaitOne();
                if (buffer[i] != null)
                {
                    Console.WriteLine($"i = {i} >> Потребитель читает число...");
                    buffer[i] = null;
                    Thread.Sleep(sleepTimeConsumer);
                    Console.WriteLine($"i = {i} >> Потребитель прочитал число {buffer[i]} | Буффер в данный момент: {ShowBuffer()}");
                }
                // освободить мютекс
                mutexes[i].ReleaseMutex();
                i++;

                // вернуться в начало буфера
                if (i > 9)
                    i = 0;
            }
        }
        // метод для потока-ввода
        static void Input()
        {
            while(true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.KeyChar == '1')
                {
                    SleepTimeIncreaseProd();
                    Console.WriteLine("Время работы производителя увеличено.");
                }
                else if (key.KeyChar == '2')
                {
                    SleepTimeDecreaseProd();
                    Console.WriteLine("Время работы производителя уменьшено.");
                }
                else if (key.KeyChar == '3')
                {
                    SleepTimeIncreaseCons();
                    Console.WriteLine("Время работы потребителя увеличено.");
                }
                else if (key.KeyChar == '4')
                {
                    SleepTimeDecreaseCons();
                    Console.WriteLine("Время работы потребителя уменьшено.");
                }
                else if (key.KeyChar == '5')
                    if (producerOnFlag == true)
                    {
                        producerOnFlag = false;
                        Console.WriteLine("Работа производителя приостановлена.");
                    }
                    else
                    {
                        producerOnFlag = true;
                        Console.WriteLine("Работа производителя возобновлена.");
                    }
                else if (key.KeyChar == '6')
                    if (consumerOnFlag == true)
                    {

                        consumerOnFlag = false;
                        Console.WriteLine("Работа потребителя приостановлена.");
                    }
                    else
                    {
                        consumerOnFlag = true;
                        Console.WriteLine("Работа потребителя возобновлена.");
                    }
                else if (key.KeyChar == '0')
                {
                    Console.WriteLine("Завершение работы...");
                    keepTheJobDone = false;
                    break;
                }
            }
        }
        static void Main()
        {
            // инициализация буфера и мьютексов
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = null;
                mutexes[i] = new Mutex();
            }

            Thread producer = new Thread(WriteToBuff);
            Thread consumer = new Thread(ReadFromBuff);
            Thread input = new Thread(Input);
            input.Start();
            producer.Start();
            consumer.Start();
        }
    }
}
