using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigen.Core.Utility
{
    public class SearchTree
    {
        #region Fields

        private readonly char RootKey = '.';
        private Node root;

        #endregion

        #region Properties

        public Node Root
        {
            get
            {
                return root;
            }

            set
            {
                root = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchTree(HashSet<KeyValuePair<string, object>> source)
        {
            this.Root = new Node(this.RootKey, source);
        }

        internal Node FindNode(string searchKey)
        {
            Node node = this.Root;
            foreach (char c in searchKey)
            {
                node = node.GetChildNode(c);
                if (node == null)
                    return null;
            }

            return node;
        }

        #endregion

        #region Methods

        internal void AddItems(HashSet<KeyValuePair<string, object>> addedItems)
        {
            if (this.Root == null)
                return;

            foreach (var item in addedItems)
            {
                this.AddItem(item);
            }
        }

        private void AddItem(KeyValuePair<string, object> item)
        {
            this.Root.AddNewItem(item.Key, item.Value);
        }

        internal void RemoveItems(HashSet<KeyValuePair<string, object>> removedItems)
        {
            if (this.Root == null)
                return;

            foreach (var item in removedItems)
            {
                this.RemoveItem(item);
            }
        }

        private void RemoveItem(KeyValuePair<string, object> item)
        {
            this.Root.RemoveItem(item.Key, item.Value);
        }

        public void Destroy()
        {
            if (this.Root != null)
                this.Root.Destroy();
            this.Root = null;
        }

        #endregion
    }
}
