using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Engine
{
    /// <summary>
    /// Represents as
    /// </summary>
    public class ExcludedAssemblies : IExcludedAssemblies
    {
        public List<string> Assemblies { get; set; }

        public ExcludedAssemblies()
        {
            Assemblies = new List<string>();
            Assemblies.Add("^System|^mscorlib|^Microsoft|^CppCodeProvider|^VJSharpCodeProvider|^WebDev|^Castle|^Iesi|^log4net|^NHibernate|^nunit|^TestDriven|^MbUnit|^Rhino|^QuickGraph|^TestFu|^Telerik|^ComponentArt|^MvcContrib|^AjaxControlToolkit|^Antlr3|^Remotion|^Recaptcha|^Lucene|^Ionic|^HibernatingRhinos|^Spark|^SharpArch|^CommonServiceLocator|^Newtonsoft|^SMDiagnostics|^App_LocalResources|^AntiXSSLibrary|^dotless|^HtmlSanitizationLibrary|^sqlce|^WindowsBase|^Pandora|^PegBase|^DynamicProxyGenAssembly|^Anonymously Hosted DynamicMethods Assembly|^WebActivator|^Deleporter|^Elmah|^Markdown|^SimpleHttpClient|^StructureMap|^WebDriver|^MySql|^App_GlobalResources|^App_global|^App_Web_|^EntityFramework|^WebGrease|^App_global.asax|^ICSharpCode");
        }

        public IExcludedAssemblies Assembly(string assembly)
        {
            Assemblies.Add(assembly);
            return this;
        }

        public IExcludedAssemblies AndAssembly(string assembly)
        {
            return Assembly(assembly);
        }

        public IIncludedAssemblies Including
        {
            get { return EngineContext.Including; }
        }

        public IIncludedOnlyAssemblies IncludingOnly
        {
            get { return EngineContext.IncludingOnly; }
        }
    }
}
