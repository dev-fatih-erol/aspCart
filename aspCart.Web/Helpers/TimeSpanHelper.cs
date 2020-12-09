using System;

namespace aspCart.Web.Helpers
{
    public class TimeSpanHelper
    {
        public string GetRemainingTime(TimeSpan currentTime, TimeSpan endTime)
        {
            var timeSpan = new TimeSpan(endTime.Ticks - currentTime.Ticks);
            double delta = Math.Abs(timeSpan.TotalSeconds);

            if (delta >= 3600)
            {
                return timeSpan.Hours + " saat";
            }
            else if (delta >= 60)
            {
                return timeSpan.Minutes + " dakika";
            }
            else if (delta >= 0)
            {
                return timeSpan.Seconds + " saniye";
            }

            return "Belirtilmemiş";
        }
    }
}