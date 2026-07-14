using PontoEletronico.Application.Common.Interfaces;

namespace PontoEletronico.UnitTests.Common;

/// <summary>
/// Relógio controlável, para que os testes não dependam da data/hora real da máquina.
/// </summary>
public class FakeDateTimeProvider : IDateTimeProvider
{
    public FakeDateTimeProvider(DateTime now) => Now = now;

    public DateTime Now { get; set; }
}
