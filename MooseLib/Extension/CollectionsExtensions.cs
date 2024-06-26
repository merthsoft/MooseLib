﻿namespace Merthsoft.Moose.MooseEngine.Extension;
public static class CollectionsExtensions
{
    public static void Resize<T>(this List<T?> list, int size, T? @default = default)
    {
        var count = list.Count;

        if (size < count)
        {
            list.RemoveRange(size, count - size);
        }
        else if (size > count)
        {
            if (size > list.Capacity)
                list.Capacity = size;

            list.AddRange(Enumerable.Repeat(@default, size - count));
        }
    }

    public static T MoveNextGetCurrent<T>(this IEnumerator<T> set)
    {
        set.MoveNext();
        return set.Current;
    }

    public static T RandomElement<T>(this IList<T> items, Random random)
           => items[random.Next(items.Count)];

    public static T RandomElement<T>(this IList<T> items)
           => RandomElement(items, MooseGame.Random);

    public static T RemoveRandomElement<T>(this IList<T> items)
    {
        var index = MooseGame.Random.Next(items.Count);
        var item = items[index];
        items.RemoveAt(index);
        return item;
    }

    public static void MoveToTop<T>(this List<T> list, int index)
    {
        var item = list[index];
        for (var i = index; i > 0; i--)
            list[i] = list[i - 1];
        list[0] = item;
    }

    public static void MoveToBottom<T>(this List<T> list, int index)
    {
        var item = list[index];
        for (var i = index; i < list.Count - 1; i++)
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
            index++;
        }
        return -1;
    }

    public static void ForEach<T>(this IEnumerable<T> set, Action<T> action)
    {
        foreach (var t in set)
            action(t);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) 
        => source.Shuffle(MooseGame.Random);

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng) 
        => source.ShuffleIterator(rng);

    private static IEnumerable<T> ShuffleIterator<T>(
        this IEnumerable<T> source, Random rng)
    {
        var buffer = source.ToList();
        for (var i = 0; i < buffer.Count; i++)
        {
            var j = rng.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }
}
