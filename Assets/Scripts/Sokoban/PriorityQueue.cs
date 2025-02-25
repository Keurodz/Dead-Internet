using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<(T item, int priority)> elements = new List<(T, int)>();

    // Enqueue an item with its priority
    public void Enqueue(T item, int priority)
    {
        elements.Add((item, priority));
        elements.Sort((x, y) => x.priority.CompareTo(y.priority));
    }

    // Dequeue the item with the highest priority (lowest priority value)
    public T Dequeue()
    {
        if (elements.Count == 0)
            throw new InvalidOperationException("The priority queue is empty.");
        
        T item = elements[0].item;
        elements.RemoveAt(0);
        return item;
    }

    // Check if the queue is empty
    public bool IsEmpty()
    {
        return elements.Count == 0;
    }

    public bool Contains(T item)
    {
        return elements.Exists(x => x.item.Equals(item));
    }
}