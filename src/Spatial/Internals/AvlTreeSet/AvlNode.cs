namespace MathNet.Spatial.Internals
{
    internal sealed class AvlNode<T>
    {
        public AvlNode<T> Parent;
        public AvlNode<T> Left;
        public AvlNode<T> Right;
        public T Item;
        public int Balance;

        /// <summary>
        /// Non recursive function that return the next ordered node
        /// </summary>
        /// <returns></returns>
        public AvlNode<T> GetNextNode()
        {
            AvlNode<T> current;

            if (this.Right != null)
            {
                current = this.Right;
                while (current.Left != null)
                {
                    current = current.Left;
                }
                return current;
            }

            current = this;
            while (current.Parent != null)
            {
                if (current.Parent.Left == current)
                {
                    return current.Parent;
                }

                current = current.Parent;
            }

            return null;
        }

        /// <summary>
        /// Non recursive function that return the previous ordered node
        /// </summary>
        /// <returns></returns>
        public AvlNode<T> GetPreviousNode()
        {
            AvlNode<T> current;

            if (this.Left != null)
            {
                current = this.Left;
                while (current.Right != null)
                {
                    current = current.Right;
                }
                return current;
            }

            current = this;
            while (current.Parent != null)
            {
                if (current.Parent.Right == current)
                {
                    return current.Parent;
                }

                current = current.Parent;
            }

            return null;
        }

        public override string ToString()
        {
            return $"AvlNode [{this.Item}], balance: {this.Balance}, Parent: {this.Parent?.Item.ToString()}, Left: {this.Left?.Item.ToString()}, Right: {this.Right?.Item.ToString()},";
        }
    }
}
