using Microsoft.AspNetCore.Builder;
using System.Security.Cryptography;


class menu
{
    static List<Bugs> bugs;
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.MapPost("/add", async (HttpContext httpContext) =>
        {
            using StreamReader reader = new StreamReader(httpContext.Request.Body);
            string data = await reader.ReadToEndAsync();
            string desc = GetDesc(data);
            string imp = GetImpontness(data);
            string id = RandomNumberGenerator.GetHexString(8);
            bugs.Add(new Bugs(id,DateTime.Now, desc,Convert.ToInt32(imp)));
            return id;
        });
        app.MapGet("/Info", async (HttpContext httpContext) =>
        {
            using StreamReader reader = new StreamReader(httpContext.Request.Body);
            string ID = await reader.ReadToEndAsync();
            Bugs result = bugs.Find(x => x.GetInfo(ID) != null);
            if (result != null)
            {
                return result.AllData();
            }
            return "Данные не были найдены.";
        });
        app.Run("http://localhost:2702");
    }
    static string GetDesc(string text)
    {
        string desc = text.Substring(0,text.IndexOf(";"));
        return desc;
    }
    static string GetImpontness(string text)
    {
        string tt = text.Substring(text.IndexOf(";"));
        return tt;
    }
}
class Bugs
{
    string ID;
    DateTime date;
    string info;
    int Impontentness;
    public Bugs(string iD, DateTime date, string info, int impontentness)
    {
        ID = iD;
        this.date = date;
        this.info = info;
        Impontentness = impontentness;
    }
    public Bugs GetInfo(string id)
    {
        if (id == ID)
            return this;
        else
            return null;
    }
    public string AllData()
    {
        return ID + ";"+info+";"+Impontentness+";"+date.Date.ToString();
    }
}


