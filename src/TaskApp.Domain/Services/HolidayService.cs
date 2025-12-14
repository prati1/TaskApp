using TaskApp.Domain.Interfaces;

namespace TaskApp.Domain.Services;

public class HolidayService : IHolidayService
{
  private static readonly HashSet<DateTime> _holidays = new()
  {
      new DateTime(2025, 12, 25),
      new DateTime(2026, 1, 1),
      new DateTime(2026, 7, 4),
      new DateTime(2026, 11, 26)
  }; 

  public bool IsHoliday(DateTime date)
  {
      return _holidays.Contains(date.Date);
  }

}
