using System.Globalization;

namespace Web.Utils
{
    public class DateUtils
    {
        public static int GetCurrentWeekOfYear()
        {
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.UtcNow, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public static int GetCurrentYear()
        {
            return DateTime.UtcNow.Year;
        }
    }
}
