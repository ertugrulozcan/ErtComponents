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

        public override string ToString()
        {
            return "[" + this.NodeKey + "]";
        }

        #endregion
    }
}
