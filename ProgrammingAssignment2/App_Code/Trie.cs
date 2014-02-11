using System;
using System.Collections.Generic;

// Hudson Poissant
// This code was created with assistance from Googled online resources.

/// <summary>
/// This class represents a trie data structure which is holds part of 
/// a letter of a word in a node for fast retrieval. 
/// </summary>
public class Trie
{
    private Node _root;

    /// <summary>
    /// Constructs a new empty trie.
    /// </summary>
    public Trie()
    {
        _root = new Node(' ');
    }

    /// <summary>
    /// Inserts a new word into the trie.
    /// </summary>
    /// <param name="s">Word to be added.</param>
    public void Insert(string s)
    {
        char[] wordDiv = s.ToLower().ToCharArray();
        Node current = _root;
        if (wordDiv.Length == 0)
        {
            current.Last = true;
        }

        for (int i = 0; i < s.Length; i++)
        {
            Node child = current.ChildNode(wordDiv[i]);
            if (child != null)
            {
                current = child;
            }
            else
            {
                current.Children.Add(wordDiv[i], new Node(wordDiv[i]));
                current = current.ChildNode(wordDiv[i]);
            }

            if (i == wordDiv.Length - 1)
            {
                current.Last = true;
            }
        } 
    }

    /// <summary>
    /// Verifies whether or not a word is already present in the trie.
    /// </summary>
    /// <param name="s">Word to check.</param>
    /// <returns>True or False depending on presence of word.</returns>
    public bool Search(string s)
    {
        char[] wordDiv = s.ToLower().ToCharArray();
        Node current = _root;
        while (current != null)
        {
            Console.WriteLine(current.Letter);
            for (int i = 0; i < wordDiv.Length; i++)
            {
                if (current.ChildNode(wordDiv[i]) == null)
                    return false;
                else
                    current = current.ChildNode(wordDiv[i]);
            }

            if (current.Last == true)
            {
                return true;
            }
            else
                return false;
        }
        return false;
    }

    /// <summary>
    /// Traverses trie and returns possible words.
    /// </summary>
    /// <returns>List of strings where each string is a word.</returns>
    public List<string> possibilities()
    {
        return possibilities(_root,"", new List<string>());
    } 

    /// <summary>
    /// Helper method that can retrieve words based off of a starting point in the trie.
    /// </summary>
    /// <param name="root">Node to start at.</param>
    /// <param name="word">Word to be concatenated</param>
    /// <param name="words">List of words to be suggested.</param>
    /// <returns></returns>
    private List<String> possibilities(Node root, string word, List<string> words)
    {
        if (words.Count == 10)
        {
            return words;
        } 
        if (root.Last == true)
        {
            words.Add(word);
        }
        foreach (var pair in root.Children)
        {
            word = word + pair.Value.Letter.ToString();
            possibilities(pair.Value, word,words);
            word = word.Substring(0, word.Length - 1);
        }
        return words;
    }


    /// <summary>
    /// Finds words that start with a certain letter or part of a word.
    /// </summary>
    /// <param name="str">Part of a word or entire word.</param>
    /// <returns>List of 10 strings to suggest.</returns>
    public List<string> WordsThatStartWith(string str)
    {
        return WordsThatStartWith(_root, str, "", new List<string>());
    }

    /// <summary>
    /// Helper method to find words that start with a certain letter or many letters.
    /// </summary>
    /// <param name="root">Starting node in trie</param>
    /// <param name="str">String to base search off of.</param>
    /// <param name="word">Word to be built.</param>
    /// <param name="words">List of words to suggest.</param>
    /// <returns></returns>
    private List<String> WordsThatStartWith(Node root, string str, string word, List<string> words)
    {
        char[] wordDiv = str.ToLower().ToCharArray();
        foreach (var pair in root.Children)
        {
            if (pair.Value.Letter == wordDiv[0])
            {
                word = word + pair.Value.Letter.ToString();
                if (wordDiv.Length == 1)
                {
                    words = possibilities(pair.Value, word, words);
                }
                else
                {
                    words = WordsThatStartWith(pair.Value,str.Substring(1,str.Length - 1), word, words);
                }
            } 
        }
        return words;
    }

    public Node getRoot()
    {
        return _root;
    }
}