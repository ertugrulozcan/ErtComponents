using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigen.Core.Utility
{
    public class Node
    {
        #region Fields

        private char nodeKey;
        private HashSet<Node> children;
        private readonly HashSet<object> objects;

        #endregion

        #region Properties

        public char NodeKey
        {
            get
            {
                return nodeKey;
            }

            private set
            {
                nodeKey = value;
            }
        }

        public HashSet<Node> Children
        {
            get
            {
                return children;
            }

            private set
            {
                children = value;
            }
        }

        public HashSet<object> Objects
        {
            get
            {
                return objects;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Node(char key, HashSet<KeyValuePair<string, object>> source)
        {
            this.NodeKey = key;
            this.Children = new HashSet<Node>();
            this.objects = new HashSet<object>();
            foreach (var data in source)
            {
                if (!this.Objects.Contains(data.Value))
                    this.Objects.Add(data.Value);
            }

            this.SetChildSources(source);
        }

        #endregion

        #region Methods

        private void SetChildSources(HashSet<KeyValuePair<string, object>> source)
        {
            foreach (var vp in source)
            {
                if (string.IsNullOrEmpty(vp.Key) || string.IsNullOrWhiteSpace(vp.Key))
                {
                    continue;
                }
                else
                {
                    char key = vp.Key[0];
                    if (!this.Children.Any(x => x.NodeKey == key))
                    {
                        // Düğüm oluştur.
                        this.Children.Add(new Node(key, this.CreateSubNodeSource(source, key)));
                    }
                    else
                    {
                        // Düğüm zaten oluşturulmuş.
                    }
                }
            }
        }

        private HashSet<KeyValuePair<string, object>> CreateSubNodeSource(HashSet<KeyValuePair<string, object>> mainSource, char key)
        {
            HashSet<KeyValuePair<string, object>> subNodeSource = new HashSet<KeyValuePair<string, object>>();

            foreach (var vp in mainSource)
            {
                if (!string.IsNullOrEmpty(vp.Key))
                {
                    if (vp.Key[0] == key)
                        subNodeSource.Add(new KeyValuePair<string, object>(vp.Key.Substring(1, vp.Key.Length - 1), vp.Value));
                }
            }

            return subNodeSource;
        }

        public Node GetChildNode(char c)
        {
            if (this.Children.Any(x => char.ToLower(x.NodeKey) == char.ToLower(c)))
                return this.Children.First(x => char.ToLower(x.NodeKey) == char.ToLower(c));
            else
                return null;
        }

        public Node GetChildNode(char c, bool caseSensitivity)
        {
            if (caseSensitivity)
            {
                if (this.Children.Any(x => x.NodeKey == c))
                    return this.Children.First(x => x.NodeKey == c);
                else
                    return null;
            }
            else
            {
                return this.GetChildNode(c);
            }
        }

        public void AddNewItem(string path, object item)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                return;

            char keyChar = path[0];
            if (!this.Children.Any(x => x.NodeKey == keyChar))
            {
                // Düğüm oluştur.
                if (!this.Objects.Contains(item))
                    this.Objects.Add(item);

                this.Children.Add(new Node(keyChar, new HashSet<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(path.Substring(1, path.Length - 1), item) }));
            }
            else
            {
                // Düğüm zaten oluşturulmuş.
                if (!this.Objects.Contains(item))
                    this.Objects.Add(item);

                Node child = this.GetChildNode(keyChar, false);
                child.AddNewItem(path.Substring(1, path.Length - 1), item);
            }
        }

        public void RemoveItem(string path, object item)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                return;

            char keyChar = path[0];
            Node child = this.GetChildNode(keyChar, false);
            if (child != null)
            {
                child.RemoveItem(path.Substring(1, path.Length - 1), item);

                if (child.Objects.Count == 0)
                    this.Children.Remove(child);
            }

            if (this.Objects.Contains(item))
                this.Objects.Remove(item);
        }

        public override string ToString()
        {
            return "[" + this.NodeKey + "]";
        }

        #endregion

        #region Dispose

        public void Destroy()
        {
            foreach (var child in this.Children)
                child.Destroy();

            this.Objects.Clear();
            this.Children.Clear();
        }

        #endregion
    }
}
