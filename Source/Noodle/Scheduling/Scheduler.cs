using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Noodle.Engine;
using Noodle.Plugins;

namespace Noodle.Scheduling
{
    /// <summary>
    /// Maintains a list of scheduler actions and checks wether it's time to 
    /// execute them.
    /// </summary>
    public class Scheduler : IStartupTask
    {
        readonly IList<ScheduledAction> _actions;
        readonly IHeart _heart;
        readonly IWorker _worker;
        readonly IErrorNotifier _errorHandler;

        public Scheduler(IPluginFinder plugins, IHeart heart, IWorker worker, IErrorNotifier errorHandler)
        {
            _actions = new List<ScheduledAction>(InstantiateActions(plugins));
            _heart = heart;
            _worker = worker;
            _errorHandler = errorHandler;
        }

        public IList<ScheduledAction> Actions
        {
            get { return _actions; }
        }

        protected TimeSpan CalculateInterval(int interval, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return new TimeSpan(0, 0, interval);
                case TimeUnit.Minutes:
                    return new TimeSpan(0, interval, 0);
                case TimeUnit.Hours:
                    return new TimeSpan(interval, 0, 0);
                default:
                    throw new NotSupportedException("Unknown time unit: " + unit);
            }
        }

        private IEnumerable<ScheduledAction> InstantiateActions(IPluginFinder plugins)
        {
            foreach (ScheduleExecutionAttribute attr in plugins.GetPlugins<ScheduleExecutionAttribute>())
            {
                var action = Activator.CreateInstance(attr.Decorates) as ScheduledAction;
                action.Interval = CalculateInterval(attr.Interval, attr.Unit);
                action.Repeat = attr.Repeat;
                yield return action;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        void HeartBeat(object sender, EventArgs e)
        {
            for (int i = 0; i < _actions.Count; i++)
            {
                ScheduledAction action = _actions[i];
                if (action.ShouldExecute())
                {
                    action.IsExecuting = true;
                    _worker.DoWork(delegate
                    {
                        try
                        {
                            Debug.WriteLine("Executing " + action.GetType().Name);
                            action.Execute();
                            action.ErrorCount = 0;
                        }
                        catch (Exception ex)
                        {
                            action.ErrorCount++;
                            action.OnError(ex);
                        }
                        finally
                        {
                            try
                            {
                                var closable = action as IClosable;
                                if (closable != null)
                                    closable.Dispose();
                            }
                            catch (Exception ex)
                            {
                                _errorHandler.Notify("Noodle.Scheduling.Scheduler heartbeat error", ex);
                            }
                        }
                        action.LastExecuted = CommonHelper.CurrentTime();
                        action.IsExecuting = false;
                    });

                    if (action.Repeat == Repeat.Once)
                    {
                        _actions.RemoveAt(i);
                        --i;
                    }
                }
            }
        }

        public void Execute()
        {
            _heart.Beat += HeartBeat;
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
