namespace Noodle
{
    public class Single<T>
    {
        public Single()
        {
            
        }

        public Single(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}
