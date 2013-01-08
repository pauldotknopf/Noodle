namespace Noodle.Events
{
    /// <summary>
    /// A containe for entities that are udpated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityUpdated<T>
    {
        private readonly T _entity;

        public EntityUpdated(T entity)
        {
            _entity = entity;
        }

        public T Entity { get { return _entity; } }
    }
}
