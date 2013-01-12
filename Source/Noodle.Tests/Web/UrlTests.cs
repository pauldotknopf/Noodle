using NUnit.Framework;
using Noodle.Web;

namespace Noodle.Tests.Web
{
    [TestFixture]
    public class UrlTests
    {
        [Test]
        public void CanConstruct_HomePath()
        {
            Url u = new Url("/");
            Assert.AreEqual("/", u.Path);
            Assert.AreEqual("/", u.ToString());
        }

        [Test]
        public void EmptyUrl()
        {
            Url u = new Url("");
            Assert.AreEqual("", u.Path);
            Assert.AreEqual("", u.ToString());
        }

        [Test]
        public void NullUrl()
        {
            Url u = new Url((string)null);
            Assert.AreEqual("", u.Path);
            Assert.AreEqual("", u.ToString());
        }

        [Test]
        public void CanConstruct_AbsoluteLocalPath()
        {
            Url u = new Url("/hello.aspx");
            Assert.AreEqual("/hello.aspx", u.Path);
            Assert.AreEqual("/hello.aspx", u.ToString());
        }

        [Test]
        public void CanConstruct_AbsoluteLocalPath_WithQuery()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            Assert.AreEqual("/hello.aspx", u.Path);
            Assert.AreEqual("something=someotherthing", u.Query);
            Assert.AreEqual("/hello.aspx?something=someotherthing", u.ToString());
        }

        [Test]
        public void CanConstruct_AbsoluteLocalPath_WithFragment()
        {
            Url u = new Url("/hello.aspx#somebookmark");
            Assert.AreEqual("/hello.aspx", u.Path);
            Assert.AreEqual("somebookmark", u.Fragment);
            Assert.AreEqual("/hello.aspx#somebookmark", u.ToString());
        }

        [Test]
        public void CanConstruct_AbsoluteLocalPath_WithQuery_AndFragment()
        {
            Url u = new Url("/hello.aspx?something=someotherthing#somebookmark");
            Assert.AreEqual("/hello.aspx", u.Path);
            Assert.AreEqual("something=someotherthing", u.Query);
            Assert.AreEqual("somebookmark", u.Fragment);
            Assert.AreEqual("/hello.aspx?something=someotherthing#somebookmark", u.ToString());
        }

        [Test]
        public void CanConstruct_FromHostName()
        {
            Url u = new Url("http://somesite/");
            Assert.AreEqual("http", u.Scheme);
            Assert.AreEqual("somesite", u.Authority);
            Assert.AreEqual("http://somesite/", u.ToString());
        }

        [Test]
        public void CanConstruct_FromHostName_WithoutTrailingSlash()
        {
            Url u = new Url("http://somesite");
            Assert.AreEqual("http", u.Scheme);
            Assert.AreEqual("somesite", u.Authority);
            Assert.AreEqual("/", u.Path);
            Assert.AreEqual("http://somesite/", u.ToString());
        }

        [Test]
        public void CanConstruct_FromHostName_WithPort()
        {
            Url u = new Url("http://somesite:8080/");
            Assert.AreEqual("http", u.Scheme);
            Assert.AreEqual("somesite:8080", u.Authority);
            Assert.AreEqual("http://somesite:8080/", u.ToString());
        }

        [Test]
        public void CanConstruct_FromHostName_WithPath()
        {
            Url u = new Url("http://somesite/some/path");
            Assert.AreEqual("http", u.Scheme);
            Assert.AreEqual("somesite", u.Authority);
            Assert.AreEqual("/some/path", u.Path);
            Assert.AreEqual("http://somesite/some/path", u.ToString());
        }

        [Test]
        public void CanConstruct_FromHostName_WithPath_AndQuery()
        {
            Url u = new Url("http://somesite/some/path?key=value");
            Assert.AreEqual("http", u.Scheme);
            Assert.AreEqual("somesite", u.Authority);
            Assert.AreEqual("/some/path", u.Path);
            Assert.AreEqual("key=value", u.Query);
            Assert.AreEqual("http://somesite/some/path?key=value", u.ToString());
        }

