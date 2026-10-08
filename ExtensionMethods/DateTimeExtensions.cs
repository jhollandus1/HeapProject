using System.Globalization;

namespace HeapProject.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static string ToUserTimeZoneLocaleString(this DateTime utcDateTime, string timeZoneId, string cultureName)
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tzi);
            CultureInfo culture = new CultureInfo(cultureName);

            return convertedTime.ToString("g", culture);
        }
        
    }
}
