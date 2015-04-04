using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Noodle.Extensions.Reflection
{

    public static class Private
    {
        public static Expression<Func<T, TValue>> Property<T, TValue>(string propertyName)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (property == null)
            {
                throw new Exception(string.Format("Could not find a property named '{0}' on type '{1}'", propertyName, type.Name));
            }

            var parameterExpression = Expression.Parameter(type, "x");

            var expression = Expression.Lambda(
                typeof(Func<T, TValue>),
                Expression.Property(
                    parameterExpression,
                    property
                ),
                parameterExpression
            );

            return (Expression<Func<T, TValue>>)expression;
        }

        public static Expression<Func<T, TValue>> Field<T, TValue>(string fieldName)
        {
            var type = typeof(T);
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                throw new Exception(string.Format("Could not find a field named '{0}' on type '{1}'", fieldName, type.Name));
            }

            var parameterExpression = Expression.Parameter(type, "x");

            var expression = Expression.Lambda(
                typeof(Func<T, TValue>),
                Expression.Field(
                    parameterExpression,
                    field
                ),
                parameterExpression
            );

            return (Expression<Func<T, TValue>>)expression;
        }

        public static MethodInfo Method<T>(string methodName)
        {
            return typeof (T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}