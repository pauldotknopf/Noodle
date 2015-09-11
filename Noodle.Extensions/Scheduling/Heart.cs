using System;
using System.Diagnostics;
using System.Timers;

namespace Noodle.Extensions.Scheduling
{
    /// <summary>
    /// A wrapper for a timer that beats at a certain interval.
    /// </summary>
    public class Heart : IHeart
    {
        public static int IntervalSeconds = 60;
        readonly Timer _timer;

        public Heart()
        {
            _timer = new Timer(IntervalSeconds * 1000);
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
