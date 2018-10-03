using System;
using System.Collections.Generic;
using System.Linq;

namespace FerOmega.Entities.RedBlack
{
    public class Tree<T>
    {
        private Node<T> Root { get; set; }

        public Guid Id { get; private set; }

        public Tree() : this(new Node<T>())
        {
        }

        public Tree(Node<T> node)
        {
            this.Root = node;

            this.Id = Guid.NewGuid();
        }

        public bool HasRoot()
        {
            return this.Root != null;
        }

        public bool HasChildren()
        {
            return this.Root.Children.Any();
        }

        public static Tree<T> Form(Tree<T> leftTree, Node<T> root, Tree<T> rightTree)
        {
            if (leftTree == null || rightTree == null)
            {
                throw new ArgumentNullException();
            }

            if (!leftTree.HasRoot() || !rightTree.HasRoot())
            {
                throw new InvalidOperationException("Can't form tree from an empty tree");
            }

            var resultTree = new Tree<T>(root);

            root.Add(leftTree.Root);
            root.Add(rightTree.Root);

            return resultTree;
        }

        public void AppendToRoot(Tree<T> tree)
        {
            this.Root.Add(tree.Root);
        }

        public Tree<T> DeepClone(bool useOldId = false)
        {
            var treeRoot = this.Root.DeepClone();
            var tree = new Tree<T>(treeRoot);

            if (useOldId)
            {
                tree.Id = this.Id;
            }

            return tree;
        }
    }
}