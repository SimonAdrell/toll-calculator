using TollFeeCalculator.Api.Services;
using TollFeeCalculator.Api.Vehicles;

namespace TollFeeCalculator.Api;
public class TollCalculator
{
    const int MAX_FEE = 60;
    const int MAX_DIFF_MINUTES = 60;

    private readonly List<FeeTime> feeTimes = [
            new(TimeOnly.Parse("06:00"),TimeOnly.Parse("06:29"),8),
            new(TimeOnly.Parse("06:30"),TimeOnly.Parse("06:59"),13),
            new(TimeOnly.Parse("07:00"),TimeOnly.Parse("07:59"),18),
            new(TimeOnly.Parse("08:00"),TimeOnly.Parse("08:29"),13),
            new(TimeOnly.Parse("08:30"),TimeOnly.Parse("14:59"),8),
            new(TimeOnly.Parse("15:00"),TimeOnly.Parse("15:29"),13),
            new(TimeOnly.Parse("15:29"),TimeOnly.Parse("16:59"),18),
            new(TimeOnly.Parse("17:29"),TimeOnly.Parse("16:59"),13),
            new(TimeOnly.Parse("18:00"),TimeOnly.Parse("18:30"),8),
    ];

private readonly IHolidayService _holidayService;

    public TollCalculator(IHolidayService holidayService)
    {
        _holidayService = holidayService;
    }


    public int GetTollFee(IVehicle vehicle, DateTime[] dateTimePasses)
    {
        ArgumentNullException.ThrowIfNull(vehicle, nameof(dateTimePasses));
        ArgumentNullException.ThrowIfNull(dateTimePasses, nameof(dateTimePasses));

        var totalFee = 0;
        if (IsTollFreeVehicle(vehicle))
            return totalFee;

        DateTime? previusDate = null;
        int previusFee = 0;

        foreach (var date in dateTimePasses)
        {
            if (IsTollFreeDate(date))
                continue;

            var fee = GetFee(date);
            if (HasVehicleBeenChargedWithingMaxChargedMinutes(previusDate, date))
            {
                if (fee > previusFee)
                    totalFee -= previusFee;
            }

            totalFee += fee;
            previusFee = fee;
            previusDate = date;
            if (totalFee > MAX_FEE)
                return MAX_FEE;
        }

        return totalFee;
    }

    private static bool HasVehicleBeenChargedWithingMaxChargedMinutes(DateTime? previusDate, DateTime currentDateTime)
    {
        if (previusDate == null)
            return false;

        if (currentDateTime.Subtract((DateTime)previusDate).Minutes <= MAX_DIFF_MINUTES)
            return true;

        return false;
    }

    private static bool IsTollFreeVehicle(IVehicle vehicle)
    {
        return Enum.IsDefined(typeof(TollFreeVehicles), vehicle.GetVehicleType());
    }

    public int GetFee(DateTime date)
    {
        var timeOnly = TimeOnly.FromDateTime(date);
        var fee = feeTimes.First(e => timeOnly.IsBetween(e.StartTime, e.EndTime));
        return fee?.Cost ?? 0;
    }

    private  bool IsTollFreeDate(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return true;

        if(_holidayService.IsDateHoliday(date))
            return true;
        return false;
    }
}
