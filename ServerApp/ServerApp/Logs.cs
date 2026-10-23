namespace ServerApp
{
    public class Logs
    {
        DateTime time;
        string info;
        string ID;
        string result;
        public Logs(string info,string ID,string result)
        {
            this.info = info;
            this.ID = ID;
            this.result = result;
            time = DateTime.Now;
        }
    }
}
