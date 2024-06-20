public class TrieNode
{
    public Dictionary<char, TrieNode> Children { get; set; }
    public bool IsEndOfWord { get; set; }

    public char _value;

    public TrieNode(char value = ' ')
    {
        Children = new Dictionary<char, TrieNode>();
        IsEndOfWord = false;
        _value = value;
    }

    public bool HasChild(char c)
    {
        return Children.ContainsKey(c);
    }
}

public class Trie
{
    private TrieNode root;

    public Trie()
    {
        root = new TrieNode();
    }

    // Search for a word in the trie
    public bool Search(string word){
        TrieNode current = root;

        // For each character in the word, check if the current node has a child with that character.
        // If not, return false.
        foreach (char c in word)
        {
            if (!current.HasChild(c))
            {
                return false;
            }
            current = current.Children[c];
        }

        // Return true if the current node marks the end of the word.
        return current.IsEndOfWord;
    }

    public bool Insert(string word)
    {
        TrieNode current = root;

        // For each character in the word, check if the current node has a child with that character.
        // If not, create a new child node with that character.
        foreach (char c in word)
        {
            //If the current node does not have a child with the character, create a new child node.
            if (!current.HasChild(c))
            {
                // Create a new child node with the character.
                current.Children[c] = new TrieNode(c);
            }
            current = current.Children[c];
        }
        if (current.IsEndOfWord)
        {
            // The word is already in the trie.
            return false;
        }

        //Mark the end of the word.
        current.IsEndOfWord = true;
        return true;
    }
    
    /// <summary>
    /// Retrieves a list of suggested words based on the given prefix.
    /// </summary>
    /// <param name="prefix">The prefix to search for.</param>
    /// <returns>A list of suggested words.</returns>
    public List<string> AutoSuggest(string prefix)
    {
        TrieNode currentNode = root;
        foreach (char c in prefix)
        {
            if (!currentNode.HasChild(c))
            {
                return new List<string>();
            }
            currentNode = currentNode.Children[c];
        }
        return GetAllWordsWithPrefix(currentNode, prefix);
    }

    private List<string> GetAllWordsWithPrefix(TrieNode root, string prefix)
    {
        List<string> words = new List<string>();

        // If the current node marks the end of a word, add it to the list of words.
        if (root.IsEndOfWord)
        {
            words.Add(prefix);
        }

        // Recursively traverse all the child nodes and append their characters to the prefix.
        foreach (var child in root.Children)
        {
            string childPrefix = prefix + child.Key;
            words.AddRange(GetAllWordsWithPrefix(child.Value, childPrefix));
        }

        return words;
    }

    // Helper method to delete a word from the trie by recursively removing its nodes
    private bool _deleteWord(TrieNode root, string word, int index)
    {
        // If we have reached the end of the word, check if the current node marks the end of a word.
        if (index == word.Length)
        {
            if (!root.IsEndOfWord)
            {
                // The word does not exist in the trie.
                return false;
            }

            // Mark the current node as not the end of a word.
            root.IsEndOfWord = false;

            // If the current node has no children, it can be safely removed.
            return root.Children.Count == 0;
        }

        char c = word[index];
        if (!root.HasChild(c))
        {
            // The word does not exist in the trie.
            return false;
        }

        TrieNode child = root.Children[c];
        bool shouldDeleteNode = _deleteWord(child, word, index + 1);

        if (shouldDeleteNode)
        {
            // Remove the child node from the current node's children.
            root.Children.Remove(c);

            // If the current node is not the end of a word and has no children, it can be safely removed.
            return !root.IsEndOfWord && root.Children.Count == 0;
        }

        return false;
    }

    public bool Delete(string word)
    {
        return _deleteWord(root, word, 0);
   }

    public List<string> GetAllWords()
    {
        return GetAllWordsWithPrefix(root, "");
    }

    public void PrintTrieStructure()
    {
        Console.WriteLine("\nroot");
        _printTrieNodes(root);
    }

    private void _printTrieNodes(TrieNode root, string format = " ", bool isLastChild = true) 
    {
        if (root == null)
            return;

        Console.Write($"{format}");

        if (isLastChild)
        {
            Console.Write("└─");
            format += "  ";
        }
        else
        {
            Console.Write("├─");
            format += "│ ";
        }

        Console.WriteLine($"{root._value}");

        int childCount = root.Children.Count;
        int i = 0;
        var children = root.Children.OrderBy(x => x.Key);

        foreach(var child in children)
        {
            i++;
            bool isLast = i == childCount;
            _printTrieNodes(child.Value, format, isLast);
        }
    }

    public List<string> GetSpellingSuggestions(string word)
    {
        char firstLetter = word[0];
        List<string> suggestions = new();
        List<string> words = GetAllWordsWithPrefix(root.Children[firstLetter], firstLetter.ToString());
        
        foreach (string w in words)
        {
            int distance = LevenshteinDistance(word, w);
            if (distance <= 2)
            {
                suggestions.Add(w);
            }
        }

        return suggestions;
    }

    private int LevenshteinDistance(string s, string t)
    {
        int m = s.Length;
        int n = t.Length;
        int[,] d = new int[m + 1, n + 1];

        if (m == 0)
        {
            return n;
        }

        if (n == 0)
        {
            return m;
        }

        for (int i = 0; i <= m; i++)
        {
            d[i, 0] = i;
        }

        for (int j = 0; j <= n; j++)
        {
            d[0, j] = j;
        }

        for (int j = 1; j <= n; j++)
        {
            for (int i = 1; i <= m; i++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[m, n];
    }
}