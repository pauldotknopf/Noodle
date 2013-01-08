using System;
using System.Diagnostics;
using System.Timers;
using Noodle.Configuration;

namespace Noodle.Scheduling
{
    /// <summary>
    /// A wrapper for a timer that beats at a certain interval.
    /// </summary>
    public class Heart : IHeart
    {
        readonly Timer _timer;

        public Heart(NoodleCoreConfiguration configuration)
        {
            _timer = new Timer(configuration.Scheduler.Interval * 1000);
        }

        #region IStartupTask
        
        public void Execute()
        {
            _timer.Enabled = true;
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        public int Order
        {
            get { return int.MaxValue; }
        }

        #endregion

        #region IHeart
        
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("Beat: " + DateTime.Now);
            if (Beat != null)
                Beat(this, e);
        }

        public event EventHandler Beat;

        #endregion
    }
}
