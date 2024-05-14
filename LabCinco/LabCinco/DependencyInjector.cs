using System;
using System.Collections.Generic;
using System.Linq;

public interface IDependencyConfiguration
{
    void Register<TDependency, TImplementation>() where TImplementation : TDependency;
    void Register(Type dependencyType, Type implementationType);
}

public class DependenciesConfiguration : IDependencyConfiguration
{
    private readonly Dictionary<Type, Type> _dependencies = new Dictionary<Type, Type>();

    public void Register<TDependency, TImplementation>() where TImplementation : TDependency
    {
        Register(typeof(TDependency), typeof(TImplementation));
    }

    public void Register(Type dependencyType, Type implementationType)
    {
        _dependencies[dependencyType] = implementationType;
    }

    public Type GetImplementationType(Type dependencyType)
    {
        if (_dependencies.ContainsKey(dependencyType))
            return _dependencies[dependencyType];

        if (dependencyType.IsGenericType && _dependencies.ContainsKey(dependencyType.GetGenericTypeDefinition()))
            return _dependencies[dependencyType.GetGenericTypeDefinition()];

        return null;
    }
}

public class DependencyProvider
{
    private readonly DependenciesConfiguration _configuration;
    private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

    public DependencyProvider(DependenciesConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TDependency Resolve<TDependency>()
    {
        return (TDependency)Resolve(typeof(TDependency));
    }

    private object Resolve(Type dependencyType)
    {
        if (_instances.ContainsKey(dependencyType))
        {
            return _instances[dependencyType];
        }

        var implementationType = _configuration.GetImplementationType(dependencyType);
        if (implementationType == null)
        {
            throw new InvalidOperationException($"No implementation found for dependency type {dependencyType.Name}");
        }

        if (implementationType.IsGenericTypeDefinition)
        {
            implementationType = implementationType.MakeGenericType(dependencyType.GetGenericArguments());
        }

        var constructor = implementationType.GetConstructors().FirstOrDefault();
        if (constructor == null)
        {
            throw new InvalidOperationException($"No public constructor found for {implementationType.Name}");
        }

        var constructorParameters = constructor.GetParameters();
        var dependencies = new List<object>();
        foreach (var parameter in constructorParameters)
        {
            dependencies.Add(Resolve(parameter.ParameterType));
        }

        var instance = Activator.CreateInstance(implementationType, dependencies.ToArray());
        if (instance != null)
        {
            _instances[dependencyType] = instance;
        }

        return instance;
    }
}