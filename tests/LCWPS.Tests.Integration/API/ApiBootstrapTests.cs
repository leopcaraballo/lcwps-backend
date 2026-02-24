using LCWPS.API.Controllers;

namespace LCWPS.Tests.Integration.API;

public class ApiBootstrapTests
{
    [Fact]
    public void HealthController_Should_Be_Discoverable()
    {
        var controllerType = typeof(HealthController);

        Assert.Equal("HealthController", controllerType.Name);
    }
}
