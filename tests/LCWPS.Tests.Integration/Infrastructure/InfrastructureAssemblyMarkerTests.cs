using LCWPS.Infrastructure;

namespace LCWPS.Tests.Integration.Infrastructure;

public class InfrastructureAssemblyMarkerTests
{
    [Fact]
    public void InfrastructureAssemblyMarker_Should_Be_Loadable()
    {
        var markerType = typeof(InfrastructureAssemblyMarker);

        Assert.Equal("LCWPS.Infrastructure", markerType.Namespace);
    }
}
