namespace ClassLibraryTRIG
{
    public class F_TRIG
    {
        private bool actClk = true;
        public bool inClk { get; set; }
        public bool outQ { get; set; }
        public void RunCheckState()
        {
            //OFF
            if (actClk)
            {
                outQ = false;
            }
            //按下
            if (inClk)
            {
                actClk = false;
            }
            //放開
            if (!inClk && !actClk)
            {
                actClk = true;
                outQ = true;
            }
        }
    }
}
