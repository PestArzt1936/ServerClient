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
            dict.Add(key, value);
            ItemAdded?.Invoke(key, value);
        }
        public void Remove(T key)
        {
            dict.Remove(key);
            ItemRemoved?.Invoke(key);
        }
    }
}
