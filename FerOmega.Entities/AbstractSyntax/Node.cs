using System;
using System.Collections.Generic;
using System.Linq;

namespace FerOmega.Entities.RedBlack
{
    public static class NodeExtentions
    {
        public static Node<T> ToNode<T>(this T body)
        {
            return new Node<T>(body);
        }
    }

    public class Node<T>
    {
        public Node() : this(default(T))
        {
        }

        public Node(T body)
        {
            Body = body;

            Id = Guid.NewGuid();

            children = new List<Node<T>>();
        }

        public Guid Id { get; private set; }

        public T Body { get; set; }

        private readonly IList<Node<T>> children;

        public IEnumerable<Node<T>> Children => children;

        public Node<T> Parent { get; set; }

        public Node<T> Append(T body)
        {
            return Append(new Node<T>(body));
        }

        public Node<T> Append(T body, Guid id)
        {
            var node = new Node<T>(body)
            {
                Id = id,
            };

            return Append(node);
        }

        public Node<T> Append(Tree<T> tree)
        {
            Append(tree.Root);
            return this;
        }

        public Node<T> Append(Node<T> child)
        {
            children.Add(child);
            return this;
        }

        public override string ToString()
        {
            return $"{{ {Body} -> {children.Count} }}";
        }

        public Node<T> DeepClone(bool useOldId = false)
        {
            var node = new Node<T>
            {
                Parent = Parent,
            };

            if (useOldId)
            {
                node.Id = Id;
            }

            foreach (var child in children)
            {
                var nodeChild = child.DeepClone();
                node.Append(nodeChild);
            }

            return node;
        }

        public bool IsLocus()
        {
            return children.Count == 0;
        }

        public Node<T> Find(Func<Node<T>, bool> filter)
        {
            if (IsLocus())
            {
                return null;
            }

            if (filter(this))
            {
                return this;
            }

            var thisNodeChild = Children.FirstOrDefault(filter);

            if (thisNodeChild != null)
            {
                return thisNodeChild;
            }

            foreach (var child in Children)
            {
                var childNodeChild = child.Find(filter);

                if (childNodeChild != null)
                {
                    return childNodeChild;
                }
            }

            return null;
        }

        public Node<T> Find(Guid nodeId)
        {
            return Find(x => x.Id == nodeId);
        }
    }
}