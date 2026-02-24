using LCWPS.Domain;

namespace LCWPS.Tests.Unit.Domain;

public class DomainAssemblyMarkerTests
{
    [Fact]
    public void DomainAssemblyMarker_Should_Be_Loadable()
    {
        var markerType = typeof(DomainAssemblyMarker);

        Assert.Equal("LCWPS.Domain", markerType.Namespace);
    }
}
