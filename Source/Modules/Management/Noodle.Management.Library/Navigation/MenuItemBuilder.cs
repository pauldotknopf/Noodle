using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Builder;
using MvcSiteMapProvider.Collections.Specialized;
using MvcSiteMapProvider.Reflection;

namespace Noodle.Management.Library.Navigation
{
    public class MenuItemBuilder
    {
        private readonly ISiteMapNodeHelper _helper;
        private readonly IList<MenuItemBuilder> _children = new List<MenuItemBuilder>();
        private string _area;
        private string _controller;
        private string _action;
        private string _httpMethod;
        private string _title;
        private string _description;
        private string _key;
        private string _url;
        private bool? _clickable;
        private string[] _roles;
        private string _resourceKey;
        private string _visibilityProvider;
        private string _dynamicNodeProvider;
        private string _imageUrl;
        private string _targetFrame;
        private bool? _cacheResolvedUrl;
        private string _canonicalUrl;
        private string _canonicalKey;
        private string[] _metaRobotsValues;
        private ChangeFrequency? _changeFrequency;
        private UpdatePriority? _updatePriority;
        private DateTime? _lastModifiedDate;
        private int? _order;
        private string _route ;
        private object _routeValues;
        private string[] _preservedRouteParameters;
        private string _urlResolver;
        private string[] _inheritedRouteParameters;
        
        public MenuItemBuilder(ISiteMapNodeHelper helper)
        {
            _helper = helper;
        }

        public MenuItemBuilder Items(Action<MenuItemFactory> children)
        {
            if (children == null)
                return this;

            var factory = new MenuItemFactory(_children, _helper);
            children(factory);

            return this;
        }

        public MenuItemBuilder SetArea(string value)
        {
            _area = value;
            return this;
        }

        public string Area
        {
            get { return _area; }
        }

        public MenuItemBuilder SetController(string value)
        {
            _controller = value;
            return this;
        }

        public string Controller
        {
            get { return _controller; }
        }

        public MenuItemBuilder SetAction(string value)
        {
            _action = value;
            return this;
        }

        public string Action
        {
            get { return _action; }
        }

        public MenuItemBuilder SetHttpMethod(HttpVerbs? method)
        {
            _httpMethod = method.HasValue ? method.ToString().ToUpperInvariant() : null;
            return this;
        }

        public string HttpMethod
        {
            get { return _httpMethod; }
        }

        public MenuItemBuilder SetTitle(string value)
        {
            _title = value;
            return this;
        }

        public string Title
        {
            get { return _title; }
        }

        public MenuItemBuilder SetDescription(string value)
        {
            _description = value;
            return this;
        }

        public string Description
        {
            get { return _description; }
        }

        public MenuItemBuilder SetKey(string value)
        {
            _key = value;
            return this;
        }

        public string Key
        {
            get { return _key; }
        }

        public MenuItemBuilder SetUrl(string value)
        {
            _url = value;
            return this;
        }

        public string Url
        {
            get { return _url; }
        }

        public MenuItemBuilder SetClickable(bool? clickable)
        {
            _clickable = clickable;
            return this;
        }

        public bool? Clickable
        {
            get { return _clickable; }
        }

        public MenuItemBuilder SetRoles(string[] values)
        {
            _roles = values != null ? values.Where(x => !string.IsNullOrEmpty(x)).ToArray() : null;
            return this;
        }

        public string[] Roles
        {
            get { return _roles; }
        }

        public MenuItemBuilder SetResourceKey(string value)
        {
            _resourceKey = value;
            return this;
        }

        public string ResourceKey
        {
            get { return _resourceKey; }
        }

        public MenuItemBuilder SetVisibilityProvider(string value)
        {
            _visibilityProvider = value;
            return this;
        }

        public string VisibilityProvider
        {
            get { return _visibilityProvider; }
        }

        public MenuItemBuilder SetDynamicNodeProvider(string value)
        {
            _dynamicNodeProvider = value;
            return this;
        }

