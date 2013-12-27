using Noodle.Engine;

namespace Noodle.Imaging
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(TinyIoCContainer container)
        {
            container.Register<IImageManipulator, NoodleImageManipulator>();
            container.Register<IImageLayoutBuilder, ImageLayoutBuilder>();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