        [Test]
        public void CanConstruct_FromHostName_WithPath_AndFragment()
        {
            Url u = new Url("http://somesite/some/path#somebookmark");
            Assert.AreEqual("http", u.Scheme);
            Assert.AreEqual("somesite", u.Authority);
            Assert.AreEqual("/some/path", u.Path);
            Assert.AreEqual("somebookmark", u.Fragment);
            Assert.AreEqual("http://somesite/some/path#somebookmark", u.ToString());
        }

        [Test]
        public void CanConstruct_FromHostName_WithPath_AndQuery_AndFragment()
        {
            Url u = new Url("http://somesite/some/path?key=value#bookmark");
            Assert.AreEqual("http", u.Scheme);
            Assert.AreEqual("somesite", u.Authority);
            Assert.AreEqual("/some/path", u.Path);
            Assert.AreEqual("key=value", u.Query);
            Assert.AreEqual("bookmark", u.Fragment);
            Assert.AreEqual("http://somesite/some/path?key=value#bookmark", u.ToString());
        }

        [Test]
        public void CanImplicitlyConvert_FromString()
        {
            Url u = "http://somesite/some/path?key=value#bookmark";
            Assert.AreEqual("http", u.Scheme);
            Assert.AreEqual("somesite", u.Authority);
            Assert.AreEqual("/some/path", u.Path);
            Assert.AreEqual("key=value", u.Query);
            Assert.AreEqual("bookmark", u.Fragment);
        }

        [Test]
        public void CanImplicitlyConvert_ToString()
        {
            Url u = "http://somesite/some/path?key=value#bookmark";
            string url = u;
            Assert.AreEqual("http://somesite/some/path?key=value#bookmark", url);
        }

        [Test]
        public void CanConstruct_RelativePath()
        {
            Url u = "../Default.aspx?key=value#bookmark";
            Assert.AreEqual("../Default.aspx", u.Path);
            Assert.AreEqual("key=value", u.Query);
            Assert.AreEqual("bookmark", u.Fragment);
        }

        [Test]
        public void CanAppend_KeyAndValue_ToEmpyQueryString()
        {
            Url u = "/";
            u = u.AppendQuery("key", "value");
            Assert.AreEqual("key=value", u.Query);
        }
        [Test]
        public void CanAppend_KeyValuePair_ToEmptyQueryString()
        {
            Url u = "/";
            u = u.AppendQuery("key=value");
            Assert.AreEqual("key=value", u.Query);
        }

