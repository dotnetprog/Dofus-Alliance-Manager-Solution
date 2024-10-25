namespace DAM.Core.Helpers.Utility
{
    public static class DateHelper
    {
        public static DateTime ToParisTime(this DateTime dateTime)
        {
            //Central European Standard Time
            var parisdate = TimeZoneInfo.ConvertTimeFromUtc(
                                  dateTime.ToUniversalTime(),
                                  TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
            return parisdate;
        }
    }
}
