namespace Noodle
{
    /// <summary>
    /// Used to store a single item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Single<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Single{T}"/> class.
        /// </summary>
        public Single()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Single{T}"/> class with a value
        /// </summary>
        /// <param name="value">The value.</param>
        public Single(T value)
        {
            Value = value;
        }

        /// <summary>
        /// The enclosing item
        /// </summary>
        public T Value { get; set; }
    }
}
