using System;
using System.Collections;
using System.Collections.Generic;

public class MyList<T> : IList<T>
{
    private const int m_DefaultCapacity = 4;
    
    private int m_Count = 0;
    private int m_Version = 0;
    private T[] m_Array;

    public int Count => m_Count;
    public int Version => m_Version;
    public bool IsReadOnly => false;

    public MyList()
    {
        m_Array = Array.Empty<T>();
    }

    public MyList(int capacity)
    {
        m_Array = new T[capacity];
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        if (m_Count == m_Array.Length)
        {
            ResizeArray();
        }

        m_Array[m_Count] = item;
        m_Count++;
        m_Version++;
    }

    public void AddRange(T[] array, int index)
    {
        if (array == null)
        {
            throw new ArgumentNullException();
        }
        
        if (index < 0 || index > m_Count)
        {
            throw new ArgumentOutOfRangeException();
        }
        
        int newSize = m_Count + array.Length;

        if (newSize > m_Array.Length)
        {
            int newCapacity = m_Array.Length == 0 ? m_DefaultCapacity : m_Array.Length * 2;
            while (newCapacity < newSize)
            {
                newCapacity *= 2;
            }
            
            Array.Resize(ref m_Array, newCapacity);
        }
        
        Array.Copy(m_Array, index, m_Array, index + array.Length, m_Count - index);
        Array.Copy(array, 0, m_Array, index, array.Length);
        m_Count += array.Length;
        m_Version++;
    }

    public void Clear()
    {
        for (int i = 0; i < m_Array.Length; i++)
        {
            m_Array[i] = default;
        }

        m_Count = 0;
        m_Version++;
    }

    public bool Contains(T item)
    {
        return IndexOf(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException();
        }
        
        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException();
        }
        
        if (array.Length - arrayIndex < m_Count)
        {
            throw new ArgumentException();
        }

        Array.Copy(m_Array, 0, array, arrayIndex, m_Count);
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);

        if (index < 0)
        {
            return false;
        }
        
        RemoveAt(index);
        return true;
    }
    
    public int IndexOf(T item)
    {
        for (int i = 0; i < m_Count; i++)
        {
            if (m_Array[i].Equals(item))
            {
                return i;
            }
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > m_Count)
        {
            throw new ArgumentOutOfRangeException();
        }
        
        if (m_Count == m_Array.Length)
        {
            ResizeArray();
        }

        if (index == m_Count)
        {
            m_Array[m_Count] = item;
            m_Count++;
            return;
        }

        Array.Copy(m_Array, index, m_Array, index + 1, m_Count - index);
        m_Array[index] = item;
        m_Count++;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= m_Count)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (index == m_Count - 1)
        {
            m_Array[index] = default;
            m_Count--;
            m_Version++;
            return;
        }
        
        Array.Copy(m_Array, index + 1, m_Array, index, m_Count - index - 1);
        m_Array[m_Count - 1] = default;
        m_Count--;
        m_Version++;
    }

    public T Find(Predicate<T> match)
    {
        if (match == null)
        {
            throw new ArgumentNullException();
        }
        
        for (int i = 0; i < m_Count; i++)
        {
            if (match(m_Array[i]))
            {
                return m_Array[i];
            }
        }

        return default;
    }

    public T FindLast(Predicate<T> match)
    {
        if (match == null)
        {
            throw new ArgumentNullException();
        }
        
        for (int i = m_Count - 1; i >= 0; i--)
        {
            if (match(m_Array[i]))
            {
                return m_Array[i];
            }
        }

        return default;
    }

    public MyList<T> FindAll(Predicate<T> match)
    {
        if (match == null)
        {
            throw new ArgumentNullException();
        }

        MyList<T> result = new MyList<T>();

        for (int i = 0; i < m_Count; i++)
        {
            if (match(m_Array[i]))
            {
                result.Add(m_Array[i]);
            }
        }

        return result;
    }

    public void Sort()
    {
        Sort(Comparer<T>.Default);
    }

    public void Sort(IComparer<T> comparer)
    {
        if (comparer == null)
        {
            throw new ArgumentNullException();
        }

        for (int i = 0; i < m_Count - 1; i++)
        {
            for (int j = 0; j < m_Count - 1 - i; j++)
            {
                if (comparer.Compare(m_Array[j], m_Array[j + 1]) > 0)
                {
                    (m_Array[j], m_Array[j + 1]) = (m_Array[j + 1], m_Array[j]);
                }
            }
        }

        m_Version++;
    }

    public void TrimExcess()
    {
        Array.Resize(ref m_Array, m_Count);
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= m_Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return m_Array[index];
        }
        set
        {
            if (index < 0 || index >= m_Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            m_Array[index] = value;
            m_Version++;
        }
    }

    private int GetNewCapacity()
    {
        return m_Array.Length == 0
            ? m_DefaultCapacity
            : m_Array.Length * 2;
    }

    private void ResizeArray()
    {
        Array.Resize(ref m_Array, GetNewCapacity());
    }
    
    public struct Enumerator: IEnumerator<T>
    {
        private readonly MyList<T> m_List;
        private int m_Index;
        private int m_Version;
        private T m_Current;
        
        public T Current => m_Current;
        object IEnumerator.Current => Current;

        internal Enumerator(MyList<T> list)
        {
            m_List = list;
            m_Index = 0;
            m_Current = default;
            m_Version = list.Version;
        }
        
        public bool MoveNext()
        {
            if (m_Version != m_List.Version)
            {
                throw new InvalidOperationException();
            }
            
            if (m_Index >= m_List.Count)
            {
                return false;
            }

            m_Current = m_List[m_Index];
            m_Index++;
            return true;
        }

        public void Reset()
        {
            m_Index = 0;
            m_Current = default;
        }

        public void Dispose()
        {
            //nothing
        }
    }
}
