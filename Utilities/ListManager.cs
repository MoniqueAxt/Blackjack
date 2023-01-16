namespace Utilities;

/** A class that manages the operations on a generic List.
 * */
public class ListManager<T> : IListManager<T>
{
    //==============================================
    // member variables, Properties, constructor/s
    //==============================================        
    public int Count => List.Count;            /* the number of elements in the _list */
    public List<T> List { get; set; }
    public T LastDeletedItem { get; internal set; } = default!; /* keep track of the last item deleted from the _list */

    // constructor/s
    public ListManager()
    {
        List = new List<T>();
    }

    public ListManager(List<T> list)
    {
        List = list;
    }

    //==============================================
    // List management
    //==============================================
    /** Adds an element to the _list if the element is not null. Returns false if item is null. */
    public bool Add(T item)
    {
        if (item == null) return false;
        List.Add(item);
        return true;
    }
    /** Change an element at a certain index in the _list. The element must be non-null and exist in the _list.
         * The given index must be valid (within bounds). Returns true if element was successfully changed, else false. */
    public bool ChangeAt(T item, int index)
    {
        if (!ValidIndex(index) || item == null) return false;
        // check if the item exists in the _list
        if (!ItemExists(item)) return false;
        List[index] = item;
        return true;
    }
    /** Returns whether a given index is valid in the _list or not. */
    public bool ValidIndex(int index)
    {
        return index >= 0 && index < List.Count; ;
    }

    /** Removes an element at a given index in the _list. The index must be valid.
         * The element to be deleted is saved in a member field as the last deleted item.
         * Returns true if the element was successfully removed.
         */
    public bool DeleteAt(int index)
    {
        if (!ValidIndex(index)) return false;
        LastDeletedItem = List[index];
        List.RemoveAt(index);
        return true;
    }

    /** Removes a specific element from the _list.
         * The element to be deleted is saved in a member field as the last deleted item.
         * Returns true if the element was successfully removed, false if it could not be
         * deleted or was not found in the _list.
         */
    public bool Delete(T item)
    {
        if (ListisEmpty() || item == null) return false;
        LastDeletedItem = item;
        return List.Remove(item);
    }

    public T Pop()
    {
        if (ListisEmpty()) return default!;
        T t = GetAt(0);
        List.RemoveAt(0);
        return t;
    }

    /** Removes all elements from the _list. */
    public void DeleteAll()
    {
        if (!ListisEmpty())
        {
            List.Clear();
        }
    }
    /** Returns an element at a certain index if successful. The given index must be valid.
         * Returns the default of an element-type if the element could not be found.
         */
    public T GetAt(int index)
    {
        return ValidIndex(index) ? List.ElementAt(index) : default!;
    }
    /** Returns the index of an element in the _list if it exists, otherwise returns -1 */
    public int GetIndex(T item)
    {
        return List.IndexOf(item);
    }
    /** Returns whether the _list is empty or not */
    public bool ListisEmpty()
    {
        return List.Count == 0;
    }
    /** Returns a string array representing each element in the _list (calls .ToString() on elements) */
    public string[] ToStringArray()
    {
        string[] array = (from item in List where item != null select item.ToString()).ToArray()!;
        return array;
    }
    /** Returns a _list of strings representing each element in the _list (calls .ToString() on elements) */
    public List<string> ToStringList()
    {
        return ToStringArray().ToList();
    }

    /** Returns whether a specific item exists in the _list or not */
    public bool ItemExists(T item)
    {
        return List.IndexOf(item) >= 0;
    }

    public void Shuffle()
    {
        List.Shuffle();
    }
}