        [Test]
        public void CanAppend_KeyAndValue_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.AppendQuery("key", "value");
            Assert.AreEqual("somekey=somevalue&key=value", u.Query);
        }
        [Test]
        public void CanAppend_KeyValuePair_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.AppendQuery("key=value");
            Assert.AreEqual("somekey=somevalue&key=value", u.Query);
        }

        [Test]
        public void CanSet_KeyAndValue_ToEmptyQueryString()
        {
            Url u = "/";
            u = u.SetQueryParameter("key", "value");
            Assert.AreEqual("key=value", u.Query);
        }

        [Test]
        public void CanSet_KeyValuePair_ToEmptyQueryString()
        {
            Url u = "/";
            u = u.SetQueryParameter("key=value");
            Assert.AreEqual("key=value", u.Query);
        }

        [Test]
        public void CanSet_KeyAndValue_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.SetQueryParameter("key", "value");
            Assert.AreEqual("somekey=somevalue&key=value", u.Query);
        }
        [Test]
        public void CanSet_KeyValuePair_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.SetQueryParameter("key=value");
            Assert.AreEqual("somekey=somevalue&key=value", u.Query);
        }

        [Test]
        public void CanReplaceValue_OnExistingQueryString_UsingKeyAndValue()
        {
            Url u = "/?key=somevalue";
            u = u.SetQueryParameter("key", "someothervalue");
            Assert.AreEqual("key=someothervalue", u.Query);
        }
        [Test]
        public void CanReplaceValue_OnExistingQueryString_UsingKeyValuePair()
        {
            Url u = "/?key=somevalue";
            u = u.SetQueryParameter("key=someothervalue");
            Assert.AreEqual("key=someothervalue", u.Query);
        }

        [Test]
        public void CanRemoveValue_UsingKeyAndValue()
        {
            Url u = "/";
            u = u.SetQueryParameter("key", null, true);
            Assert.IsNull(u.Query);
        }

        [Test]
        public void CanRemoveQuery()
        {
            Url u = "/?key=value&key2=value2";
            u = u.RemoveQuery("key");
            Assert.AreEqual("/?key2=value2", u.ToString());
        }

        [Test]
        public void CanRemoveQuery2()
        {
            Url u = "/?key=value&key2=value2";
            u = u.RemoveQuery("key2");
            Assert.AreEqual("/?key=value", u.ToString());
        }

        [Test]
        public void CanRemoveValue_OnExistingQueryString_UsingKeyAndValue()
        {
            Url u = "/?key=somevalue";
            u = u.SetQueryParameter("key", null, true);
            Assert.IsNull(u.Query);
        }

        [Test]
        public void AppendedValue_IsUrlEncoded()
        {
            Url u = "/";
            u = u.AppendQuery("key", "cristian & maria");
            Assert.AreEqual("key=cristian+%26+maria", u.Query);
        }

        [Test]
        public void SetValue_IsUrlEncoded()
        {
            Url u = "/key=sometihng";
            u = u.SetQueryParameter("key", "cristian & maria");
            Assert.AreEqual("key=cristian+%26+maria", u.Query);
        }

        [Test]
        public void CanSetScheme()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.SetScheme("https");
            Assert.AreEqual("https://n2cms.com/test.aspx?key=value", u.ToString());
        }

        [Test]
        public void CanSetAuthority()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.SetAuthority("n2cms.com:8080");
            Assert.AreEqual("http://n2cms.com:8080/test.aspx?key=value", u.ToString());
        }

        [Test]
        public void CanSetPath()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.SetPath("/test2.aspx");
            Assert.AreEqual("http://n2cms.com/test2.aspx?key=value", u.ToString());
        }

        [Test]
        public void CanSetQuery()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.SetQuery("key2=value2");
            Assert.AreEqual("http://n2cms.com/test.aspx?key2=value2", u.ToString());
        }

        [Test]
        public void CanSetFragment()
        {
            Url u = "http://n2cms.com/test.aspx?key=value#fragment";
            u = u.SetFragment("fragment2");
            Assert.AreEqual("http://n2cms.com/test.aspx?key=value#fragment2", u.ToString());
        }

        [Test]
        public void CanAppendSegment()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.AppendSegment("test2");
            Assert.AreEqual("http://n2cms.com/test/test2.aspx?key=value", u.ToString());
        }

        [Test]
        public void CanAppendSegment_ToEmptyPath()
        {
            Url u = "http://n2cms.com";
            u = u.AppendSegment("test2");
            Assert.AreEqual("http://n2cms.com/test2.aspx", u.ToString());
        }

        [Test]
        public void CanAppendSegment_UsingDefaultExtension_ToPathWithNoExtension()
        {
            Url u = "http://n2cms.com/path";
            u = u.AppendSegment("test2", true);
            Assert.AreEqual("http://n2cms.com/path/test2.aspx", u.ToString());
        }

        [Test]
        public void CanAppendSegment_UsingDefaultExtension_ToPathWithTrailingSlash()
        {
            Url u = "http://n2cms.com/path/";
            u = u.AppendSegment("test2", true);
            Assert.AreEqual("http://n2cms.com/path/test2.aspx", u.ToString());
        }

        [Test]
        public void Extension_WillNotCrossSegments()
        {
            Url u = "/hello.world/universe";
            Assert.IsNull(u.Extension);
        }

        [Test]
        public void PathWithoutExtension_WillNotCrossSegments()
        {
            Url u = "/hello.world/universe";
            Assert.AreEqual("/hello.world/universe", u.PathWithoutExtension);
        }

        [Test]
        public void CanAppendSegment_UsingDefaultExtension_ToEmptyPath()
        {
            Url u = "http://n2cms.com";
            u = u.AppendSegment("test2", true);
            Assert.AreEqual("http://n2cms.com/test2.aspx", u.ToString());
        }

        [Test]
        public void CanAppendSegment_ToPathWithoutExtension()
        {
            Url u = "http://n2cms.com/wiki";
            u = u.AppendSegment("test");
            Assert.AreEqual("http://n2cms.com/wiki/test", u.ToString());
        }

        [Test]
        public void CanAppendSegment_ToPathWithTrailingSlash()
        {
            Url u = "http://n2cms.com/wiki/";
            u = u.AppendSegment("test");
            Assert.AreEqual("http://n2cms.com/wiki/test", u.ToString());
        }

        [Test]
        public void CanPrependSegment_ToEmptyPath()
        {
            Url u = "http://n2cms.com";
            u = u.PrependSegment("test2");
            Assert.AreEqual("http://n2cms.com/test2.aspx", u.ToString());
        }

        [Test]
        public void CanPrependSegment_ToSimplePath()
        {
            Url u = "http://n2cms.com/test.aspx";
            u = u.PrependSegment("test2");
            Assert.AreEqual("http://n2cms.com/test2/test.aspx", u.ToString());
        }

        [Test]
        public void CanControlExtension_WhilePrependingPath_ToEmptyPath()
        {
            Url u = "http://n2cms.com/";
            u = u.PrependSegment("test", ".html");
            Assert.AreEqual("http://n2cms.com/test.html", u.ToString());
        }

        [Test]
        public void CanControlExtension_WhilePrependingPath()
        {
            Url u = "http://n2cms.com/test.aspx";
            u = u.PrependSegment("test2", ".html");
            Assert.AreEqual("http://n2cms.com/test2/test.html", u.ToString());
        }

        [Test]
        public void WillNotUse_DefaultExtension_WhenAppendingSegment_ToPathWithNoExtension()
        {
            Url u = "http://n2cms.com/test";
            u = u.AppendSegment("test2");
            Assert.AreEqual("http://n2cms.com/test/test2", u.ToString());
        }

        [Test]
        public void WillUse_UrlExtension_WhenAppendingSegment()
        {
            Url u = "http://n2cms.com/test.html";
            u = u.AppendSegment("test2");
            Assert.AreEqual("http://n2cms.com/test/test2.html", u.ToString());
        }

        [Test]
        public void CanAppendSegment_WithoutUsingDefaultExtension()
        {
            Url u = "http://n2cms.com/hello";
            u = u.AppendSegment("test2", false);
            Assert.AreEqual("http://n2cms.com/hello/test2", u.ToString());
        }

        [Test]
        public void CanRemove_TrailingSegment()
        {
            Url u = "/hello/world";

            u = u.RemoveTrailingSegment();

            Assert.AreEqual("/hello", u.ToString());
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenTrailingSlash()
        {
            Url u = "/hello/world/";

            u = u.RemoveTrailingSegment();

            Assert.AreEqual("/hello", u.ToString());
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenFileExtension()
        {
            Url u = "/hello/world.aspx";

            u = u.RemoveTrailingSegment();

            Assert.AreEqual("/hello.aspx", u.ToString());
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenSingleSegment()
        {
            Url u = "/hello";

            u = u.RemoveTrailingSegment();

            Assert.AreEqual("/", u.ToString());
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenSingleSegment_AndTrailingSlash()
        {
            Url u = "/hello/";

            u = u.RemoveTrailingSegment();

            Assert.AreEqual("/", u.ToString());
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenSingleSegment_AndExtension()
        {
            Url u = "/hello.aspx";

            u = u.RemoveTrailingSegment();

            Assert.AreEqual("/", u.ToString());
        }

        //[TestCase("/hello/world.aspx", "/hello/")]
        //[TestCase("/hello/world/", "/hello/")]
        //[TestCase("/hello.aspx", "/")]
        //[TestCase("/hello/", "/")]
        //[TestCase("/", "")]
        //[TestCase("", null)]
        //public void CanRemove_LastSegment(string input, string expected)
        //{
        //    string result = Url.RemoveLastSegment(input);

        //    Assert.That(result, Is.EqualTo(expected));
        //}

        //[TestCase("/hello/world.aspx", "world.aspx")]
        //[TestCase("/hello/world/", "world")]
        //[TestCase("/hello.aspx", "hello.aspx")]
        //[TestCase("/hello/", "hello")]
        //[TestCase("/", "")]
        //[TestCase("", null)]
        //public void CanGet_Name(string input, string expected)
        //{
        //    string result = Url.GetName(input);

        //    Assert.That(result, Is.EqualTo(expected));
        //}

        [Test]
        public void CanRemove_SegmentIndex0()
        {
            Url u = "/hello/whole/world";

            u = u.RemoveSegment(0);

            Assert.AreEqual("/whole/world", u.ToString());
        }

        [Test]
        public void CanRemove_SegmentIndex0_WhenTrailingSlash()
        {
            Url u = "/hello/whole/world/";

            u = u.RemoveSegment(0);

            Assert.AreEqual("/whole/world/", u.ToString());
        }

        [Test]
        public void CanRemove_SegmentIndex0_WhenFileExtension()
        {
            Url u = "/hello/whole/world.aspx";

            u = u.RemoveSegment(0);

            Assert.AreEqual("/whole/world.aspx", u.ToString());
        }

        [Test]
        public void CanRemove_SegmentIndex1()
        {
            Url u = "/hello/whole/world";

            u = u.RemoveSegment(1);

            Assert.AreEqual("/hello/world", u.ToString());
        }

        [Test]
        public void CanRemove_SegmentIndex1_WhenTrailingSlash()
        {
            Url u = "/hello/whole/world/";

            u = u.RemoveSegment(1);

            Assert.AreEqual("/hello/world", u.ToString());
        }

        [Test]
        public void CanRemove_SegmentIndex1_WhenFileExtension()
        {
            Url u = "/hello/whole/world.aspx";

            u = u.RemoveSegment(1);

            Assert.AreEqual("/hello/world.aspx", u.ToString());
        }

        [Test]
        public void CanRemove_LastSegmentIndex()
        {
            Url u = "/hello/whole/world";

            u = u.RemoveSegment(2);

            Assert.AreEqual("/hello/whole", u.ToString());
        }

        [Test]
        public void CanRemove_LastSegmentIndex_WhenTrailingSlash()
        {
            Url u = "/hello/whole/world/";

            u = u.RemoveSegment(2);

            Assert.AreEqual("/hello/whole", u.ToString());
        }

        [Test]
        public void CanRemove_LastSegmentIndex_WhenFileExtension()
        {
            Url u = "/hello/whole/world.aspx";

            u = u.RemoveSegment(2);

            Assert.AreEqual("/hello/whole.aspx", u.ToString());
        }

        [Test]
        public void RemoveSegment_GracefullyExcepts_ArgumentsBeyondBounds()
        {
            Url u = "/hello.aspx";

            u = u.RemoveSegment(2);

            Assert.AreEqual("/hello.aspx", u.ToString());
        }

        [Test]
        public void RemoveSegment_GracefullyExcepts_BeforeBounds()
        {
            Url u = "/hello.aspx";

            u = u.RemoveSegment(-1);

            Assert.AreEqual("/hello.aspx", u.ToString());
        }

        [Test]
        public void CanAppendSegment_WithoutUsingDefaultExtension_ToEmptyPath()
        {
            Url u = "http://n2cms.com";
            u = u.AppendSegment("test2", false);
            Assert.AreEqual("http://n2cms.com/test2", u.ToString());
        }

        [Test]
        public void CanAppendSegment_ToEmptyPath_WithTrailingSlash()
        {
            Url u = "http://n2cms.com/";
            u = u.AppendSegment("test2");
            Assert.AreEqual("http://n2cms.com/test2.aspx", u.ToString());
        }

        [Test]
        public void CanSetPath_WithQueryString()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            u = u.SetPath("/new/path?new=querystring");
            Assert.AreEqual("http://n2cms.com/new/path?existing=query", u.ToString());
        }

        [Test]
        public void CanCombine_PathAndQuery()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            Assert.AreEqual("/some/path.aspx?existing=query", u.PathAndQuery);
        }

        [Test]
        public void CanParsePath_WithSlashInQuery()
        {
            Url u = "/UI/500.aspx?aspxerrorpath=/Default.aspx";
            Assert.AreEqual("/UI/500.aspx", u.Path);
            Assert.AreEqual("aspxerrorpath=/Default.aspx", u.Query);
        }

        [Test]
        public void CanDetermine_DefaultExtension()
        {
            Url u = "/path/to/page.aspx";
            Assert.AreEqual(".aspx", u.Extension);
        }

        [Test]
        public void CanDetermine_HtmlExtension()
        {
            Url u = "/path/to/page.html";
            Assert.AreEqual(".html", u.Extension);
        }

        [Test]
        public void CanDetermine_NoExtension()
        {
            Url u = "/path/to/page";
            Assert.IsNull(u.Extension);
        }

        [Test]
        public void CanDetermine_NoExtension_WhenTrailingSlash()
        {
            Url u = "/path/to/page/";
            Assert.IsNull(u.Extension);
        }

        [Test]
        public void CanSplitPath_IntoPathWithoutExtension()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            Assert.AreEqual("/hello", u.PathWithoutExtension);
        }

        [Test]
        public void CanGet_QueryDictionary()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            var q = u.GetQueries();
            Assert.AreEqual(1, q.Count);
            Assert.AreEqual("someotherthing", q["something"]);
        }

        [Test]
        public void CanGet_EmptyQueryDictionary()
        {
            Url u = new Url("/hello.aspx");
            var q = u.GetQueries();
            Assert.AreEqual(0, q.Count);
        }

        [Test]
        public void CanGet_Query()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            var q = u.GetQuery("something");
            Assert.AreEqual("someotherthing", q);
        }

        [Test]
        public void Getting_NonExistantQuery_GivesNull()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            var q = u.GetQuery("nothing");
            Assert.IsNull(q);
        }

        [Test]
        public void Getting_Query_WhenNoQueries_GivesNull()
        {
            Url u = new Url("/hello.aspx");
            var q = u.GetQuery("something");;
            Assert.IsNull(q);
        }

        [Test]
        public void UpdatingQueryToNull_WhenSingleParameter_RemovesItFromUrl()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            u = u.SetQueryParameter("something", null);
            Assert.AreEqual("/hello.aspx", u.ToString());
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameter_WhenUpdatingFirst()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value");
            u = u.SetQueryParameter("something", null);
            Assert.AreEqual("/hello.aspx?query=value", u.ToString());
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameter_WhenUpdatingSecond()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value");
            u = u.SetQueryParameter("query", null);
            Assert.AreEqual("/hello.aspx?something=someotherthing", u.ToString());
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameters_WhenUpdatingFirst()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value&query3=value3");
            u = u.SetQueryParameter("something", null);
            Assert.AreEqual("/hello.aspx?query=value&query3=value3", u.ToString());
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameters_WhenUpdatingInMiddle()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value&query3=value3");
            u = u.SetQueryParameter("query", null);
            Assert.AreEqual("/hello.aspx?something=someotherthing&query3=value3", u.ToString());
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameters_WhenUpdatingLast()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value&query3=value3");
            u = u.SetQueryParameter("query3", null);
            Assert.AreEqual("/hello.aspx?something=someotherthing&query=value", u.ToString());
        }

        [Test]
        public void CanChangeExtension()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            u = u.SetExtension(".html");
            Assert.AreEqual("/hello.html?something=someotherthing", u.ToString());
        }

        [Test]
        public void CanClearExtension()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            u = u.SetExtension("");
            Assert.AreEqual("/hello?something=someotherthing", u.ToString());
        }

        [Test]
        public void CanSplitOut_HostPart()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            u = u.HostUrl;
            Assert.AreEqual("http://n2cms.com", u.ToString());
        }

        [Test]
        public void CanSplitOut_HostPart_AndGetExtension()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            string extension = u.HostUrl.Extension;
            Assert.IsNull(extension);
        }

        [Test]
        public void CanSplitOut_LocalPart()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            u = u.LocalUrl;
            Assert.AreEqual("/some/path.aspx?existing=query", u.ToString());
        }

        [Test]
        public void CanConstruct_WithBaseSchemeAndRawUrl()
        {
            Url u = new Url("http", "www.n2cms.com", "/Default.aspx?");
            Assert.AreEqual("http", u.Scheme);
            Assert.AreEqual("www.n2cms.com", u.Authority);
            Assert.AreEqual("/Default.aspx", u.PathAndQuery);
        }

        [Test]
        public void CanRecognize_AbsoluteUrl()
        {
            Url u = "http://www.n2cms.com";
            Assert.IsTrue(u.IsAbsolute);
        }

        [Test]
        public void CanRecognize_LocalUrl()
        {
            Url u = "/";
            Assert.IsFalse(u.IsAbsolute);
        }

        //[TestCase("", "", "")]
        //[TestCase("", "/", "/")]
        //[TestCase("/", "", "/")]
        //[TestCase("/", "/", "/")]
        //[TestCase("", "hello", "hello")]
        //[TestCase("/", "/hello", "/hello")]
        //[TestCase("/hello", "hello", "/hello/hello")]
        //[TestCase("/hello", "/hello", "/hello")]
        //[TestCase("/hello.aspx", "hello.aspx", "/hello.aspx/hello.aspx")]
        //[TestCase("/hello", "hello?one=1", "/hello/hello?one=1")]
        //[TestCase("/hello?one=1", "hello", "/hello/hello?one=1")]
        //[TestCase("/hello?one=1", "hello?two=2", "/hello/hello?one=1&two=2")]
        //[TestCase("/hello?one=1&two=2", "hello?three=3&four=4", "/hello/hello?one=1&two=2&three=3&four=4")]
        //[TestCase("/n2/", "{selected}", "{selected}")]
        //[TestCase("/n2/", "javascript:alert(1);", "javascript:alert(1);")]
        //[TestCase("/hello", "hello#world", "/hello/hello#world")]
        //public void Combine1(string url1, string url2, string expected)
        //{
        //    string result = Url.Combine(url1, url2);
        //    Assert.That(result, Is.EqualTo(expected), "'" + url1 + "' + '" + url2 + "' != '" + expected + "'");
        //}

        [Test]
        public void CanParse_RelativePath_WithUrl_AsQuery()
        {
            Url u = "/path?dns=host.com&url=http://www.hello.net/howdy";
            Assert.AreEqual("/path", u.Path);
            Assert.AreEqual("dns=host.com&url=http://www.hello.net/howdy", u.Query);
        }

        //[Test]
        //public void ServerUrl_Returns_FallbackValue_InThreadContext()
        //{
        //    var oldEngine = Singleton<IEngine>.Instance;
        //    try
        //    {
        //        var engine = new FakeEngine();
        //        var webContext = new FakeWebContextWrapper();
        //        webContext.isWeb = true;
        //        engine.Container.AddComponentInstance("", typeof(IWebContext), webContext);
        //        Singleton<IEngine>.Instance = engine;

        //        webContext.Url = "http://site1/app";
        //        Assert.That(Url.ServerUrl, Is.EqualTo("http://site1"));

        //        webContext.isWeb = false;
        //        webContext.Url = "http://site2/app";
        //        Assert.That(Url.ServerUrl, Is.EqualTo("http://site1"));
        //    }
        //    finally
        //    {
        //        Singleton<IEngine>.Instance = oldEngine;
        //    }
        //}

        //[Test]
        //public void ServerUrl_Returns_DifferentValues_InRequestForDifferentSites()
        //{
        //    var oldEngine = Singleton<IEngine>.Instance;
        //    try
        //    {
        //        var engine = new FakeEngine();
        //        var webContext = new FakeWebContextWrapper();
        //        webContext.isWeb = true;
        //        engine.Container.AddComponentInstance("", typeof(IWebContext), webContext);
        //        Singleton<IEngine>.Instance = engine;

        //        webContext.Url = "http://site1/app";
        //        Assert.That(Url.ServerUrl, Is.EqualTo("http://site1"));
        //        webContext.Url = "http://site2/app";
        //        Assert.That(Url.ServerUrl, Is.EqualTo("http://site2"));
        //    }
        //    finally
        //    {
        //        Singleton<IEngine>.Instance = oldEngine;
        //    }
        //}

        [Test]
        public void Resolve_CanChange_DefaultReplacements()
        {
            string backup = Url.GetToken("{ManagementUrl}");

            try
            {
                Url.SetToken("{ManagementUrl}", "/Manage");

                string result = Url.ResolveTokens("{ManagementUrl}/Resources/Icons/add.png");

                Assert.AreEqual("/Manage/Resources/Icons/add.png", result);
            }
            finally
            {
                Url.SetToken("{ManagementUrl}", backup);
            }
        }

        [Test]
        public void Resolve_CanAdd_Replcement()
        {
            string backup = Url.GetToken("{HelloUrl}");

            try
            {
                Url.SetToken("{HelloUrl}", "/Hello/World");

                string result = Url.ResolveTokens("{HelloUrl}/Resources/Icons/add.png");

                Assert.AreEqual("/Hello/World/Resources/Icons/add.png", result);
            }
            finally
            {
                Url.SetToken("{HelloUrl}", backup);
            }
        }

        [Test]
        public void Resolve_CanClear_Replcement()
        {
            string backup = Url.GetToken("{ManagementUrl}");

            try
            {
                Url.SetToken("{ManagementUrl}", null);

                string result = Url.ResolveTokens("{ManagementUrl}/Resources/Icons/add.png");

                Assert.AreEqual("{ManagementUrl}/Resources/Icons/add.png", result);
            }
            finally
            {
                Url.SetToken("{ManagementUrl}", backup);
            }
        }

        [Test]
        public void Resolve_DoesntReplace_UnknownTokens()
        {
            string result = Url.ResolveTokens("{HelloUrl}/Resources/Icons/add.png");

            Assert.AreEqual("{HelloUrl}/Resources/Icons/add.png", result);
        }

        [Test]
        public void Resolve_MakesPath_ToAbsolute()
        {
            Url.ApplicationPath = "/hello/";
            try
            {
                string result = Url.ResolveTokens("~/N2/Resources/Icons/add.png");

                Assert.AreEqual("/hello/N2/Resources/Icons/add.png", result);
            }
            finally
            {
                Url.ApplicationPath = "/";
            }
        }

        [Test]
        public void RemoveExtension_RemovesAnyExtension()
        {
            Url url = "/hello.aspx";
            string result = url.RemoveExtension();

            Assert.AreEqual("/hello", result);
        }

        [Test]
        public void RemoveExtension_RemovesValidExtension()
        {
            Url url = "/hello.aspx";
            string result = url.RemoveExtension(".aspx");

            Assert.AreEqual("/hello", result);
        }

        [Test]
        public void RemoveExtension_DesontRemove_InvalidExtension()
        {
            Url url = "/hello.gif";
            string result = url.RemoveExtension(".aspx");

            Assert.AreEqual("/hello.gif", result);
        }
    }
}
