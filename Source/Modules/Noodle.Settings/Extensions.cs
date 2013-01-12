using Ninject;
using Ninject.Planning.Bindings.Resolvers;

namespace Noodle.Settings
{
    public static class Extensions
    {
        /// <summary>
        /// Allows a kernel to resolve ISettings dynamically
        /// </summary>
        /// <param name="kernel"></param>
        public static void MakeKernelResolveSettings(this IKernel kernel)
        {
            // This so the types implementing ISettings can be resolved dynamically.
            #region
            kernel.Components.RemoveAll<IMissingBindingResolver>();
            kernel.Components.Add<IMissingBindingResolver, SettingsBindingResolver>();
            #endregion
        }
    }
}
