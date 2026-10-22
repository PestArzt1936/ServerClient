using System.Security.Cryptography;

namespace ServerApp
{
    public class Scans
    {
        private static List<Scans> inProg = new List<Scans>(3);
        public State state;
        string URL;
        string ID;
        DateTime started;
        DateTime end;
        public Scans(string id,string URL)
        {
            state = State.InQueue;
            ID = id;
            this.URL= URL;
            started = DateTime.Now;
            end = started.AddSeconds(RandomNumberGenerator.GetInt32(59)).AddMinutes(RandomNumberGenerator.GetInt32(5));
            if (inProg.Count < 3)
                state = State.InProgress;
        }
        static public void refreshScans()
        {
            for(int i=inProg.Count-1;i>=0;i--)
            {
                if (inProg[i].end<=DateTime.Now)
                {
                    inProg[i].state = State.finished;
                    inProg.RemoveAt(i);
                }
            }
        }
        public string GetID()
        {
            return ID;
        }
        public int GetDuration()
        {
            return (end - started).Seconds;
        }
    }
    public enum State
    {
        unknow,
        error,
        finished,
        InProgress,
        stopped,
        InQueue
    }
}
