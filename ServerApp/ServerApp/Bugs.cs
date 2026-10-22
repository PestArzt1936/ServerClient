namespace ServerApp
{
    class Bugs
    {
        string ID;
        DateTime date;
        string info;
        int Importantness;
        public Bugs(string iD, DateTime date, string info, int importantness)
        {
            ID = iD;
            this.date = date;
            this.info = info;
            Importantness = importantness;
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
            return ID + ";" + info + ";" + Importantness + ";" + date.Date.ToString();
        }
        public string GetID()
        {
            return ID;
        }
    }
}