        public string DynamicNodeProvider
        {
            get { return _dynamicNodeProvider; }
        }

        public MenuItemBuilder SetImageUrl(string value)
        {
            _imageUrl = value;
            return this;
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
        }

        public MenuItemBuilder SetTargetFrame(string value)
        {
            _targetFrame = value;
            return this;
        }

        public string TargetFrame
        {
            get { return _targetFrame; }
        }

        public MenuItemBuilder SetCachedResolvedUrl(bool? cacheResolvedUrl)
        {
            _cacheResolvedUrl = cacheResolvedUrl;
            return this;
        }

        public bool? CacheResolvedUrl
        {
            get { return _cacheResolvedUrl; }
        }

        public MenuItemBuilder SetCanonicalUrl(string value)
        {
            _canonicalUrl = value;
            return this;
        }

        public string CanonicalUrl
        {
            get { return _canonicalUrl; }
        }

        public MenuItemBuilder SetCanonicalKey(string value)
        {
            _canonicalKey = value;
            return this;
        }

        public string CanonicalKey
        {
            get { return _canonicalKey; }
        }

        public MenuItemBuilder SetMetaRobotsValues(string[] values)
        {
            _metaRobotsValues = values != null ? values.Where(x => !string.IsNullOrEmpty(x)).ToArray() : null;
            return this;
        }

        public string[] MetaRobotsValues
        {
            get { return _metaRobotsValues; }
        }

        public MenuItemBuilder SetChangeFrequency(ChangeFrequency? value)
        {
            _changeFrequency = value;
            return this;
        }

        public ChangeFrequency? ChangeFrequency
        {
            get { return _changeFrequency; }
        }

        public MenuItemBuilder SetUpdatePriority(UpdatePriority? value)
        {
            _updatePriority = value;
            return this;
        }

        public UpdatePriority? UpdatePriority
        {
            get { return _updatePriority; }
        }

        public MenuItemBuilder SetLastModifiedDate(DateTime? value)
        {
            _lastModifiedDate = value;
            return this;
        }

        public DateTime? LastModifiedDate
        {
            get { return _lastModifiedDate; }
        }

        public MenuItemBuilder SetOrder(int order)
        {
            _order = order;
            return this;
        }

        public int? Order
        {
            get { return _order; }
        }

        public MenuItemBuilder SetRoute(string value)
        {
            _route = value;
            return this;
        }

        public string Route
        {
            get { return _route; }
        }

        public MenuItemBuilder SetRouteValues(object routeValues)
        {
            _routeValues = routeValues;
            return this;
        }

        public object RouteValues
        {
            get { return _routeValues; }
        }

        public MenuItemBuilder SetPreservedRouteValues(string[] values)
        {
            _preservedRouteParameters = values != null ? values.Where(x => !string.IsNullOrEmpty(x)).ToArray() : null;
            return this;
        }

        public string[] PreservedRouteParameters
        {
            get { return _preservedRouteParameters; }
        }

        public MenuItemBuilder SetUrlResolver(string value)
        {
            _urlResolver = value;
            return this;
        }

        public string UrlResolver
        {
            get { return _urlResolver; }
        }

        public MenuItemBuilder SetInheritedRouteParameters(string[] values)
        {
            _inheritedRouteParameters = values != null ? values.Where(x => !string.IsNullOrEmpty(x)).ToArray() : null;
            return this;
        }

        public string[] InheritedRouteParameters
        {
            get { return _inheritedRouteParameters; }
        }

        public IList<MenuItemBuilder> Children { get { return _children; } }

        #region Node Building

        public string CreateNodeKey(ISiteMapNodeHelper helper, string parentKey)
        {
            return helper.CreateNodeKey(
                parentKey,
                Key ?? string.Empty,
                Url ?? string.Empty,
                Title ?? string.Empty,
                Area ?? string.Empty,
                Controller ?? string.Empty,
                Action ?? string.Empty,
                HttpMethod ?? "GET",
                !Clickable.HasValue || Clickable.Value);
        }

