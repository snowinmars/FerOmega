using System;
using System.Collections.Generic;
using System.Linq;

namespace FerOmega.Entities.AbstractSyntax
{
    public class Node<T>
    {
        public Node()
            : this(default(T)) { }

        public Node(T body)
        {
            Id = Guid.NewGuid();
            children = new List<Node<T>>();
            Color = NodeColor.White;
            
            Body = body;
        }

        public NodeColor Color { get; set; }
        
        public T Body { get; set; }

        public IEnumerable<Node<T>> Children => children;

        public Guid Id { get; private set; }

        public Node<T> Parent { get; private set; }

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