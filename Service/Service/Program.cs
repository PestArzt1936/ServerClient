using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

enum ChooseOption
{
    Exit,
    Add,
    GetInfo,
    CodeScan,
    GetScanStatus,
    CancelScan,
    Unknown
    

}
class Service
{
    
    public static HttpClient httpClient = new HttpClient();
    public static async Task Main()
    {
        Console.WriteLine("Добро пожаловать в сервис по управлению уязвимостями программного обеспечения.\nВыберите действие:\n");
        ChooseOption answ = ChooseOption.Unknown;
        while(answ != ChooseOption.Exit)
        {
            Console.WriteLine("1.Добавить новую запись\n2.Получить информации о уязвимости\n3.Просканировать код\n4.Узнать статус сканирования кода\n5.Отменить сканирование кода\n0.Закрыть приложение.");
            
            try
            {
                int temp = Convert.ToInt32(Console.ReadLine());
                if (temp < 0 || temp > 5)
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
                await Task.Delay(4000);
                Console.Clear();
                continue;
            }
            switch(answ){
                case ChooseOption.Add:
                    {
                        Console.Clear();
                        Console.WriteLine("Какую запись вы хотите добавить? Опишите уязвимость");
                        string desc = Console.ReadLine();
                        string scale = "";
                        while (true)
                        {
                            Console.WriteLine("Оцените важность данной уязвимости от 1 до 10");
                            scale = Console.ReadLine();
                            int temp;
                            if (Int32.TryParse(scale, out temp) == false)
                                Console.WriteLine("Оценка может быть выражена только числовым знаечнием от 1 до 10");
                            else if (temp < 1 || temp > 10)
                                Console.WriteLine("Оценка может быть выражена только числовым знаечнием от 1 до 10");
                            else
                                break;
                        }
                        StringContent content = new StringContent(desc +";"+ scale);
                        using var response = await httpClient.PostAsync("https://localhost:2702/add", content);
                        string responseText = await response.Content.ReadAsStringAsync();
                        Console.Clear();
                        Console.WriteLine("Ваша заявка записана. Её уникальный номер: "+responseText);
                        continue;
                    }
                case ChooseOption.GetInfo:
                    Console.Clear();
                    string id = "";
                    int k = 0;
                    while (k<3)
                    {
                        Console.WriteLine("Какую запись вы ищите? Необходимо ввести ее уникальный номер");
                        id = Console.ReadLine();
                        if (id.Length == 8)
                            break;
                        Console.WriteLine("Индефикатор заявки имеет строгую длину в 8 символов. Попробуйте еще раз");
                        k++;
                    }
                    if (k < 3)
                    {
                        using var response = await httpClient.GetAsync($"https://localhost:2702/Info/{id}");
                        string responseText = await response.Content.ReadAsStringAsync();
                        Console.Clear();
                        Console.WriteLine("Результат запроса:");
                        int count = 1;
                        while (responseText.Contains(";"))
                        {
                            int pos = responseText.IndexOf(";");
                            Console.WriteLine(count+". "+(responseText.Substring(0,pos)));
                            responseText=responseText.Remove(0, pos+1);
                            count++;
                        }
                        Console.WriteLine(count + ". " +responseText);
                        Console.WriteLine("\n");
                    }
                    continue;
                case ChooseOption.CodeScan:
                    {
                        Console.Clear();
                        Console.WriteLine("Вставте ссылку на код");
                        string desc = Console.ReadLine();
                        StringContent content = new StringContent(desc);
                        using var response = await httpClient.PostAsync("https://localhost:2702/Scan", content);
                        string responseText = await response.Content.ReadAsStringAsync();
                        Console.Clear();
                        if(responseText.Length==12)
                            Console.WriteLine("Сканирование начато. Её уникальный номер: " + responseText);
                        else
                            Console.WriteLine("Результат: "+responseText);
                            continue;
                    }
                case ChooseOption.GetScanStatus:
                    Console.Clear();
                    id = "";
                    k = 0;
                    while (k < 3)
                    {
                        Console.WriteLine("Какое сканирование вы ищите? Необходимо ввести ее уникальный номер");
                        id = Console.ReadLine();
                        if (id.Length == 12)
                            break;
                        Console.WriteLine("Индефикатор сканирования имеет строгую длину в 12 символов. Попробуйте еще раз");
                        k++;
                    }
                    if (k < 3)
                    {
                        using var response = await httpClient.GetAsync($"https://localhost:2702/ScanStatus/{id}");
                        string responseText = await response.Content.ReadAsStringAsync();
                        Console.Clear();
                        Console.WriteLine("Результат запроса:");
                        int count = 1;
                        while (responseText.Contains(";"))
                        {
                            int pos = responseText.IndexOf(";");
                            Console.WriteLine(count + ". " + (responseText.Substring(0, pos)));
                            responseText = responseText.Remove(0, pos + 1);
                            count++;
                        }
                        Console.WriteLine(count + ". " + responseText);
                        Console.WriteLine("\n");
                    }
                    continue;
                case ChooseOption.CancelScan:
                    Console.Clear();
                    id = "";
                    k = 0;
                    while (k < 3)
                    {
                        Console.WriteLine("Какое сканирование вы хотите отменить? Необходимо ввести ее уникальный номер");
                        id = Console.ReadLine();
                        if (id.Length == 12)
                            break;
                        Console.WriteLine("Индефикатор сканирования имеет строгую длину в 12 символов. Попробуйте еще раз");
                        k++;
                    }
                    if (k < 3)
                    {
                        StringContent content = new StringContent(id);
                        using var response = await httpClient.PostAsync("https://localhost:2702/ScanCancel", content);
                        string responseText = await response.Content.ReadAsStringAsync();
                        Console.Clear();
                        Console.WriteLine("Результат запроса:");
                        int count = 1;
                        while (responseText.Contains(";"))
                        {
                            int pos = responseText.IndexOf(";");
                            Console.WriteLine(count + ". " + (responseText.Substring(0, pos)));
                            responseText = responseText.Remove(0, pos + 1);
                            count++;
                        }
                        Console.WriteLine(count + ". " + responseText);
                        Console.WriteLine("\n");
                    }
                    continue;
                default:
                    continue;
            }
        }
    }
}