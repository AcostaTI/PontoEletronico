using PontoEletronico.Application.Common.Interfaces;

namespace PontoEletronico.Infrastructure.Common;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}
