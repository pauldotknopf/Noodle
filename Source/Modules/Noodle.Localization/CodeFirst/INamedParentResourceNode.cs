namespace Noodle.Localization.CodeFirst
{
    public interface INamedParentResourceNode : IParentResourceNode
    {
        string Name { get; set; }
    }
}
