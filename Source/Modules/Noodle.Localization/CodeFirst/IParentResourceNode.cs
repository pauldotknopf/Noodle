using System.Collections.Generic;

namespace Noodle.Localization.CodeFirst
{
    public interface IParentResourceNode : IResourceNode
    {
        List<IResourceNode> Children();
    }
}
