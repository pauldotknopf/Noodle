using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Noodle.Data;

namespace Noodle.Tests
{
    [TestClass]
    public class EmbeddedSchemaPlannerTests
    {
        private readonly IEmbeddedSchemaPlanner _planner;
        public EmbeddedSchemaPlannerTests()
        {
            _planner = new EmbeddedSchemaPlanner();
        }

        //  ---Schema dependency graph---
        //  Schema1
        //      Schema2
        //      Schema5
        //  Schema3
        //      Schema1
        //  Schema4
        //      Schema3
        //      Schema5
        //  Schema5

        [TestMethod]
        public void Can_resolve_shema1()
        {
            var schemas = _planner.GetPlansFor(new EmbeddedSchema1());

            schemas.Count.ShouldEqual(3);
            schemas[0].ShouldBe<EmbeddedSchema2>();
            schemas[1].ShouldBe<EmbeddedSchema5>();
            schemas[2].ShouldBe<EmbeddedSchema1>();
        }

        [TestMethod]
        public void Can_resolve_shema2()
        {
            var schemas = _planner.GetPlansFor(new EmbeddedSchema2());

            schemas.Count.ShouldEqual(1);
            schemas[0].ShouldBe<EmbeddedSchema2>();
        }

        [TestMethod]
        public void Can_resolve_shema3()
        {
            var schemas = _planner.GetPlansFor(new EmbeddedSchema3());

            schemas.Count.ShouldEqual(4);
            schemas[0].ShouldBe<EmbeddedSchema2>();
            schemas[1].ShouldBe<EmbeddedSchema5>();
            schemas[2].ShouldBe<EmbeddedSchema1>();
            schemas[3].ShouldBe<EmbeddedSchema3>();
        }

        [TestMethod]
        public void Can_resolve_shema4()
        {
            var schemas = _planner.GetPlansFor(new EmbeddedSchema4());

            schemas.Count.ShouldEqual(5);
            schemas[0].ShouldBe<EmbeddedSchema2>();
            schemas[1].ShouldBe<EmbeddedSchema5>();
            schemas[2].ShouldBe<EmbeddedSchema1>();
            schemas[3].ShouldBe<EmbeddedSchema3>();
            schemas[4].ShouldBe<EmbeddedSchema4>();
        }

        [TestMethod]
        public void Can_resolve_shema5()
        {
            var schemas = _planner.GetPlansFor(new EmbeddedSchema5());

            schemas.Count.ShouldEqual(1);
            schemas[0].ShouldBe<EmbeddedSchema5>();
        }
    }

    public abstract class BaseEmbeddedSchema : VSDBCMDEmbeddedSchemaProvider
    {
        public override System.Reflection.Assembly GetContainingAssembly()
        {
            return typeof (BaseEmbeddedSchema).Assembly;
        }

        public override string ResourceNamePrefix
        {
            get { return string.Empty; }
        }
    }

    public class EmbeddedSchema1 : BaseEmbeddedSchema
    {
        public override List<AbstraceEmbeddedSchemaProvider> GetDependentSchemaProviders()
        {
            return new List<AbstraceEmbeddedSchemaProvider>
                       {
                           new EmbeddedSchema2(),
                           new EmbeddedSchema5()
                       };
        }
    }

    public class EmbeddedSchema2 : BaseEmbeddedSchema
    {
        public override List<AbstraceEmbeddedSchemaProvider> GetDependentSchemaProviders()
        {
            return new List<AbstraceEmbeddedSchemaProvider>();
        }
    }

    public class EmbeddedSchema3 : BaseEmbeddedSchema
    {
        public override List<AbstraceEmbeddedSchemaProvider> GetDependentSchemaProviders()
        {
            return new List<AbstraceEmbeddedSchemaProvider>
                       {
                           new EmbeddedSchema1()
                       };
        }
    }

    public class EmbeddedSchema4 : BaseEmbeddedSchema
    {
        public override List<AbstraceEmbeddedSchemaProvider> GetDependentSchemaProviders()
        {
            return new List<AbstraceEmbeddedSchemaProvider>
                       {
                           new EmbeddedSchema3(),
                           new EmbeddedSchema5()
                       };
        }
    }

    public class EmbeddedSchema5 : BaseEmbeddedSchema
    {
        public override List<AbstraceEmbeddedSchemaProvider> GetDependentSchemaProviders()
        {
            return new List<AbstraceEmbeddedSchemaProvider>();
        }
    }
}
