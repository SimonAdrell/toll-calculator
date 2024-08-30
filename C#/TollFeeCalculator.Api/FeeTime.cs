namespace TollFeeCalculator.Api;

public record FeeTime(TimeOnly StartTime, TimeOnly EndTime, int Cost);