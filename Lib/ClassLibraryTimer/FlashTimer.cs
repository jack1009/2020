using System.Timers;

namespace ClassLibraryTimer
{
    /// <summary>
    /// 閃爍TIMER
    /// </summary>
    public class FlashTimer
    {
        Timer t1 = new Timer();
        public bool outFlash { get; set; }
        public FlashTimer(int _interval)
        {
            t1.Interval = _interval;
            t1.AutoReset = true;
            t1.Elapsed += T1_Elapsed;
            t1.Start();
        }
        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
            outFlash = !outFlash;
        }
    }
}
