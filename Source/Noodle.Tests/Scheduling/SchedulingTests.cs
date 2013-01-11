using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Noodle.Configuration;
using Noodle.Engine;
using Noodle.Plugins;
using Noodle.Scheduling;
using Noodle.Web;

namespace Noodle.Tests.Scheduling
{
    [TestClass]
    public class SchedulingTests : TestBase
    {
        Scheduler _scheduler;
        IErrorNotifier _errorHandler;
        Mock<IHeart> _heart;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var types = new Mock<ITypeFinder>();
            types.Setup(x => x.GetAssemblies()).Returns(new[] { GetType().Assembly });

            _heart = new Mock<IHeart>();

            _errorHandler = new Mock<IErrorNotifier>().Object;

            var ctx = new Mock<IRequestContext>();

            var plugins = new PluginFinder(types.Object, new NoodleCoreConfiguration(), null); //); // TODO: Add mock kernel

            var worker = new AsyncWorker();
            worker.QueueUserWorkItem = delegate(WaitCallback function)
            {
                function(null);
                return true;
            };

            _scheduler = new Scheduler(plugins, _heart.Object, worker, ctx.Object, _errorHandler);
            _scheduler.Execute();
        }

        [TestMethod]
        public void CanRunOnce()
        {
            var once = SelectThe<OnceAction>();
            var repeat = SelectThe<RepeatAction>();

            _heart.Raise(x => x.Beat += null, new EventArgs());

            once.executions.ShouldEqual(1);
            repeat.executions.ShouldEqual(1);
        }

        [TestMethod]
        public void OnceActionIsRemovedAfterRunningOnce()
        {
            var once = SelectThe<OnceAction>();
            var repeat = SelectThe<RepeatAction>();

            _heart.Raise(x => x.Beat += null, new EventArgs());

            _scheduler.Actions.Contains(once).ShouldEqual(false);
        }

        [TestMethod]
        public void RepeatActionIsNotExecutedBeforeTimeElapsed()
        {
            var repeat = SelectThe<RepeatAction>();

            var prev = CommonHelper.CurrentTime;
            try
            {
                var lastExecuted = repeat.LastExecuted;
                _heart.Raise(x => x.Beat += null, new EventArgs());
                lastExecuted = repeat.LastExecuted;

                CommonHelper.CurrentTime = () => DateTime.UtcNow.AddSeconds(50).ToUniversalTime();

                lastExecuted = repeat.LastExecuted;
                _heart.Raise(x => x.Beat += null, new EventArgs());
                lastExecuted = repeat.LastExecuted;
            }
            finally
            {
                CommonHelper.CurrentTime = prev;
            }
            repeat.executions.ShouldEqual(1);
        }

        [TestMethod]
        public void RepeatActionIsExecutedAfterTimeElapsed()
        {
            var repeat = SelectThe<RepeatAction>();

            var prev = CommonHelper.CurrentTime;
            try
            {
                _heart.Raise(x => x.Beat += null, new EventArgs());
                CommonHelper.CurrentTime = delegate { return DateTime.UtcNow.AddSeconds(70); };
                _heart.Raise(x => x.Beat += null, new EventArgs());

                repeat.executions.ShouldEqual(2);
            }
            finally
            {
                CommonHelper.CurrentTime = prev;
            }
        }

        [TestMethod]
        public void WillCatchErrorsAndContinueExecutionOfOtherActions()
        {
            var ex = new NullReferenceException("Bad bad");
            _scheduler.Actions.Insert(0, new ThrowingAction { ExceptionToThrow = ex });
            var once = SelectThe<OnceAction>();
            var repeat = SelectThe<RepeatAction>();

            Singleton<IKernel>.Instance = new StandardKernel();

            _heart.Raise(x => x.Beat += null, new EventArgs());

            once.executions.ShouldEqual(1);
            repeat.executions.ShouldEqual(1);
        }

        [TestMethod]
        public void Action_withIClosable_interface_are_disposed()
        {
            var action = new ClosableAction();
            _scheduler.Actions.Insert(0, action);
            _heart.Raise(x => x.Beat += null, new EventArgs());
            action.wasClosed.ShouldBeTrue();
            action.wasExecuted.ShouldBeTrue();
        }

        private T SelectThe<T>() where T : ScheduledAction
        {
            return (from a in _scheduler.Actions where a.GetType() == typeof(T) select a).Single() as T;
        }

        private class ClosableAction : ScheduledAction, IClosable
        {
            public bool wasExecuted = false;
            public bool wasClosed = false;

            public override void Execute()
            {
                wasExecuted = true;
            }

            #region IDisposable Members

            public void Dispose()
            {
                wasClosed = true;
            }

            #endregion
        }

        private class ThrowingAction : ScheduledAction
        {
            public Exception ExceptionToThrow { get; set; }
            public override void Execute()
            {
                throw ExceptionToThrow;
            }
        }
    }
}
