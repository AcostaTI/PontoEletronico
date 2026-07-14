using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PontoEletronico.Infrastructure.Persistence.Conversions;

public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
{
    public TimeOnlyConverter() : base(
        t => t.ToTimeSpan(),                  
        t => TimeOnly.FromTimeSpan(t))  
    { }
}
