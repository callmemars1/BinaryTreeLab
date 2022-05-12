namespace BinaryTreeLab;

// god save STL
public class Map<TKey, TValue>
    where TKey : IComparable<TKey>
{
    private Node? _head;
    private uint _count = 0;

    public Map() { }

    // Метод рекурсивно ищет узел с нужным ключем
    private Node? Find(Node node, TKey key)
    {
        switch (node.Key.CompareTo(key))
        {
            case 0: return node;
            case > 0:
                {
                    if (node.NextLower is null)
                        return null;

                    return Find(node.NextLower, key);
                }
            case < 0:
                {
                    if (node.NextHigher is null)
                        return null;

                    return Find(node.NextHigher, key);
                }
        }
    }

    // Метод рекурсивно ищет узел с ближайшим по значению ключем
    private Node FindClosest(Node node, TKey key)
    {
        switch (node.Key.CompareTo(key))
        {
            case 0: return node;
            case > 0:
                {
                    if (node.NextLower is null)
                        return node;

                    return FindClosest(node.NextLower, key);
                }
            case < 0:
                {
                    if (node.NextHigher is null)
                        return node;

                    return FindClosest(node.NextHigher, key);
                }
        }
    }

    // Метод проверяет, существует ли узел с ключем key
    public bool ContainsKey(TKey key)
    {
        if (_head is null)
            return false;

        return Find(_head, key) != null;
    }

    // Метод добавляет элемент в коллекцию, если такого не существует
    public bool TryAdd(TKey key, TValue value)
    {
        if (_head is null)
        {
            _head = new Node(key, value, null);
            _count++;
            return true;
        }

        var finded = FindClosest(_head, key);
        switch (finded.Key.CompareTo(key))
        {
            // Уже есть узел с таким ключем
            case 0: return false;
            case > 0:
                {
                    finded.NextLower = new Node(key, value, finded);
                    _count++;
                    return true;
                }
            case < 0:
                {
                    finded.NextHigher = new Node(key, value, finded);
                    _count++;
                    return true;
                }
        }
    }

    // Метод удаляет элемент с ключем key
    public bool TryRemove(TKey key, out (TKey key, TValue value)? keyValuePair)
    {
        keyValuePair = null;

        if (_head is null)
            return false;

        var finded = Find(_head, key);

        if (finded is null)
            return false;

        // Нет дочерних узлов (листьев)
        if (finded.NextLower is null && finded.NextHigher is null)
        {
            if (finded == _head)
                _head = null;


            else if (finded == finded!.Parent!.NextLower)
                finded!.Parent!.NextLower = null;

            else
                finded!.Parent!.NextHigher = null;

            _count--;
            keyValuePair = (finded!.Key!, finded!.Value!);
            return true;
        }

        // Есть узлы только с меньшим значением
        if (finded.NextHigher is null) 
        {
            if (finded == _head)
                _head = finded.NextLower;

            else if (finded == finded!.Parent!.NextLower)
                finded!.Parent!.NextLower = finded.NextLower;
            
            else
                finded!.Parent!.NextHigher = finded.NextLower;

            _count--;
            keyValuePair = (finded!.Key!, finded!.Value!);
            return true;
        }

        if (finded.NextLower is null) 
        {
            if (finded == _head)
                _head = finded.NextHigher;

            else if (finded == finded!.Parent!.NextLower)
                finded!.Parent!.NextLower = finded.NextHigher;

            else
                finded!.Parent!.NextHigher = finded.NextHigher;

            _count--;
            keyValuePair = (finded!.Key!, finded!.Value!);
            return true;
        }

        // Если есть оба под-дерева
        // ищем наименьший узел в правом под-дереве
        Node leastGreatestNode = finded;
        Node current = finded.NextHigher;
        while (current != null) 
        {
            leastGreatestNode = current;
            current = current!.NextLower!;
        }

        if (leastGreatestNode != finded.NextHigher) 
        {
            leastGreatestNode!.Parent!.NextLower = leastGreatestNode.NextHigher;
            leastGreatestNode.NextHigher = finded.NextHigher;
        }

        leastGreatestNode.NextLower = finded.NextLower;
        leastGreatestNode.NextLower.Parent = leastGreatestNode;

        if (finded == _head)
            _head = leastGreatestNode;

        else if (finded == finded!.Parent!.NextLower)
            finded!.Parent!.NextLower = leastGreatestNode;

        else
            finded!.Parent!.NextHigher = leastGreatestNode;

        keyValuePair = (finded!.Key!, finded!.Value!);
        _count--;
        return true;
    }

    public List<(TKey key, TValue value)> GetElements() 
    {
        var elements = new List<(TKey key, TValue value)>();

        if (_head == null)
            return elements;

        InOrderTraversal(_head, elements);

        return elements;
    }

    private void InOrderTraversal(Node node, List<(TKey key, TValue value)> elements) 
    {
        if (node.NextLower != null)
            InOrderTraversal(node.NextLower, elements);

        elements.Add((node.Key, node.Value)!);

        if (node.NextHigher != null)
            InOrderTraversal(node.NextHigher, elements);
    }

    public uint Count { get => _count; }

    public TValue this[TKey key] 
    {
        get 
        {
            if (_head is null)
                throw new ArgumentOutOfRangeException(nameof(key));

            var finded = Find(_head, key);
            if (finded is null)
                throw new ArgumentOutOfRangeException(nameof(key));

            return finded.Value!;
        }
        set 
        {
            if (_head is null)
            {
                TryAdd(key, value);
                return;
            }

            var finded = Find(_head, key);
            if (finded is not null)
                finded.Value = value;
            else
                TryAdd(key, value);
        }
    }

    class Node
    {
        private Node? _nextHigher;
        private Node? _nextLower;
        private Node? _parent;
        private readonly TKey _key;
        private TValue? _value;

        public Node(TKey key, TValue value, Node? parent)
        {
            _key = key;
            _value = value;
            _parent = parent;
        }

        public Map<TKey, TValue>.Node? NextLower { get => _nextLower; set => _nextLower = value; }
        public Map<TKey, TValue>.Node? NextHigher { get => _nextHigher; set => _nextHigher = value; }
        public Map<TKey, TValue>.Node? Parent { get => _parent; set => _parent = value; }

        public TKey Key => _key;

        public TValue? Value { get => _value; set => _value = value; }
    }
}