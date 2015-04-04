using System;

namespace Noodle.Scheduling
{
    /// <summary>
    /// Interface of a timer wrapper that beats at a certain interval.
    /// </summary>
    public interface IHeart : IStartupTask
    {
        event EventHandler Beat;
    }
}
