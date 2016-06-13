using System;
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



        #endregion
    }
}