        public ISiteMapNodeToParentRelation CreateNode(ISiteMapNodeHelper helper, ISiteMapNode parentNode)
        {
            var parentKey = parentNode == null ? "" : parentNode.Key;
            var key = CreateNodeKey(helper, parentKey);

            var nodeParentMap = helper.CreateNode(key, parentKey, "Fluent", ResourceKey ?? string.Empty);
            var siteMapNode = nodeParentMap.Node;

            AddAttributesToNode(siteMapNode);
            if (_routeValues != null)
                foreach (var routeValue in new System.Web.Routing.RouteValueDictionary(_routeValues))
                    siteMapNode.RouteValues.Add(routeValue);
            if (PreservedRouteParameters != null)
                foreach (var preservedRouteParameter in PreservedRouteParameters)
                    siteMapNode.PreservedRouteParameters.Add(preservedRouteParameter);

            siteMapNode.Title = Title ?? string.Empty;
            siteMapNode.Description = string.IsNullOrEmpty(Description) ? Title : Description;
            if(Roles != null)
                foreach(var role in Roles)
                    siteMapNode.Roles.Add(role);
            siteMapNode.Clickable = !Clickable.HasValue || Clickable.Value;
            siteMapNode.VisibilityProvider = VisibilityProvider ?? string.Empty;
            siteMapNode.DynamicNodeProvider = DynamicNodeProvider ?? string.Empty;
            siteMapNode.ImageUrl = ImageUrl ?? string.Empty;
            siteMapNode.TargetFrame = TargetFrame ?? string.Empty;
            siteMapNode.HttpMethod = HttpMethod ?? "GET";
            siteMapNode.Url = Url ?? string.Empty;
            siteMapNode.CacheResolvedUrl = !CacheResolvedUrl.HasValue || CacheResolvedUrl.Value;
            siteMapNode.CanonicalUrl = CanonicalUrl ?? string.Empty;
            siteMapNode.CanonicalKey = CanonicalKey ?? string.Empty;
            if(MetaRobotsValues != null)
                foreach(var metaRobotValue in MetaRobotsValues)
                    siteMapNode.MetaRobotsValues.Add(metaRobotValue);
            siteMapNode.ChangeFrequency = ChangeFrequency.HasValue ? ChangeFrequency.Value : MvcSiteMapProvider.ChangeFrequency.Undefined;
            siteMapNode.UpdatePriority = ChangeFrequency.HasValue ? UpdatePriority.Value : MvcSiteMapProvider.UpdatePriority.Undefined;
            siteMapNode.LastModifiedDate = LastModifiedDate.HasValue ? LastModifiedDate.Value : DateTime.MinValue;
            siteMapNode.Order = Order.HasValue ? Order.Value : 0;
            siteMapNode.Route = Route ?? string.Empty;
            siteMapNode.UrlResolver = UrlResolver ?? string.Empty;
            if (InheritedRouteParameters != null && parentNode != null)
            {
                foreach (var inheritedRouteParameter in InheritedRouteParameters)
                {
                    var item = inheritedRouteParameter.Trim();
                    if (siteMapNode.RouteValues.ContainsKey(item))
                        throw new MvcSiteMapException(String.Format("SiteMapNodeSameKeyInRouteValueAndInheritedRouteParameter:{0}:{1}:{2}", key, Title ?? string.Empty, item));
                    if (parentNode.RouteValues.ContainsKey(item))
                        siteMapNode.RouteValues.Add(item, parentNode.RouteValues[item]);
                }
            }
            if (parentNode != null)
            {
                if (string.IsNullOrEmpty(Area) && !siteMapNode.RouteValues.ContainsKey("area"))
                {
                    siteMapNode.Area = parentNode.Area;
                }
                if (string.IsNullOrEmpty(Controller) && !siteMapNode.RouteValues.ContainsKey("controller"))
                {
                    siteMapNode.Controller = parentNode.Controller;
                }
            }
            if (!siteMapNode.RouteValues.ContainsKey("area"))
            {
                siteMapNode.RouteValues.Add("area", "");
            }

            return nodeParentMap;
        }

