namespace DependencyTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Conditions;

    using Moq;

    using Shouldly;

    using StructureMap;
    using StructureMap.Graph;
    using StructureMap.Graph.Scanning;

    using Xunit;

    public interface IInterfaceOne
    {
        void CallMeOne();
    }

    public interface IInterfaceTwo
    {
        void CallMeTwo();
    }

    class ImplementationOne : IInterfaceOne
    {
        /// <inheritdoc />
        public void CallMeOne()
        {
            throw new InvalidOperationException("Should not be called!");
        }
    }

    class ImplementationTwo : IInterfaceTwo
    {
        /// <inheritdoc />
        public void CallMeTwo()
        {
            throw new InvalidOperationException("Should not be called!");
        }
    }

    public class TestedService
    {
        private readonly IInterfaceOne _i1;
        private readonly IInterfaceTwo _i2;

        public TestedService(IInterfaceOne i1, IInterfaceTwo i2)
        {
            i1.Requires().IsNotNull();
            i2.Requires().IsNotNull();

            this._i1 = i1;
            this._i2 = i2;
        }

        public void TestCall()
        {
            this._i1.CallMeOne();
            this._i2.CallMeTwo();
        }
    }

    public class TestClass
    {
        // Perhaps move together with DI container into some shared class? Or test base class?
        private Container BuildMockedMap(Action<MockingReplacements> replacements)
        {
            MockingReplacements rep = new MockingReplacements();
            if (replacements != null)
            {
                replacements(rep);
            }

            Container ct = new Container();
            ct.Configure(cfg => cfg
            .Scan(scan =>
               {
                   scan.TheCallingAssembly();
                   scan.With(new MockingAllInterfacesConvention(rep));
               }));

            return ct;
        }

        [Fact]
        public void Test()
        {
            bool mockCalledFlag = false;

            var oneMock = new Mock<IInterfaceOne>();
            oneMock.Setup(m => m.CallMeOne())
                .Callback(() =>
                {
                    // verification sample, can of course use here any Setup() mocking options
                    mockCalledFlag = true;
                });

            var container = this.BuildMockedMap(replacements =>
            {
                replacements.Add<IInterfaceOne>(oneMock.Object);
                // add more replacements here...
            });

            
            var instance = container.GetInstance<TestedService>();

            instance.TestCall();

            mockCalledFlag.ShouldBeTrue();
        }

    }

    public class MockingReplacements
    {
        private readonly Dictionary<Type, object> _replacements = new Dictionary<Type, object>();

        public Dictionary<Type, object> Replacements
        {
            get { return this._replacements; }
        }

        public void Add<TInterface>(object instance)
        {
            instance.Requires().IsNotNull().IsOfType(typeof(TInterface));

            this._replacements.Add(typeof(TInterface), instance);
        }
    }

    public class MockingAllInterfacesConvention : IRegistrationConvention
    {
        private readonly MockingReplacements _rep;

        public MockingAllInterfacesConvention(MockingReplacements rep)
        {
            rep.Requires().IsNotNull();

            this._rep = rep;
        }

        public void ScanTypes(TypeSet types, Registry registry)
        {
            var genericMockingType = typeof(Mock<>);

            // Only work on interface types
            var tInterfaces = types.FindTypes(TypeClassification.Interfaces);
            foreach (var type in tInterfaces)
            {
                // ignoring provided interfaces seems the fastest (and simplest).
                // Another possibility - overwrite what is returned by Convention object in calling class

                object replacement;
                if (this._rep.Replacements.TryGetValue(type, out replacement))
                {
                    registry.For(type).Use(replacement);
                }
                else
                {
                    var mockType = genericMockingType.MakeGenericType(type);
                    var creationProperty = genericMockingType
                        .GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty |
                                       BindingFlags.Instance)
                        .SingleOrDefault(p =>
                            p.Name.Equals(nameof(Mock.Object)) && p.PropertyType.Equals(typeof(object)));

                    var mockedMethod = creationProperty.GetMethod;

                    var mock = Activator.CreateInstance(mockType);

                    registry.For(type).Use(mockedMethod.Invoke(mock, null));
                }
            };
        }
    }
}
