using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace SyntacticSugar
{
    public static class pytime
    {
        private static readonly Stopwatch CpuTimer = Stopwatch.StartNew();

        public static int altzone()
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            if (localZone.SupportsDaylightSavingTime)
            {
                var now = DateTime.UtcNow;
                var daylightOffset = localZone.GetUtcOffset(now.AddMonths(6)); // 假设夏令时发生在6个月后
                return (int)-daylightOffset.TotalSeconds; // 返回偏移秒数，负值表示东部
            }

            return 0; // 非夏令时
        }

        public static string asctime(DateTime? tupletime = null)
        {
            var dt = tupletime ?? DateTime.Now;
            return dt.ToString("ddd MMM dd HH:mm:ss yyyy"); // 格式: Tue Dec 11 18:07:14 2008
        }

        public static double clock()
        {
            return CpuTimer.Elapsed.TotalSeconds; // 返回当前 CPU 时间
        }

        public static string ctime(long? secs = null)
        {
            var dt = secs.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(secs.Value).LocalDateTime
                : DateTime.Now;
            return dt.ToString("ddd MMM dd HH:mm:ss yyyy");
        }

        public static DateTime gmtime(long? secs = null)
        {
            return secs.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(secs.Value).UtcDateTime
                : DateTime.UtcNow;
        }

        public static DateTime localtime(long? secs = null)
        {
            return secs.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(secs.Value).LocalDateTime
                : DateTime.Now;
        }

        public static long mktime(DateTime tupletime)
        {
            return new DateTimeOffset(tupletime).ToUnixTimeSeconds();
        }

        public static void sleep(double secs)
        {
            Thread.Sleep((int)(secs * 1000)); // 毫秒为单位
        }

        public static string strftime(string fmt, DateTime? tupletime = null)
        {
            var dt = tupletime ?? DateTime.Now;
            return dt.ToString(fmt, CultureInfo.InvariantCulture);
        }

        public static DateTime strptime(string timeString, string fmt = "%a %b %d %H:%M:%S %Y")
        {
            var format = fmt.Replace("%a", "ddd").Replace("%b", "MMM")
                .Replace("%d", "dd").Replace("%H", "HH")
                .Replace("%M", "mm").Replace("%S", "ss")
                .Replace("%Y", "yyyy");
            return DateTime.ParseExact(timeString, format, CultureInfo.InvariantCulture);
        }

        public static long time()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public static void tzset(string timezoneId = "UTC")
        {
            TimeZoneInfo.ClearCachedData(); // 清除时区缓存
            TimeZoneInfo newZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            Console.WriteLine($"Time zone set to {newZone.DisplayName}");
        }
    }

    public static class calendar
    {
        private static DayOfWeek FirstWeekday = DayOfWeek.Monday;

        public static string calendar_(int year, int w = 2, int l = 1, int c = 6)
        {
            var result = "";
            var cal = new GregorianCalendar();
            for (int i = 1; i <= 12; i += 3)
            {
                result += string.Join(new string(' ', c),
                    month(year, i, w, l),
                    month(year, i + 1, w, l),
                    month(year, i + 2, w, l)) + "\n\n";
            }

            return result;
        }

        public static int firstweekday()
        {
            return (int)FirstWeekday;
        }

        public static bool isleap(int year)
        {
            return DateTime.IsLeapYear(year);
        }

        public static int leapdays(int y1, int y2)
        {
            int count = 0;
            for (int i = y1; i < y2; i++)
            {
                if (DateTime.IsLeapYear(i))
                    count++;
            }

            return count;
        }

        public static string month(int year, int month, int w = 2, int l = 1)
        {
            if (month < 1 || month > 12) throw new ArgumentException("Month must be between 1 and 12");
            var cal = new GregorianCalendar();
            var firstDay = new DateTime(year, month, 1);
            var result = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year}\n";
            result += "Mo Tu We Th Fr Sa Su\n";
            var offset = (int)(firstDay.DayOfWeek - FirstWeekday + 7) % 7;

            result += new string(' ', w * offset);
            for (int day = 1; day <= cal.GetDaysInMonth(year, month); day++)
            {
                result += $"{day,2} ";
                if ((offset + day) % 7 == 0) result += "\n";
            }

            return result.TrimEnd();
        }

        public static List<List<int>> monthcalendar(int year, int month)
        {
            if (month < 1 || month > 12) throw new ArgumentException("Month must be between 1 and 12");
            var cal = new GregorianCalendar();
            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = cal.GetDaysInMonth(year, month);

            var result = new List<List<int>>();
            var week = new List<int>(new int[7]);
            int offset = (int)(firstDay.DayOfWeek - FirstWeekday + 7) % 7;

            for (int day = 1; day <= daysInMonth; day++)
            {
                week[offset] = day;
                offset = (offset + 1) % 7;
                if (offset == 0 || day == daysInMonth)
                {
                    result.Add(week);
                    week = new List<int>(new int[7]);
                }
            }

            return result;
        }

        public static Tuple<int, int> monthrange(int year, int month)
        {
            if (month < 1 || month > 12) throw new ArgumentException("Month must be between 1 and 12");
            var cal = new GregorianCalendar();
            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = cal.GetDaysInMonth(year, month);

            return Tuple.Create((int)(firstDay.DayOfWeek - FirstWeekday + 7) % 7, daysInMonth);
        }

        public static void prcal(int year, int w = 2, int l = 1, int c = 6)
        {
            Console.WriteLine(calendar_(year, w, l, c));
        }

        public static void prmonth(int year, int m, int w = 2, int l = 1)
        {
            Console.WriteLine(month(year, m, w, l));
        }

        public static void setfirstweekday(int weekday)
        {
            if (weekday < 0 || weekday > 6)
                throw new ArgumentException("Weekday must be between 0 (Monday) and 6 (Sunday)");
            FirstWeekday = (DayOfWeek)weekday;
        }

        public static long timegm(DateTime tupletime)
        {
            return new DateTimeOffset(tupletime).ToUnixTimeSeconds();
        }

        public static int weekday(int year, int month, int day)
        {
            var date = new DateTime(year, month, day);
            return (int)(date.DayOfWeek - FirstWeekday + 7) % 7;
        }
    }
}