using System;

namespace Noodle.Scheduling
{
    /// <summary>
    /// Base class for actions that can be scheduled to be executed by the 
    /// system at certain intervals. Inherit from this class and decorate using 
	/// the <see cref="ScheduleExecutionAttribute"/> to enable.
    /// </summary>
    public abstract class ScheduledAction
    {
		/// <summary>The method that executes the action. Implement in a subclass.</summary>
        public abstract void Execute();

        public ScheduledAction()
        {
            Repeat = Repeat.Once;
            Interval = new TimeSpan(0, 1, 0);
            ErrorCount = 0;
            IsExecuting = false;
        }

        /// <summary>
        /// The number of consecutive times this action has failed.
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// Whether the action is currently executing.
        /// </summary>
        public bool IsExecuting { get; set; }

        /// <summary>
        /// The interval before next execution.
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// When the action was last executed.
        /// </summary>
        public DateTime? LastExecuted { get; set; }

        /// <summary>
        /// Wheter the action should run again.
        /// </summary>
        public Repeat Repeat { get; set; }

		/// <summary>
		/// Examines the properties to determine whether the action should run.
		/// </summary>
		public virtual bool ShouldExecute()
        {
            return !IsExecuting && (!LastExecuted.HasValue || LastExecuted.Value.Add(Interval) < CommonHelper.CurrentTime());
        }

        /// <summary>
        /// This method will be called when error occured in the action's Execute() method. 
        /// It can be overrided to write custom error handling routine. 
        /// The default behavior is to call ILogger.Fatal() with the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public virtual void OnError(Exception ex)
        {
            // TODO: Error notifier
            //EngineContext.Resolve<ILogger>().Fatal("Scheduled action error with '{0}'.".F(GetType().Name), ex);
        }
    }
}