        private void AddAttributesToNode(ISiteMapNode node)
        {
            if(Area != null)
                node.Attributes.Add(new KeyValuePair<string, object>("area", Area));

            if (Controller != null)
                node.Attributes.Add(new KeyValuePair<string, object>("controller", Controller));

            if (Action != null)
                node.Attributes.Add(new KeyValuePair<string, object>("action", Action));

            if (HttpMethod != null)
                node.Attributes.Add(new KeyValuePair<string, object>("httpMethod", HttpMethod));

            if (Key != null)
                node.Attributes.Add(new KeyValuePair<string, object>("key", Key));

            if (Url != null)
                node.Attributes.Add(new KeyValuePair<string, object>("url", Url));

            if (Clickable.HasValue)
                node.Attributes.Add(new KeyValuePair<string, object>("clickable", Clickable.Value.ToString().ToLowerInvariant()));

            if (Roles != null)
                node.Attributes.Add(new KeyValuePair<string, object>("roles", string.Join(",", Roles)));

            if (ResourceKey != null)
                node.Attributes.Add(new KeyValuePair<string, object>("resourceKey", ResourceKey));

            if(VisibilityProvider != null)
                node.Attributes.Add(new KeyValuePair<string, object>("visibilityProvider", VisibilityProvider));

            if (DynamicNodeProvider != null)
                node.Attributes.Add(new KeyValuePair<string, object>("dynamicNodeProvider", DynamicNodeProvider));

            if (ImageUrl != null)
                node.Attributes.Add(new KeyValuePair<string, object>("imageUrl", ImageUrl));

            if (TargetFrame != null)
                node.Attributes.Add(new KeyValuePair<string, object>("targetFrame", TargetFrame));

            if (CacheResolvedUrl.HasValue)
                node.Attributes.Add(new KeyValuePair<string, object>("cacheResolvedUrl", CacheResolvedUrl.Value.ToString().ToLowerInvariant()));

            if (CanonicalUrl != null)
                node.Attributes.Add(new KeyValuePair<string, object>("canonicalUrl", CanonicalUrl));

            if (CanonicalKey != null)
                node.Attributes.Add(new KeyValuePair<string, object>("canonicalKey", CanonicalKey));

            if (MetaRobotsValues != null)
                node.Attributes.Add(new KeyValuePair<string, object>("metaRobotsValues", string.Join(" ", MetaRobotsValues)));

            if (ChangeFrequency != null)
                node.Attributes.Add(new KeyValuePair<string, object>("changeFrequency", ChangeFrequency));

            if (UpdatePriority.HasValue)
                node.Attributes.Add(new KeyValuePair<string, object>("updatePriority", UpdatePriority.Value.ToString()));

            if(LastModifiedDate.HasValue)
                node.Attributes.Add(new KeyValuePair<string, object>("lastModifiedDate", LastModifiedDate.Value.ToString()));

            if (Order.HasValue)
                node.Attributes.Add(new KeyValuePair<string, object>("order", Order.Value.ToString()));

            if (Route != null)
                node.Attributes.Add(new KeyValuePair<string, object>("route", Route));

            if (LastModifiedDate.HasValue)
                node.Attributes.Add(new KeyValuePair<string, object>("lastModifiedDate", LastModifiedDate.Value.ToString()));

            if(PreservedRouteParameters != null)
                node.Attributes.Add(new KeyValuePair<string, object>("preservedRouteParameters", string.Join(",", PreservedRouteParameters)));

            if(UrlResolver != null)
                node.Attributes.Add(new KeyValuePair<string, object>("urlResolver", UrlResolver));

            if (InheritedRouteParameters != null)
                node.Attributes.Add(new KeyValuePair<string, object>("inheritedRouteParameters", string.Join(",", InheritedRouteParameters)));
        }

        #endregion
    }
}
