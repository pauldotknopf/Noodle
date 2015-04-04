using Noodle.Extensions.Scheduling;

namespace Noodle.Tests.Scheduling
{
    [ScheduleExecution(60, Repeat = Repeat.Indefinitely)]
    public class RepeatAction : OnceAction
    {
    }
}
