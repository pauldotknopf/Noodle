using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Noodle.Tests
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void Can_store_open_generics_as_singleton()
        {
            // arrange
            var container = new TinyIoCContainer();
            // conventions say this will store as singleton by default
            container.Register(typeof(IOpenGeneric<>), typeof(OpenGeneric<>));

            // act
            var type1 = container.Resolve<IOpenGeneric<Type1>>();
            var type2 = container.Resolve<IOpenGeneric<Type2>>();

            // assert
			type1.ShouldNotBeNull ();
			type2.ShouldNotBeNull ();

            // arrange
            type1.Value = 2;
            type2.Value = 2;

            // act
            type1 = container.Resolve<IOpenGeneric<Type1>>();
            type2 = container.Resolve<IOpenGeneric<Type2>>();

            // assert
            type1.Value.ShouldEqual(2);
            type2.Value.ShouldEqual(2);
        }

        public interface IOpenGeneric<T>
        {
            int Value { get; set; }
        }

        public class OpenGeneric<T> : IOpenGeneric<T>
        {
            public OpenGeneric()
            {
                Value = 1;
            }
        
            public int Value{get;set;}
        }

        public class Type1
        { 
        }

        public class Type2
        {
        }
    }
}
