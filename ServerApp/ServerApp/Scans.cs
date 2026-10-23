using System.Security.Cryptography;

namespace ServerApp
{
    public class Scans
    {
        public State state;
        string URL;
        string ID;
        DateTime started;
        DateTime end;
        public DateTime changed;
        public Scans(string id,string URL)
        {
            state = State.InProgress;
            ID = id;
            this.URL= URL;
            started = DateTime.Now;
            changed = started;
            end = started.AddSeconds(RandomNumberGenerator.GetInt32(59)+10).AddMinutes(RandomNumberGenerator.GetInt32(5));
        }
        public void FinishScan()
        {
            state = State.finished;
            changed = DateTime.Now;
        }
        public string GetID()
        {
            return ID;
        }
        public int GetDuration()
        {
            return Convert.ToInt32((end - started).TotalSeconds);
        }
        public void Changed()
        {
            changed = DateTime.Now;
        }
    }
    public enum State
    {
        unknow,
        error,
        finished,
        InProgress,
        Canceled,
        InQueue
    }
}
