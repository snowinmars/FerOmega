using System;
using System.Collections.Generic;

namespace FerOmega.Entities.AbstractSyntax
{
    public class Node<T>
    {
        public Node(T body)
        {
            Id = Guid.NewGuid();
            children = new List<Node<T>>();
            Color = NodeColor.White;

            Body = body;
        }

        public NodeColor Color { get; set; }

        public T Body { get; }

        public IEnumerable<Node<T>> Children => children;

        public Guid Id { get; }

        private readonly IList<Node<T>> children;

        public Node<T> Append(Node<T> child)
        {
            children.Add(child);

            return this;
        }

        public override string ToString()
        {
            return $"{{ {Body} -> {children.Count} }}";
        }
    }
}
