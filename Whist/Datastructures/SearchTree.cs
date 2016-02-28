using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.Datastructures
{

    public class SearchTree<T>
    {
        private SearchTreeNode<T> root;

        public SearchTree()
        {
            root = null;
        }

        public virtual void Clear()
        {
            root = null;
        }

        public SearchTreeNode<T> Root
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
    }

    public delegate void TreeVisitor<T>(T nodeData);

    public class SearchTreeNode<T>
    {
        private T data;
        public T Data { get { return data; } }

        private LinkedList<SearchTreeNode<T>> children;
        public LinkedList<SearchTreeNode<T>> Children { get { return children; } }

        public SearchTreeNode(T data)
        {
            this.data = data;
            children = new LinkedList<SearchTreeNode<T>>();
        }

        public void AddChild(T data)
        {
            children.AddFirst(new SearchTreeNode<T>(data));
        }

        public void AddChild(SearchTreeNode<T> data)
        {
            children.AddFirst(data);
        }


        public SearchTreeNode<T> GetChild(int i)
        {
            foreach (SearchTreeNode<T> n in children)
                if (--i == 0)
                    return n;
            return null;
        }

        public void Traverse(SearchTreeNode<T> node, TreeVisitor<T> visitor)
        {
            visitor(node.data);
            foreach (SearchTreeNode<T> kid in node.children)
                Traverse(kid, visitor);
        }

        public override string ToString() {
            return data.ToString();
        }
    }
}
