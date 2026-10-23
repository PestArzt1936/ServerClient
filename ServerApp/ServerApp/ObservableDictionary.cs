namespace ServerApp
{
    public class ObservableDictionary<T,N>
    {
        public event Action<T, N>? ItemAdded;
        public event Action<T>? ItemRemoved;
        Dictionary<T, N> dict;
        int Limit;
        public ObservableDictionary()
        {
            dict = new Dictionary<T, N>();
            Limit = -1;
        }
        public ObservableDictionary(int limit)
        {
            dict = new Dictionary<T, N>();
            Limit = limit;
        }
        public void Add(T key, N value)
        {
            if (Limit == -1 || dict.Count < Limit) {
                dict.Add(key, value);
                ItemAdded?.Invoke(key, value); 
            }
            else
                throw new ArgumentOutOfRangeException("Aboba");
        }
        public void Remove(T key)
        {
            dict.Remove(key);
            ItemRemoved?.Invoke(key);
        }
        public N GetValue(T key)
        {
            return dict[key];
        }
        public void ChangeLimit(int limit)
        {
            Limit = limit;
        }
    }
}
