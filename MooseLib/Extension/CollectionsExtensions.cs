namespace Merthsoft.Moose.MooseEngine.Extension;
public static class CollectionsExtensions
{
    public static T RandomElement<T>(this IList<T> items)
           => items[MooseGame.Instance.Random.Next(items.Count)];

    public static T RemoveRandomElement<T>(this IList<T> items)
    {
        var index = MooseGame.Instance.Random.Next(items.Count);
        var item = items[index];
        items.RemoveAt(index);
        return item;
    }

    public static void MoveToTop<T>(this List<T> list, int index)
    {
        T item = list[index];
        for (int i = index; i > 0; i--)
            list[i] = list[i - 1];
        list[0] = item;
    }

    public static void MoveToBottom<T>(this List<T> list, int index)
    {
        T item = list[index];
        for (int i = index; i < list.Count - 1; i++)
            list[i] = list[i + 1];
        list[^1] = item;
    }

    public static void Swap<T>(this IList<T> list, int index1, int index2)
    {
        var track1 = list[index1];
        var track2 = list[index2];
        list[index1] = track2;
        list[index2] = track1;
    }

    public static T AddItem<T>(this IList<T> list, T item)
    {
        list.Add(item);
        return item;
    }
    
    public static int IndexOf<T>(this IEnumerable<T> set, Func<T, bool> func)
    {
        var index = 0;
        foreach (var item in set)
        {
            if (func(item))
                return index;
        }
        return -1;
    }

    public static void ForEach<T>(this IEnumerable<T> set, Action<T> action)
    {
        foreach (var t in set)
            action(t);
    }
}
