using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http;

class Service
{
    enum ChooseOption
    {
        Exit,
        Add,
        GetInfo,
        CodeScan,
        GetScanStatus,
        Unknown

    }
    public static async Task Main()
    {
        Console.WriteLine("Добро пожаловать в сервис по управлению уязвимостями программного обеспечения.\nВыберите действие:\n");
        ChooseOption answ = ChooseOption.Unknown;
        while(answ != ChooseOption.Exit)
        {
            Console.WriteLine("1.Добавить новую запись\n2.Получить информации о уязвимости\n3.Просканировать код\n4.Узнать статус сканирования кода\n0.Закрыть приложение.");
            
            try
            {
                int temp = Convert.ToInt32(Console.ReadLine());
                if (temp < 0 || temp > 4)
                {
                    throw new Exception("Неизвестная команда");
                }
                answ = (ChooseOption)temp;
            }
            catch
            {
                Console.Beep();
                Console.Clear();
                Console.WriteLine("Была введена неизвестная команда. Попробуйте еще раз.");
                answ = ChooseOption.Unknown;
                await Task.Delay(3000);
                Console.Clear();
                continue;
            }
            switch(answ){
                case ChooseOption.Add:
                    Console.WriteLine("Временная заглушка");
                    continue;
                case ChooseOption.GetInfo:
                    Console.WriteLine("Временная заглушка");
                    continue;
                case ChooseOption.CodeScan:
                    Console.WriteLine("Временная заглушка");
                    continue;
                case ChooseOption.GetScanStatus:
                    Console.WriteLine("Временная заглушка");
                    continue;
                default:
                    continue;
            }
        }
    }
}