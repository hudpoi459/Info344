using System.Collections.Generic;

/// <summary>
/// This class represents a node in a trie.
/// </summary>

public class Node
{
    char _letter;
    bool _last;
    Dictionary<char, Node> _children;

    protected Node() { }

    /// <summary>
    /// Constructs a new Node based off of provided character.
    /// </summary>
    /// <param name="c">Character to be the letter of the node.</param>
    public Node(char c)
    {
        _children = new Dictionary<char, Node>();
        _last = false;
        _letter = c;
    }

    /// <summary>
    /// Represents the letter of the node.
    /// </summary>
    public char Letter
    {
        get { return this._letter; }
        set { this._letter = value; }
    }

    /// <summary>
    /// Determines whether or not a node is the end of a word.
    /// </summary>
    public bool Last
    {
        get { return this._last; }
        set { this._last = value; }
    }

    /// <summary>
    /// Contains children nodes of possible pathways to other letters that make a word.
    /// </summary>
    public Dictionary<char, Node> Children
    {
        get { return this._children; }
        set { this._children = value; }
    }

    /// <summary>
    /// Returns the child node of the node with the the given character.
    /// </summary>
    /// <param name="c">Provided character</param>
    /// <returns>Returns a node with the specified character</returns>
    public Node ChildNode(char c)
    {
        if (Children != null)
        {
            if (Children.ContainsKey(c))
            {
                return Children[c];
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if two objects are equal.
    /// </summary>
    /// <param name="obj">Object to be compared to.</param>
    /// <returns>Returns true or false.</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
            return false;

        return Equals(obj);
    }

    /// <summary>
    /// Checks if two nodes are equivalent.
    /// </summary>
    /// <param name="obj">Node to be compared to.</param>
    /// <returns>Returns true or false.</returns>
    public bool Equals(Node obj)
    {
        if (obj != null
            && obj.Letter == this.Letter)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Method to get hashcode of an object.
    /// </summary>
    /// <returns>Hashcode in the form of an integer.</returns>
    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + this.Letter.GetHashCode();
        return hash;
    }
}