using Noodle.Engine;

namespace Noodle.Imaging.AForge
{
    /// <summary>
    /// AForge imaging dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register your services with the container. You are given a type finder to help you find anything you need.
        /// </summary>
        /// <param name="container"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Register(TinyIoCContainer container)
        {
            container.Register<IImageManipulator, AForgeImageManipulator>();
        }

        /// <summary>
        /// The lower numbers will be registered first. Higher numbers the latest.
        /// If you are registering fakes, give a high integer (int.Max ?) so that that it will be registered last,
        /// and the container will use it instead of the previously registered services.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public int Importance
        {
            get { return int.MaxValue; }
        }
    }
}
