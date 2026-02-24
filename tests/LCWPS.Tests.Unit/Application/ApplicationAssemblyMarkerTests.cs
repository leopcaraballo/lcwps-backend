using LCWPS.Application;

namespace LCWPS.Tests.Unit.Application;

public class ApplicationAssemblyMarkerTests
{
    [Fact]
    public void ApplicationAssemblyMarker_Should_Be_Loadable()
    {
        var markerType = typeof(ApplicationAssemblyMarker);

        Assert.Equal("LCWPS.Application", markerType.Namespace);
    }
}
