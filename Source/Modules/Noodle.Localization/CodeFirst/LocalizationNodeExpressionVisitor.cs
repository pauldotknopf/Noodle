using System;
using System.Collections.Generic;
using System.Linq;
using Noodle.Linq;

namespace Noodle.Localization.CodeFirst
{
    public class LocalizationNodeExpressionVisitor : ExpressionVisitor
    {
        private static IDictionary<Type, string> _typeNamespaces = new Dictionary<Type, string>();
        private static object _typeNamespacesLockObject = new object();

        public System.Linq.Expressions.Expression<Func<string>> Expression { get; set; }

        public string ResourceName { get; protected set; }

        public string DefaultValue { get; protected set; }

        public LocalizationNodeExpressionVisitor(System.Linq.Expressions.Expression<Func<string>> expression)
        {
            Expression = expression;
            Visit(expression);
        }

        protected override System.Linq.Expressions.Expression VisitMemberAccess(System.Linq.Expressions.MemberExpression m)
        {
            var field = m.Member.Name;
            var fieldNamespace = GetNamespace(m.Member.DeclaringType);

            ResourceName = field == "Value" ? fieldNamespace : fieldNamespace + "." + field;
            DefaultValue = Expression.Compile()();

            return base.VisitMemberAccess(m);
        }

        public string GetNamespace(Type type)
        {
            lock(_typeNamespacesLockObject)
            {
                if (!_typeNamespaces.ContainsKey(type))
                {
                    _typeNamespaces[type] = GetNamespaceRecursively(type);
                }
                return _typeNamespaces[type];
            }
        }

        public string GetNamespaceRecursively(Type type)
        {
            var resourceNamespaceAttribute = type.GetCustomAttributes(true).OfType<ResourceNamespaceAttribute>().SingleOrDefault();
            var typeNamespace = resourceNamespaceAttribute == null 
                ? type.Name : 
                resourceNamespaceAttribute.ResourceNamespace;

            var parentClass = type.ReflectedType;

            if (parentClass != null)
            {
                return GetNamespaceRecursively(parentClass) + "." + typeNamespace;
            }
            else
            {
                return typeNamespace;
            }
        }
    }
}
