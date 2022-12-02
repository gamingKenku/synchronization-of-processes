using System;
using System.Threading;

namespace synchronization_of_processes
{
    class Program
    {
        static int sleepTimeProducer = 3500;
        static int sleepTimeConsumer = 3500;

        static Mutex[] mutexes = new Mutex[10];
        static int?[] buffer = new int?[10];

        static Random rnd = new Random();

        static bool consumerOnFlag = true;
        static bool producerOnFlag = true;
        static bool keepTheJobDone = true;

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

        static void WriteToBuff()
        { 
            int i = 0;
            while (keepTheJobDone)
            {
                if (producerOnFlag == false)
                    continue;

                mutexes[i].WaitOne();
                if (buffer[i] == null)
                {
                    Console.WriteLine($"i = {i} >> Производитель генерирует число...");
                    buffer[i] = rnd.Next(10);
                    Thread.Sleep(sleepTimeProducer);
                    Console.WriteLine($"i = {i} >> Производитель сгенерировал число: {buffer[i]}");
                }
                mutexes[i].ReleaseMutex();
                i++;
                if (i > 9)
                    i = 0;
            }
        }
        static void ReadFromBuff()
        {
            int i = 0;
            while (keepTheJobDone)
            {
                if (consumerOnFlag == false)
                    continue;

                mutexes[i].WaitOne();
                if (buffer[i] != null)
                {
                    Console.WriteLine($"i = {i} >> Потребитель читает число...");
                    buffer[i] = null;
                    Thread.Sleep(sleepTimeConsumer);
                    Console.WriteLine($"i = {i} >> Потребитель прочитал число в " + i);
                }
                mutexes[i].ReleaseMutex();
                i++;
                if (i > 9)
                    i = 0;
            }
        }
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
