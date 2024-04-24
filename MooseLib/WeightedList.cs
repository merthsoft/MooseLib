using System.Collections;

namespace Merthsoft.Moose.MooseEngine;
public class WeightedSet<T> : IEnumerable<T>
{
    protected List<T> set = [];

    public WeightedSet()
    {

    }

    public void Add(T item, int weight)
    {
        for (var i = 0; i < weight; i++)
            set.Add(item);
    }

    public void Clear()
        => set.Clear();

    public void Remove(T item)
        => set.RemoveAll(i => EqualityComparer<T>.Default.Equals(i, item));

    public IEnumerator<T> GetEnumerator() => set.Shuffle().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => set.Shuffle().GetEnumerator();
}
