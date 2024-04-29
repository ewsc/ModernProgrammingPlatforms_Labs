using NUnit.Framework;

namespace LabTres.Tests
{
    [TestFixture]
    public class AssemblyInfoTests
    {
        private string assemblyPath = @"X:/1Third_Year/MPP/ModernProgrammingPlatforms_Labs/LabTres/LabTres/bin/Debug/net8.0-windows/LabTres.dll";
        private AssemblyInfo assemblyInfo;

        [SetUp]
        public void Setup()
        {
            assemblyInfo = new AssemblyInfo(assemblyPath);
        }

        [Test]
        public void LoadAssembly_ValidPath_PopulatesNamespaces()
        {
            Assert.That(assemblyInfo.Namespaces, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void GetOrCreateNamespace_NewNamespace_ReturnsNewNamespaceInfo()
        {
            string namespaceName = "MyNamespace";

            NamespaceInfo namespaceInfo = assemblyInfo.GetOrCreateNamespace(namespaceName);

            Assert.That(namespaceInfo, Is.Not.Null);
            Assert.That(namespaceInfo.Name, Is.EqualTo(namespaceName));
            Assert.That(assemblyInfo.Namespaces, Contains.Item(namespaceInfo));
        }

        [Test]
        public void GetOrCreateNamespace_ExistingNamespace_ReturnsExistingNamespaceInfo()
        {
            string namespaceName = "System";

            NamespaceInfo namespaceInfo = assemblyInfo.GetOrCreateNamespace(namespaceName);

            Assert.That(namespaceInfo, Is.Not.Null);
            Assert.That(namespaceInfo.Name, Is.EqualTo(namespaceName));
            Assert.That(assemblyInfo.Namespaces, Contains.Item(namespaceInfo));
        }
    }

    public class ParameterInfo
    {
        public Type ParameterType { get; }
        public string Name { get; }

        public ParameterInfo(Type parameterType, string name)
        {
            ParameterType = parameterType;
            Name = name;
        }
    }
    
    public class TestClass
    {
        public static void TestMethod()
        {
        }
    }
}