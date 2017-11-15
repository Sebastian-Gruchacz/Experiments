using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyTests
{
    using System.Reflection;

    using Moq;

    using StructureMap;
    using StructureMap.Graph;
    using StructureMap.Graph.Scanning;

    using Xunit;

    public interface IInterfaceOne
    {

    }

    public interface IInterfaceTwo
    {

    }


    public class T1
    {
        public T1(IInterfaceOne i1, IInterfaceTwo i2)
        {

        }
    }

    public class Class1
    {
        public StructureMap.Container BuildMockedMap()
        {
            // TODO: pass overrides for interfaces that are going to be mocked manually

            Container ct = new Container();
            ct.Configure(cfg => cfg
            .Scan(scan =>
               {
                   scan.TheCallingAssembly();
                   scan.With(new MockingAllInterfacesConvention());
               }));

            return ct;
        }

        [Fact]
        public void Test()
        {
            var container = BuildMockedMap();

            var instance = container.GetInstance<T1>();
        }

    }

    public class MockingAllInterfacesConvention : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            var genericMockingType = typeof(Mock<>);

            // Only work on interface types
            var tInterfaces = types.FindTypes(TypeClassification.Interfaces);
            foreach (var type in tInterfaces)
            {
                var mockType = genericMockingType.MakeGenericType(type);
                var creationProperty = genericMockingType
                    .GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty | BindingFlags.Instance)
                    .SingleOrDefault(p => p.Name.Equals(nameof(Mock.Object)) && p.PropertyType.Equals(typeof(object)));

                var mockedMethod = creationProperty.GetMethod;

                var mock = Activator.CreateInstance(mockType);

                registry.For(type).Use(mockedMethod.Invoke(mock, null));
            };
        }
    }
}
