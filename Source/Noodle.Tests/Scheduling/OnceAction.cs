using System;
using Noodle.Extensions.Scheduling;

namespace Noodle.Tests.Scheduling
{
    [ScheduleExecution(Repeat.Once)]
    public class OnceAction : ScheduledAction
    {
        public int executions = 0;
        public DateTime LastCall;

        public override void Execute()
        {
            executions++;
            LastCall = CommonHelper.CurrentTime();
        }
    }
}
