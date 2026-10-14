using System;
using System.Globalization;

public class TimeUtil
{

    public static bool isSameDay(long day1, long day2)
    {
        TimeSpan t1 = TimeSpan.FromSeconds(day1);
        TimeSpan t2 = TimeSpan.FromSeconds(day2);
        if (t1.Days == t2.Days)
        {
            return true;
        }
        return false;
    }


    public static bool IsNextDay(long lastTime, long currentTime)
    {
        TimeSpan t1 = TimeSpan.FromSeconds(lastTime);
        TimeSpan t2 = TimeSpan.FromSeconds(currentTime);

        return t1.Days != t2.Days && currentTime > lastTime;
    }

    public static bool DifferFourDays(long lastTime, long currentTime)
    {
        TimeSpan t1 = TimeSpan.FromSeconds(lastTime);
        TimeSpan t2 = TimeSpan.FromSeconds(currentTime);
        return t2.Days - t1.Days == 4;
    }
    /// <summary>
    /// 获取当前的UTC毫秒数
    /// </summary>
    /// <returns></returns>
    public static long GetCurrentUTCMilliSeconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 获取当前的UTC秒数
    /// </summary>
    /// <returns></returns>
    public static long GetCurrentUTSeconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public static long GetTomorrowBeginUTCSconds()
    {
        var timestamp = GetCurrentUTSeconds();
        return GetTomorrowBeginUTCSconds(timestamp);
    }


    public static long GetTodayBeginUTCSconds()
    {
        TimeSpan t = TimeSpan.FromSeconds(GetCurrentUTSeconds());
        long tomorrowTime = (long)t.TotalDays * 24 * 3600;
        return tomorrowTime;
    }

    public static long GetTomorrowBeginUTCSconds(long currentSeconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(currentSeconds);
        long tomorrowTime = (long)(t.TotalDays + 1) * 24 * 3600;
        return tomorrowTime;
    }

    public static DateTime GetLocalTime(double totalSeconds)
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(totalSeconds).ToLocalTime();
        return startTime;
    }

    public static string GetTimeHourString(double _time)
    {
        if (_time < 0)
        {
            _time = 0;
        }
        TimeSpan t = TimeSpan.FromMilliseconds(_time);
        string answer = string.Format("{0:D2}:{1:D2}:{2:D2}", (t.Days * 24 + t.Hours), t.Minutes, t.Seconds);
        return answer;
    }

    public static string GetTimeDayHourString(double _time)
    {
        if (_time < 0)
        {
            _time = 0;
        }
        TimeSpan t = TimeSpan.FromMilliseconds(_time);
        string answer = string.Format("{0}day {1:D2}:{2:D2}:{3:D2}", t.Days, t.Hours, t.Minutes, t.Seconds);
        return answer;
    }

    public static string GetTimeStrFromSeconds(double _time)
    {
        if (_time < 0)
        {
            _time = 0;
        }
        TimeSpan t = TimeSpan.FromSeconds(_time);
        string answer = string.Format("{0:D2}:{1:D2}:{2:D2}", (t.Days * 24 + t.Hours), t.Minutes, t.Seconds);
        return answer;
    }

    public static string GetMSStrFromSeconds(double _time)
    {
        if (_time < 0)
        {
            _time = 0;
        }
        TimeSpan t = TimeSpan.FromSeconds(_time);
        string answer = string.Format("{0}m{1}s", (t.Days * 24 + t.Hours) * 60 + t.Minutes, t.Seconds);
        return answer;
    }

    public static string GetTimeHMFromSeconds(double _time)
    {
        if (_time < 0)
        {
            _time = 0;
        }
        TimeSpan t = TimeSpan.FromSeconds(_time);
        string answer = string.Format("{0:D2}:{1:D2}", (t.Days * 24 + t.Hours), t.Minutes);
        return answer;
    }

    public static string GetTimeMSFromMill(double _time)
    {
        if (_time < 0)
        {
            _time = 0;
        }
        TimeSpan t = TimeSpan.FromMilliseconds(_time);
        string answer = string.Format("{0:D2}:{1:D2}", (t.Days * 24 + t.Hours) * 60 + t.Minutes, t.Seconds);
        return answer;
    }

    public static string GetDHMS(long seconds)
    {
        if (seconds < 0) seconds = 0;
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        string answer = string.Format("{0}d {1}h {2}m {3}s", t.Days, t.Hours, t.Minutes, t.Seconds);
        return answer;
    }

    public const string date_time_format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
    public static string GetCurrentTimeString()
    {
        DateTime recordTime = DateTime.UtcNow.ToUniversalTime();
        return recordTime.ToString(date_time_format);
    }


    public static string FormatDate(long seconds)
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).ToLocalTime();
        return startTime.ToString("yyyy-MM-dd");
    }

    public static string FormatDate2(long seconds)
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).ToLocalTime();
        return startTime.ToString("yyyy/MM/dd");
    }

    public static string FormatChatTime(long seconds)
    {
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).ToLocalTime();
        DateTime curTime = DateTime.Now;
        if (curTime.Year == dt.Year)
        {
            if (curTime.DayOfYear == dt.DayOfYear)  // 同一天
                return dt.ToString("HH:mm");
            else if (curTime.DayOfYear - dt.DayOfYear <= 7)
            {
                var month = dt.ToString("MMM", CultureInfo.GetCultureInfo("en-US"));
                var week = dt.DayOfWeek.ToString().Substring(0, 3);
                return string.Format("{0}, {1} {2}", week, month, dt.ToString("dd"));
            }
            else
            {
                var month = dt.ToString("MMM", CultureInfo.GetCultureInfo("en-US"));
                return month + " " + dt.ToString("dd");
            }
        }
        return dt.ToString("MMM dd yyyy", CultureInfo.GetCultureInfo("en-US"));
    }

    public static int DifferDays(long lastTime, long currentTime)
    {
        TimeSpan t1 = TimeSpan.FromSeconds(lastTime);
        TimeSpan t2 = TimeSpan.FromSeconds(currentTime);
        return t2.Days - t1.Days;
    }

}