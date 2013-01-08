using Noodle.Collections;

namespace Noodle
{
    public interface IBaseEntityWithDetails<T, TDetail> 
        where T:class,IBaseEntityWithDetails<T, TDetail>
        where TDetail: EntityDetail<T, TDetail>, new()
    {
        int Id { get; set; }
        NamedCollection<TDetail> Details { get; set; }
        //NamedCollection<EntityDetailCollection<T>> DetailCollections { get; set; }
        object this[string detailName] { get; set; }
    }
}
