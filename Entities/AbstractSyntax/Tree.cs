using System;
using System.Collections.Generic;
using System.Linq;

namespace FerOmega.Entities.AbstractSyntax
{
    public class Tree<T>
    {
        public Tree()
            : this(default(T)) { }

        public Tree(T body)
            : this(new Node<T>(body)) { }

        public Tree(Node<T> node)
        {
            Root = node;

            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public Node<T> Root { get; }

        public bool IsEmpty => Root == default || ReferenceEquals(Root.Body, default);

        public Node<T> AppendToRoot(Tree<T> tree)
        {
            Root.Append(tree.Root);

            return tree.Root;
        }

        public void BreadthFirst(Action<Node<T>> onEnter = null, Action<Node<T>> onLeave = null)
        {
            var queue = new Queue<Node<T>>();

            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                onEnter?.Invoke(node);

                node.Color = NodeColor.Black;

                foreach (var child in node.Children
                                          .Where(x => x.Color == NodeColor.White &&
                                                      !queue.Contains(x)))
                {
                    child.Color = NodeColor.Grey;
                    queue.Enqueue(child);
                }

                onLeave?.Invoke(node);
            }
        }

        public void DeepFirst(Action<Node<T>> onEnter = null, Action<Node<T>> onLeave = null)
        {
            var stack = new Stack<Node<T>>();

            stack.Push(Root);

            while (stack.Count > 0)
            {
                var node = stack.Peek();

                switch (node.Color)
                {
                case NodeColor.White:
                {
                    node.Color = NodeColor.Grey;
                    onEnter?.Invoke(node);

                    foreach (var child in node.Children
                                              .Where(x => x.Color == NodeColor.White))
                    {
                        stack.Push(child);
                    }

                    break;
                }

                case NodeColor.Black:
                case NodeColor.Grey:
                {
                    node = stack.Pop();
                    node.Color = NodeColor.Black;
                    onLeave?.Invoke(node);

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}