using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerOmega.Entities.RedBlack
{
    public class Node<T>
    {
        public Node() : this(default(T), Color.Black)
        {
        }

        private Node(T body, Color color)
        {
            this.Body = body;
            this.Color = color;

            this.Id = Guid.NewGuid();
            this.children = new List<Node<T>>(4);
        }

        public Guid Id { get; private set; }

        public T Body { get; set; }

        public Color Color { get; set; }

        private readonly IList<Node<T>> children;

        public IEnumerable<Node<T>> Children => children;

        public Node<T> Parent { get; set; }

        public void Add(Node<T> child)
        {
            this.children.Add(child);
        }
        
        public override string ToString()
        {
            return $"{{ {Body} -> {children.Count} }}";
        }

        public Node<T> DeepClone(bool useOldId = false)
        {
            var node = new Node<T>
            {
                Color = this.Color,
                Parent = this.Parent,
            };

            if (useOldId)
            {
                node.Id = this.Id;
            }

            foreach (var child in this.children)
            {
                var nodeChild = child.DeepClone();
                node.Add(nodeChild);
            }

            return node;
        }
    }
}
