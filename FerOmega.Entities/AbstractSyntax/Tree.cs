using System;
using System.Linq;

namespace FerOmega.Entities.AbstractSyntax
{
    public class Tree<T>
    {
        public Guid Id { get; private set; }

        public Node<T> Root { get; }

        public Tree() : this(new Node<T>())
        {
        }

        public Tree(T body) : this(new Node<T>(body))
        {
        }

        public Tree(Node<T> node) : this(node, new T[0])
        {
        }

        public Tree(Node<T> node, params T[] childrenBodies)
        {
            Root = node;

            Id = Guid.NewGuid();

            foreach (var childrenBody in childrenBodies)
            {
                Root.Append(childrenBody);
            }
        }

        public static Tree<T> Join(Tree<T> leftTree, T dataForRootNode, Tree<T> rightTree)
        {
            return Join(leftTree, new Node<T>(dataForRootNode), rightTree);
        }

        public static Tree<T> Join(Tree<T> leftTree, Node<T> root, Tree<T> rightTree)
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

            root.Append(leftTree.Root);
            root.Append(rightTree.Root);

            return resultTree;
        }

        public Node<T> AppendToRoot(T data)
        {
            return AppendToRoot(new Tree<T>(data));
        }

        public Node<T> AppendToRoot(Node<T> node)
        {
            return AppendToRoot(new Tree<T>(node));
        }

        public Node<T> AppendToRoot(Tree<T> tree)
        {
            Root.Append(tree.Root);

            return tree.Root;
        }

        public Tree<T> DeepClone(bool useOldId = false)
        {
            var treeRoot = Root.DeepClone();
            var tree = new Tree<T>(treeRoot);

            if (useOldId)
            {
                tree.Id = Id;
            }

            return tree;
        }

        public Node<T> Find(Func<Node<T>, bool> filter)
        {
            return Root.Find(filter);
        }

        public Node<T> Find(Node<T> node)
        {
            return Find(node.Id);
        }

        public Node<T> Find(Guid nodeId)
        {
            return Root.Find(nodeId);
        }

        public bool HasChildren()
        {
            return Root.Children.Any();
        }

        public bool HasRoot()
        {
            return Root != null;
        }
    }
}