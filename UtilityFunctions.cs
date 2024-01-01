using OtterGui;

namespace Penumbra.GameData;

public static class UtilityFunctions
{
    /// <summary> Try to find the index of the first object fulfilling the predicate in an enumerable. </summary>
    /// <param name="enumerable"> The enumerable. </param>
    /// <param name="predicate"> The predicate to compare on. </param>
    /// <param name="index"> The index of the first object, if any. </param>
    /// <returns> True if one was found, false otherwise. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool FindIndex<T>(IEnumerable<T> enumerable, Predicate<T> predicate, out int index)
    {
        index = enumerable.IndexOf(predicate);
        return index != -1;
    }

    /// <summary> Return the first object fulfilling the predicate or null for structs. </summary>
    /// <param name="values"> The enumerable. </param>
    /// <param name="predicate"> The predicate. </param>
    /// <returns> The first object fulfilling the predicate, or a null-optional. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T? FirstOrNull<T>(this IEnumerable<T> values, Func<T, bool> predicate) where T : struct
        => values.Cast<T?>().FirstOrDefault(v => predicate(v!.Value));


    /// <summary> Add an item <paramref name="count"/> times to the end of a given array. </summary>
    /// <param name="array"> The existing array. </param>
    /// <param name="element"> The element to be added. </param>
    /// <param name="count"> The number of times to add the item. </param>
    /// <returns> The new array. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T[] AddItem<T>(this T[] array, T element, int count = 1)
    {
        var length   = array.Length;
        var newArray = new T[array.Length + count];
        Array.Copy(array, newArray, length);
        for (var i = length; i < newArray.Length; ++i)
            newArray[i] = element;

        return newArray;
    }

    /// <summary> Remove <paramref name="count"/> items starting from <paramref name="offset"/> from an array. </summary>
    /// <param name="array"> The existing array. </param>
    /// <param name="offset"> The index of the first item to remove. </param>
    /// <param name="count"> The number of items to remove. </param>
    /// <returns> The new array. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T[] RemoveItems<T>(this T[] array, int offset, int count = 1)
    {
        var newArray = new T[array.Length - count];
        Array.Copy(array, newArray,       offset);
        Array.Copy(array, offset + count, newArray, offset, newArray.Length - offset);
        return newArray;
    }
}
