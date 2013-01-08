namespace Noodle.Events
{
    /// <summary>
    /// Consumers implementing this interface can be sorted. Sequenced comsumers take precedence over non-sequenced consumers.
    /// </summary>
    public interface IConsumerSequence
    {
        int Sequence { get; }
    }
}
