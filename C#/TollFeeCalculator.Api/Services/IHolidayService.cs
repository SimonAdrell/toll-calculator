namespace TollFeeCalculator.Api.Services;

public interface IHolidayService
{
    public bool IsDateHoliday(DateTime dateTime);

}
