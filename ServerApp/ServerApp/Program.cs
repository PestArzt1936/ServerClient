using Microsoft.AspNetCore.Builder;
using System.Security.Cryptography;
using ServerApp;
using System.Timers;

class menu
{
    static List<Bugs> bugs;
    static List<Scans> scans;
    static ObservableDictionary<string, Thread> ProgScans;

    public event Action<string, Thread>? ProgScansChanged;

    public static void Main()
    {
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
            return id;
        });
        app.MapGet("/Info", async (HttpContext httpContext) =>
        {
            using StreamReader reader = new StreamReader(httpContext.Request.Body);
            string ID = await reader.ReadToEndAsync();
            Bugs result = bugs.Find(x => x.GetID()==ID);
            if (result != null)
            {
                return result.AllData();
            }
            return "Данные не были найдены.";
        });
        app.MapPost("/scan", async (HttpContext httpContext) =>
        {
            using StreamReader reader = new StreamReader(httpContext.Request.Body);
            string data = await reader.ReadToEndAsync();
            string id = CreateID(12);
            var temp = new Scans(id, data);
            scans.Add(temp);
            return id;
        });
        app.Run("http://localhost:2702");
    }
    static string GetDesc(string text)
    {
        string desc = text.Substring(0,text.IndexOf(";"));
        return desc;
    }
    static string GetImportantness(string text)
    {
        string tt = text.Substring(text.IndexOf(";"));
        return tt;
    }
    static string CreateID(int count)
    {
        while (true) {
            string id = RandomNumberGenerator.GetHexString(count);
            if (bugs.Find(x => x.GetID() ==id) != null)
            {
                continue;
            }
            if (scans.Find(x => x.GetID()==id) != null)
            {
                continue;
            }
            return id;
        }
    }
    static System.Timers.Timer CreateTimer(int secs)
    {
        System.Timers.Timer timer = new System.Timers.Timer(secs);
        timer.Elapsed += OnTimeEvent;
        timer.AutoReset = false;
        timer.Enabled = true;
        return timer;
    }
    private static void OnTimeEvent(Object source,ElapsedEventArgs e)
    {
        Scans.refreshScans();
    }
}



