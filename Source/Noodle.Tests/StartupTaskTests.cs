using System;
using NUnit.Framework;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using Noodle.Engine;

namespace Noodle.Tests
{
    [TestFixture]
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

        [Test]
        public void Can_startup()
        {
            var prev = CommonHelper.CurrentTime;
            try
            {
                var now = DateTime.Now;
                CommonHelper.CurrentTime = () => now;
                StartupTask1.Executed.ShouldBeNull();
                StartupTask1.NumberOfTimesRan.ShouldEqual(0);

                var kernel = new StandardKernel(new NinjectSettings{});
                kernel.Components.Add<IBindingResolver, AutoStartBindingResolver>();
                kernel.Get<StartupTask1>();
                EngineContext.RunStartupTasks(kernel);

                kernel.Get<StartupTask1>();

                StartupTask1.Executed.ShouldEqual(now);
                StartupTask1.NumberOfTimesRan.ShouldEqual(1);
            }
            finally
            {
                CommonHelper.CurrentTime = prev;
            }
        }
    }
}
