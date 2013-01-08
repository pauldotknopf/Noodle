using System.Reflection;

namespace Noodle
{
    public static class ReflectionUtility
    {
        public static object CallMethod(object instance, string methodName, params object[] parameters)
        {
            MethodInfo dynMethod = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            return dynMethod.Invoke(instance, parameters);
        }

        public static TReturnType CallMethod<TReturnType>(object instance, string methodName, params object[] parameters)
        {
            return (TReturnType)CallMethod(instance, methodName, parameters);
        }
    }
}
