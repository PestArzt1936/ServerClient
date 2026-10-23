using Microsoft.AspNetCore.Builder;
using ServerApp;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Timers;

class menu
{
    static List<Bugs> bugs;
    static List<Scans> scans;
    static ObservableDictionary<string, CancellationTokenSource> ProgScans;
    static List<Scans> scansQueue;
    static List<Logs> logs;

    public event Action<string, Thread>? ProgScansChanged;

    public static void Main()
    {
        bugs = new List<Bugs>();
        scans = new List<Scans>();
        ProgScans = new ObservableDictionary<string, CancellationTokenSource>(3);
        scansQueue = new List<Scans>();
        logs = new List<Logs>();
        ProgScans.ItemRemoved += OnScanRemove;
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapPost("/add", async (HttpContext httpContext) =>
        {
            using StreamReader reader = new StreamReader(httpContext.Request.Body);
            string data = await reader.ReadToEndAsync();
            string desc = GetDesc(data);
            string imp = GetImportantness(data);
            string id = CreateID(8);
            bugs.Add(new Bugs(id,DateTime.Now, desc,Convert.ToInt32(imp)));
            logs.Add(new Logs("Создание заявки", id, "Успешно"));
            return id;
        });
        app.MapGet("/Info/{id}", async (string id) =>
        {
            Bugs result = bugs.Find(x => x.GetID() == id);
            if (result != null)
            {
                logs.Add(new Logs("Получение информации по заявке", id, "Успешно"));
                return result.AllData();
            }
            logs.Add(new Logs("Получение информации по заявке", id, "Отказ"));
            return "Данные не были найдены.";
        });
        app.MapPost("/Scan", async (HttpContext httpContext) =>
        {
            using StreamReader reader = new StreamReader(httpContext.Request.Body);
            string data = await reader.ReadToEndAsync();
            string id = CreateID(12);
            var temp = new Scans(id, data);
            try
            {
                scans.Add(temp);
                    CancellationTokenSource CT = new CancellationTokenSource();
                    ProgScans.Add(id, CT);
                    _ = ScanAsync(temp, CT);
            }
            catch(ArgumentOutOfRangeException e)
            {
                temp.state = State.InQueue;
                temp.Changed();
                scansQueue.Add(temp);
            }
            logs.Add(new Logs($"Запрос на сканирование кода.", id, temp.state.ToString()));
            return id;
        });
        app.MapGet("/ScanStatus/{id}", async (string id) =>
        {
            Scans result = scans.Find(x => x.GetID() == id);
            if (result != null)
            {
                logs.Add(new Logs("Получение информации по сканированию", id, "Успешно"));
                return "status:" + result.state.ToString() + ";Last changed:" + result.changed.ToString(); ;
            }
            logs.Add(new Logs("Получение информации по сканированию", id, "Отказ"));
            return "Данные не были найдены.";
        });
        app.MapPost("/ScanCancel", async (HttpContext httpContext) =>
        {
            using StreamReader reader = new StreamReader(httpContext.Request.Body);
            string id = await reader.ReadToEndAsync();
            Scans sc = scans.Find(x=>x.GetID() == id);
            if (sc != null)
            {
                string callResult = "";
                if (sc.state == State.InQueue)
                {
                    sc.state = State.Canceled;
                    sc.Changed();
                    scansQueue.Remove(sc);
                    logs.Add(new Logs("Запрос на отмену сканирования", id, "Успешно"));
                    callResult = "Успешно";
                }
                else if(sc.state == State.InProgress)
                {
                    var CT=ProgScans.GetValue(id);
                    CT.Cancel();
                    ProgScans.Remove(id);
                    logs.Add(new Logs("Запрос на отмену сканирования", id, "Успешно"));
                    callResult = "Успешно";
                    CT.Dispose();
                }
                else
                {
                    logs.Add(new Logs("Запрос на отмену сканирования", id, "Отказ"));
                    callResult = "Отказ";
                }
                return "ID:"+id+";Date:"+sc.changed+";Result:"+callResult;
            }
            else
            {
                logs.Add(new Logs("Запрос на отмену сканирования", id, "Отказ"));
                return "Данные о сканировании не были найдены.";
            }
        });
        app.MapPost("/ScanLimitChange", async (HttpContext httpContext) =>
        {
            using StreamReader reader = new StreamReader(httpContext.Request.Body);
            string limit = await reader.ReadToEndAsync();
            ProgScans.ChangeLimit(Convert.ToInt32(limit));
            logs.Add(new Logs("Изменение лимита сканирвоания. Лимит:"+limit, "-", "Успешно"));
            return "Лимит усмпешно изменен на "+limit+". Учтите, что измененный лимит не отменяет текущие сканирования";
        });
        app.Run("https://localhost:2702");
    }

    private static void OnScanRemove(string obj)
    {
        if (scansQueue.Count > 0)
        {
            CancellationTokenSource CT = new CancellationTokenSource();
            Scans temp = scansQueue[0];
            scansQueue.RemoveAt(0);
            temp.state = State.InProgress;
            logs.Add(new Logs($"Изменение состояния сканирования", temp.GetID(), temp.state.ToString()));
            temp.Changed();
            ProgScans.Add(temp.GetID(), CT);
            _ = ScanAsync(temp, CT);
        }
    }

    static string GetDesc(string text)
    {
        string desc = text.Substring(0,text.IndexOf(";"));
        return desc;
    }
    static string GetImportantness(string text)
    {
        string tt = text.Substring(text.IndexOf(";")+1);
        return tt;
    }
    static string CreateID(int count)
    {
        while (true) {
            string id = RandomNumberGenerator.GetHexString(count);
                if (bugs.Exists(x => x.GetID() ==id) == true)
                {
                    continue;
                }
                if (scans.Exists(x => x.GetID() == id) == true)
                {
                    continue;
                }
            return id;
        }
    }
    static async Task ScanAsync(Scans scan, CancellationTokenSource ct)
    {
        try
        {
            await Task.Delay(scan.GetDuration()*1000, ct.Token);
            scan.FinishScan();
            lock (logs)
            {
                logs.Add(new Logs($"Изменение состояния сканирования", scan.GetID(), scan.state.ToString()));
            }
            //Debug.Print($"Закончено сканирование. ID:{scan.GetID()}");
        }
        catch (OperationCanceledException e)
        {
            lock (logs)
            {
                logs.Add(new Logs($"Изменение состояния сканирования", scan.GetID(), scan.state.ToString()));
                scan.state = State.Canceled;
                scan.Changed();
            }
            Debug.Print($"Отмена сканирования. ID:{scan.GetID()}");
        }
        finally
        {
            lock (ProgScans)
            {
                ProgScans.Remove(scan.GetID());
            }
            ct.Dispose();
        }
    }
}



