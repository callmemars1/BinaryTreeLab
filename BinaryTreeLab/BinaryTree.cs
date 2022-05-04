namespace BinaryTreeLab;

// TODO: Добавить возможность удаление элемента

public record Pair<TFirst, TSecond>(TFirst First, TSecond Second);
public class BinaryTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    private Node? _head;
    private uint _count = 0;
    public BinaryTree()
    {

    }

    public bool TryAdd(TKey key, TValue value) 
    {
        if (_head is null)
        {
            _head = new Node(key, value, null);
            return true;
        }

        var findedNode = FindClosestNode(key);
        if (findedNode == null)
            return false;

        switch (findedNode!.Key!.CompareTo(key))
        {
            // узел с таким ключем уже существует
            case 0: return false;
            case > 0: 
                findedNode.NextLower = new Node(key, value, findedNode);
                _count++;
                return true;
            case < 0:
                findedNode.NextHigher = new Node(key, value, findedNode);
                _count++;
                return true;
        }
    }

    private Node? FindClosestNode(TKey key)
    {
        if (_head == null)
            return null;

        var currentNode = _head;
        for (int i = 0; i < _count; i++)
        {
            switch (currentNode!.Key!.CompareTo(key))
            {
                // найден узел с таким ключем
                case 0: return currentNode;
                case > 0:
                    if (currentNode.NextLower is not null)
                    {
                        currentNode = currentNode.NextLower;
                        break;
                    }

                    return currentNode;
                case < 0:
                    if (currentNode.NextHigher is not null)
                    {
                        currentNode = currentNode.NextHigher;
                        break;
                    }

                    return currentNode;
            }
        }
        return null;
    }

    public bool ContainsKey(TKey key, out Pair<TKey, TValue>? pair)
    {
        pair = null;
        if (_head == null)
            return false;

        var currentNode = _head;
        for (int i = 0; i < _count; i++)
        {
            switch (currentNode!.Key!.CompareTo(key))
            {
                case 0:
                    pair = currentNode;
                    return true;
                case > 0:
                    if (currentNode.NextLower is not null)
                    {
                        currentNode = currentNode.NextLower;
                        break;
                    }

                    return false;
                case < 0:
                    if (currentNode.NextHigher is not null)
                    {
                        currentNode = currentNode.NextHigher;
                        break;
                    }

                    return false;
            }
        }
        return false;
    }

    public bool ContainsKey(TKey key) => ContainsKey(key, out _);

    private bool TryGetNode(TKey key, out Node? node) 
    {
        node = null;
        if (_head == null)
            return false;

        var currentNode = _head;
        for (int i = 0; i < _count; i++)
        {
            switch (currentNode!.Key!.CompareTo(key))
            {
                case 0:
                    node = currentNode;
                    return true;
                case > 0:
                    if (currentNode.NextLower is not null)
                    {
                        currentNode = currentNode.NextLower;
                        break;
                    }

                    return false;
                case < 0:
                    if (currentNode.NextHigher is not null)
                    {
                        currentNode = currentNode.NextHigher;
                        break;
                    }

                    return false;
            }
        }
        return false;
    }

    public TValue this[TKey key] 
    {
        get 
        {
            ContainsKey(key, out var pair);
            return pair!.Second ?? throw new ArgumentOutOfRangeException($"Item with {key} key not exist!");
        }
        set 
        {
            TryGetNode(key, out var node);
            if (node is null)
                TryAdd(key, value);
            else
                node.Value = value;
        }
    }

    public class Node
    {
        private readonly TKey? _key;
        private TValue? _value;
        private Node? _parent;
        private Node? _nextHigher;
        private Node? _nextLower;

        public Node(TKey key, TValue value, Node? parent)
        {
            _key = key;
            _value = value;
            _parent = parent;
        }

        public TKey? Key { get => _key; }
        public TValue? Value { get => _value; set => _value = value; }
        public BinaryTree<TKey, TValue>.Node? Parent { get => _parent; set => _parent = value; }
        public BinaryTree<TKey, TValue>.Node? NextHigher { get => _nextHigher; set => _nextHigher = value; }
        public BinaryTree<TKey, TValue>.Node? NextLower { get => _nextLower; set => _nextLower = value; }

        public static implicit operator Pair<TKey, TValue>(Node node)
        {
            return new Pair<TKey, TValue>(First: node.Key, Second: node.Value);
        }
    }
}
