using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Noodle.Engine;
using Noodle.TinyIoC;

namespace Noodle.Tests
{
    [TestClass]
    public class StartupTaskTests
    {
        public class StartupTask1 : IStartupTask
        {
            public static DateTime? Executed;
            public static int NumberOfTimesRan;

            public void Execute()
            {
                NumberOfTimesRan += 1;
                Executed = CommonHelper.CurrentTime();
            }

            public int Order
            {
                get { return int.MaxValue; }
            }
        }

        [TestMethod]
        public void Can_startup()
        {
            var now = DateTime.Now;
            CommonHelper.CurrentTime = () => now;
            StartupTask1.Executed.ShouldBeNull();
            StartupTask1.NumberOfTimesRan.ShouldEqual(0);

            var kernel = new TinyIoCContainer();
            kernel.Register<StartupTask1>();
            EngineContext.RunStartupTasks(kernel);

            kernel.Resolve<StartupTask1>();

            StartupTask1.Executed.ShouldEqual(now);
            StartupTask1.NumberOfTimesRan.ShouldEqual(1);
        }
    }
}
