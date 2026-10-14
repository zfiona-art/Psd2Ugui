namespace FrameWork.Time
{
    public class TimeManager
    {
        public static long CurrentUtcMilliSeconds = TimeUtil.GetCurrentUTCMilliSeconds();
        public static void UpdateTime()
        {
            CurrentUtcMilliSeconds = TimeUtil.GetCurrentUTCMilliSeconds();
        }

        public static long CurrentUtcSeconds
        {
            get
            {
                return CurrentUtcMilliSeconds / 1000;
            }
        }
    }
}
