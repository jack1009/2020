using System.Timers;

namespace ClassLibraryTimer
{
    public class OnDelayTimer
    {
        Timer t1 = new Timer();
        int _count;
        public bool inEN { get; set; }
        public int inInterval { get; set; }
        public bool outQ { get; set; }
        public OnDelayTimer()
        {
            t1.Interval = 100;
            t1.AutoReset = true;
            t1.Elapsed += T1_Elapsed;
            t1.Start();
        }
        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (inEN && _count < inInterval)
            {
                _count++;
                if (_count >= inInterval)
                {
                    outQ = true;
                }
            }
            if (!inEN)
            {
                _count = 0;
                outQ = false;
            }
        }
    }
}
