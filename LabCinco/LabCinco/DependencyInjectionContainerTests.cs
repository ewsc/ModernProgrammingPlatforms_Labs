namespace LabCinco;
using NUnit.Framework;

public class DependencyInjectionContainerTests
{
    public interface IService { }
    public class ServiceImpl : IService { }
    
    public interface IService2 { }
    public class ServiceImpl2 : IService2 { }

    [Test]
    public void TestDependencyResolution()
    {
        var dependencies = new DependenciesConfiguration();
        dependencies.Register<IService, ServiceImpl>();
        var provider = new DependencyProvider(dependencies);

        var service = provider.Resolve<IService>();

        Assert.That(service, Is.Not.Null);
    }
    
    [Test]
    public void TestMultipleDependencyResolution()
    {
        var dependencies = new DependenciesConfiguration();
        dependencies.Register<IService, ServiceImpl>();
        dependencies.Register<IService2, ServiceImpl2>();
        var provider = new DependencyProvider(dependencies);

        var service = provider.Resolve<IService>();
        var service2 = provider.Resolve<IService2>();

        Assert.That(service, Is.Not.Null);
        Assert.That(service2, Is.Not.Null);
    }
}