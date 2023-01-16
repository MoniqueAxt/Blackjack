namespace Utilities;

internal interface IListManager<T>
{
    // member Properties
    // (fields cannot be used in Interfaces)

    /** The number of elements currently in the _list */
    public int Count { get; }

    // member methods

    /** Add an item to a _list */
    bool Add(T type);
    /** Change/update the element at the given index with the given element */
    bool ChangeAt(T type, int index);
    /** A check to determine whether an index is valid */
    bool ValidIndex(int index);
    /** Remove all elements from a _list */
    void DeleteAll();
    /** Delete an element in the _list at the given index */
    bool DeleteAt(int index);
    /** Delete a specific element from the _list */
    bool Delete(T type);
    /** Get the element in the _list at the given index */
    T GetAt(int index);
    /** Get an array of strings representing each element in the _list */
    string[] ToStringArray();
    /** Get a _list of strings representing each element in the _list */
    List<string> ToStringList();
}