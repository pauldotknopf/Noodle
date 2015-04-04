using System;
using System.Linq;
using System.Threading;
using Moq;
using Noodle.Extensions.Plugins;
using Noodle.Extensions.Scheduling;
using NUnit.Framework;
using Noodle.Engine;

namespace Noodle.Tests.Scheduling
{
    [TestFixture]
    public class SchedulingTests : TestBase
    {
        Scheduler _scheduler;
        IErrorNotifier _errorHandler;
        Mock<IHeart> _heart;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var types = new Mock<ITypeFinder>();
            types.Setup(x => x.GetAssemblies()).Returns(new[] { GetType().Assembly });

            _heart = new Mock<IHeart>();

            _errorHandler = new Mock<IErrorNotifier>().Object;

            var plugins = new PluginFinder(types.Object, _container);

            var worker = new AsyncWorker();
            worker.QueueUserWorkItem = delegate(WaitCallback function)
            {
                function(null);
                return true;
            };

            _scheduler = new Scheduler(plugins, _heart.Object, worker, _errorHandler);
            _scheduler.Execute();
        }

        [Test]
        public void CanRunOnce()
        {
            var once = SelectThe<OnceAction>();
            var repeat = SelectThe<RepeatAction>();

            _heart.Raise(x => x.Beat += null, new EventArgs());

            once.executions.ShouldEqual(1);
            repeat.executions.ShouldEqual(1);
        }

        [Test]
        public void OnceActionIsRemovedAfterRunningOnce()
        {
            var once = SelectThe<OnceAction>();

            _heart.Raise(x => x.Beat += null, new EventArgs());

            _scheduler.Actions.Contains(once).ShouldEqual(false);
        }

        [Test]
        public void RepeatActionIsNotExecutedBeforeTimeElapsed()
        {
            var repeat = SelectThe<RepeatAction>();

            var prev = CommonHelper.CurrentTime;
            try
            {
                _heart.Raise(x => x.Beat += null, new EventArgs());
                CommonHelper.CurrentTime = () => DateTime.UtcNow.AddSeconds(50).ToUniversalTime();
                _heart.Raise(x => x.Beat += null, new EventArgs());
            }
            finally
            {
                CommonHelper.CurrentTime = prev;
            }
            repeat.executions.ShouldEqual(1);
        }

        [Test]
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

        [Test]
        public void WillCatchErrorsAndContinueExecutionOfOtherActions()
        {
            var ex = new NullReferenceException("Bad bad");
            _scheduler.Actions.Insert(0, new ThrowingAction { ExceptionToThrow = ex });
            var once = SelectThe<OnceAction>();
            var repeat = SelectThe<RepeatAction>();

            _heart.Raise(x => x.Beat += null, new EventArgs());

            once.executions.ShouldEqual(1);
            repeat.executions.ShouldEqual(1);
        }

        [Test]
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
