using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace MathNet.Spatial.Internals
{
    /// <summary>
    /// 2016-12-08, Eric Ouellet
    /// The code is an adapted version of BitLush AvlTree: https://bitlush.com/blog/efficient-avl-tree-in-c-sharp
    /// </summary>
    public class AvlTreeSet<T> : IEnumerable<T>, IEnumerable, ICollection<T>, ICollection// ISet<T>
    {
        private IComparer<T> comparer;
        private AvlNode<T> root;
        protected int count = 0;

        public AvlTreeSet(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        // ******************************************************************
        public AvlTreeSet() : this(Comparer<T>.Default)
        {
        }

        public AvlNode<T> Root => root;

        public int Count => count;

        private object _syncRoot;

        public object SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), (object)null);
                return this._syncRoot;
            }
        }

        public bool IsSynchronized => true;

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            return new AvlNodeItemEnumerator<T>(this);
        }

        protected AvlNode<T> GetNode(T item)
        {
            AvlNode<T> node = root;

            while (node != null)
            {
                int compareResult = comparer.Compare(item, node.Item);
                if (compareResult < 0)
                {
                    node = node.Left;
                }
                else if (compareResult > 0)
                {
                    node = node.Right;
                }
                else
                {
                    return node;
                }
            }

            return null;
        }

        public bool Contains(T item)
        {
            AvlNode<T> node = root;

            while (node != null)
            {
                int compareResult = this.comparer.Compare(item, node.Item);
                if (compareResult < 0)
                {
                    node = node.Left;
                }
                else if (compareResult > 0)
                {
                    node = node.Right;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool Add(T item)
        {
            AvlNode<T> node = root;

            while (node != null)
            {
                int compare = comparer.Compare(item, node.Item);

                if (compare < 0)
                {
                    AvlNode<T> left = node.Left;

                    if (left == null)
                    {
                        node.Left = new AvlNode<T> { Item = item, Parent = node };
                        AddBalance(node, 1);
                        return true;
                    }
                    else
                    {
                        node = left;
                    }
                }
                else if (compare > 0)
                {
                    AvlNode<T> right = node.Right;

                    if (right == null)
                    {
                        node.Right = new AvlNode<T> { Item = item, Parent = node };
                        AddBalance(node, -1);
                        return true;
                    }
                    else
                    {
                        node = right;
                    }
                }
                else
                {
                    return false;
                }
            }

            this.root = new AvlNode<T> { Item = item };
            this.count++;

            return true;
        }

        /// <summary>
        /// Should always be called for any inserted node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="balance"></param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AddBalance(AvlNode<T> node, int balance)
        {
            count++;

            while (node != null)
            {
                balance = (node.Balance += balance);

                if (balance == 0)
                {
                    break;
                }

                if (balance == 2)
                {
                    if (node.Left.Balance == 1)
                    {
                        RotateRight(node);
                    }
                    else
                    {
                        RotateLeftRight(node);
                    }

                    break;
                }

                if (balance == -2)
                {
                    if (node.Right.Balance == -1)
                    {
                        RotateLeft(node);
                    }
                    else
                    {
                        RotateRightLeft(node);
                    }

                    break;
                }

                AvlNode<T> parent = node.Parent;

                if (parent != null)
                {
                    balance = parent.Left == node ? 1 : -1;
                }

                node = parent;
            }
        }

        protected AvlNode<T> RotateLeft(AvlNode<T> node)
        {
            AvlNode<T> right = node.Right;
            AvlNode<T> rightLeft = right.Left;
            AvlNode<T> parent = node.Parent;

            right.Parent = parent;
            right.Left = node;
            node.Right = rightLeft;
            node.Parent = right;

            if (rightLeft != null)
            {
                rightLeft.Parent = node;
            }

            if (node == root)
            {
                root = right;
            }
            else if (parent.Right == node)
            {
                parent.Right = right;
            }
            else
            {
                parent.Left = right;
            }

            right.Balance++;
            node.Balance = -right.Balance;

            return right;
        }

        protected AvlNode<T> RotateRight(AvlNode<T> node)
        {
            AvlNode<T> left = node.Left;
            AvlNode<T> leftRight = left.Right;
            AvlNode<T> parent = node.Parent;

            left.Parent = parent;
            left.Right = node;
            node.Left = leftRight;
            node.Parent = left;

            if (leftRight != null)
            {
                leftRight.Parent = node;
            }

            if (node == root)
            {
                root = left;
            }
            else if (parent.Left == node)
            {
                parent.Left = left;
            }
            else
            {
                parent.Right = left;
            }

            left.Balance--;
            node.Balance = -left.Balance;

            return left;
        }

        protected AvlNode<T> RotateLeftRight(AvlNode<T> node)
        {
            AvlNode<T> left = node.Left;
            AvlNode<T> leftRight = left.Right;
            AvlNode<T> parent = node.Parent;
            AvlNode<T> leftRightRight = leftRight.Right;
            AvlNode<T> leftRightLeft = leftRight.Left;

            leftRight.Parent = parent;
            node.Left = leftRightRight;
            left.Right = leftRightLeft;
            leftRight.Left = left;
            leftRight.Right = node;
            left.Parent = leftRight;
            node.Parent = leftRight;

            if (leftRightRight != null)
            {
                leftRightRight.Parent = node;
            }

            if (leftRightLeft != null)
            {
                leftRightLeft.Parent = left;
            }

            if (node == root)
            {
                root = leftRight;
            }
            else if (parent.Left == node)
            {
                parent.Left = leftRight;
            }
            else
            {
                parent.Right = leftRight;
            }

            if (leftRight.Balance == -1)
            {
                node.Balance = 0;
                left.Balance = 1;
            }
            else if (leftRight.Balance == 0)
            {
                node.Balance = 0;
                left.Balance = 0;
            }
            else
            {
                node.Balance = -1;
                left.Balance = 0;
            }

            leftRight.Balance = 0;

            return leftRight;
        }

        protected AvlNode<T> RotateRightLeft(AvlNode<T> node)
        {
            AvlNode<T> right = node.Right;
            AvlNode<T> rightLeft = right.Left;
            AvlNode<T> parent = node.Parent;
            AvlNode<T> rightLeftLeft = rightLeft.Left;
            AvlNode<T> rightLeftRight = rightLeft.Right;

            rightLeft.Parent = parent;
            node.Right = rightLeftLeft;
            right.Left = rightLeftRight;
            rightLeft.Right = right;
            rightLeft.Left = node;
            right.Parent = rightLeft;
            node.Parent = rightLeft;

            if (rightLeftLeft != null)
            {
                rightLeftLeft.Parent = node;
            }

            if (rightLeftRight != null)
            {
                rightLeftRight.Parent = right;
            }

            if (node == root)
            {
                root = rightLeft;
            }
            else if (parent.Right == node)
            {
                parent.Right = rightLeft;
            }
            else
            {
                parent.Left = rightLeft;
            }

            if (rightLeft.Balance == 1)
            {
                node.Balance = 0;
                right.Balance = -1;
            }
            else if (rightLeft.Balance == 0)
            {
                node.Balance = 0;
                right.Balance = 0;
            }
            else
            {
                node.Balance = 1;
                right.Balance = 0;
            }

            rightLeft.Balance = 0;

            return rightLeft;
        }

        public virtual bool Remove(T item)
        {
            AvlNode<T> node = root;

            while (node != null)
            {
                if (this.comparer.Compare(item, node.Item) < 0)
                {
                    node = node.Left;
                }
                else if (this.comparer.Compare(item, node.Item) > 0)
                {
                    node = node.Right;
                }
                else
                {
                    this.RemoveNode(node);
                    return true;
                }
            }

            return false;
        }

        protected void RemoveNode(AvlNode<T> node)
        {
            this.count--;

            AvlNode<T> left = node.Left;
            AvlNode<T> right = node.Right;

            if (left == null)
            {
                if (right == null)
                {
                    if (node == root)
                    {
                        root = null;
                    }
                    else
                    {
                        if (node.Parent.Left == node)
                        {
                            node.Parent.Left = null;

                            RemoveBalance(node.Parent, -1);
                        }
                        else if (node.Parent.Right == node)
                        {
                            node.Parent.Right = null;

                            RemoveBalance(node.Parent, 1);
                        }
                    }
                }
                else
                {
                    Replace(node, right);

                    RemoveBalance(node, 0);
                }
            }
            else if (right == null)
            {
                Replace(node, left);

                RemoveBalance(node, 0);
            }
            else
            {
                AvlNode<T> successor = right;

                if (successor.Left == null)
                {
                    AvlNode<T> parent = node.Parent;

                    successor.Parent = parent;
                    successor.Left = left;
                    successor.Balance = node.Balance;

                    left.Parent = successor;

                    if (node == root)
                    {
                        root = successor;
                    }
                    else
                    {
                        if (parent.Left == node)
                        {
                            parent.Left = successor;
                        }
                        else
                        {
                            parent.Right = successor;
                        }
                    }

                    RemoveBalance(successor, 1);
                }
                else
                {
                    while (successor.Left != null)
                    {
                        successor = successor.Left;
                    }

                    AvlNode<T> parent = node.Parent;
                    AvlNode<T> successorParent = successor.Parent;
                    AvlNode<T> successorRight = successor.Right;

                    if (successorParent.Left == successor)
                    {
                        successorParent.Left = successorRight;
                    }
                    else
                    {
                        successorParent.Right = successorRight;
                    }

                    if (successorRight != null)
                    {
                        successorRight.Parent = successorParent;
                    }

                    successor.Parent = parent;
                    successor.Left = left;
                    successor.Balance = node.Balance;
                    successor.Right = right;
                    right.Parent = successor;

                    left.Parent = successor;

                    if (node == root)
                    {
                        root = successor;
                    }
                    else
                    {
                        if (parent.Left == node)
                        {
                            parent.Left = successor;
                        }
                        else
                        {
                            parent.Right = successor;
                        }
                    }

                    RemoveBalance(successorParent, -1);
                }
            }
        }

        /// <summary>
        /// Shoould always be called for any removed node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="balance"></param>
        protected void RemoveBalance(AvlNode<T> node, int balance)
        {
            while (node != null)
            {
                balance = node.Balance += balance;

                if (balance == 2)
                {
                    if (node.Left.Balance >= 0)
                    {
                        node = RotateRight(node);

                        if (node.Balance == -1)
                        {
                            return;
                        }
                    }
                    else
                    {
                        node = this.RotateLeftRight(node);
                    }
                }
                else if (balance == -2)
                {
                    if (node.Right.Balance <= 0)
                    {
                        node = RotateLeft(node);

                        if (node.Balance == 1)
                        {
                            return;
                        }
                    }
                    else
                    {
                        node = RotateRightLeft(node);
                    }
                }
                else if (balance != 0)
                {
                    return;
                }

                AvlNode<T> parent = node.Parent;

                if (parent != null)
                {
                    balance = parent.Left == node ? -1 : 1;
                }

                node = parent;
            }
        }

        private static void Replace(AvlNode<T> target, AvlNode<T> source)
        {
            AvlNode<T> left = source.Left;
            AvlNode<T> right = source.Right;

            target.Balance = source.Balance;
            target.Item = source.Item;
            target.Left = left;
            target.Right = right;

            if (left != null)
            {
                left.Parent = target;
            }

            if (right != null)
            {
                right.Parent = target;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T GetFirstItem()
        {
            AvlNode<T> node = GetFirstNode();
            if (node != null)
            {
                return node.Item;
            }

            return default(T);
        }

        public AvlNode<T> GetFirstNode()
        {
            if (Root != null)
            {
                AvlNode<T> current = Root;
                while (current.Left != null)
                {
                    current = current.Left;
                }

                return current;
            }

            return null;
        }

        public T GetLastItem()
        {
            AvlNode<T> node = GetLastNode();
            if (node != null)
            {
                return node.Item;
            }

            return default(T);
        }

        public AvlNode<T> GetLastNode()
        {
            if (this.Root != null)
            {
                AvlNode<T> current = this.Root;
                while (current.Right != null)
                {
                    current = current.Right;
                }

                return current;
            }

            return null;
        }

        private int RecursiveCount(AvlNode<T> node)
        {
            if (node == null)
            {
                return 0;
            }

            return 1 + this.RecursiveCount(node.Left) + this.RecursiveCount(node.Right);
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            this.root = null;
            this.count = 0;
        }

        public void CopyTo(T[] array, int index, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("'array' can't be null");
            }

            if (index < 0)
            {
                throw new ArgumentException("'index' can't be null");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("'count' should be greater or equal to 0");
            }

            if (index > array.Length || count > array.Length - index)
            {
                throw new ArgumentException("The array size is not big enough to get all items");
            }

            if (count == 0)
            {
                return;
            }

            int indexIter = 0;
            int indexArray = 0;

            AvlNode<T> current = this.GetFirstNode();
            while (current.GetNextNode() != null)
            {
                if (indexIter >= index)
                {
                    array[indexArray] = current.Item;
                    indexArray++;
                    count--;
                    if (count == 0)
                    {
                        return;
                    }
                }

                indexIter++;
            }

            /*
            foreach (AvlNode<T> node in this.Nodes())
            {
                if (indexIter >= index)
                {
                    array[indexArray] = node.Item;
                    indexArray++;
                    count--;
                    if (count == 0)
                    {
                        return;
                    }
                }

                indexIter++;
            }*/
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            SortedSet<double> t = new SortedSet<double>();
            this.CopyTo(array, arrayIndex, this.Count);
        }

        public void CopyTo(Array array, int index)
        {
            this.CopyTo(array as T[], index, this.Count);
        }

        private int RecursiveGetChildMaxHeight(AvlNode<T> node)
        {
            if (node == null)
            {
                return 0;
            }

            int leftHeight = 0;
            if (node.Left != null)
            {
                leftHeight = this.RecursiveGetChildMaxHeight(node.Left);
            }

            int rightHeight = 0;
            if (node.Right != null)
            {
                rightHeight = this.RecursiveGetChildMaxHeight(node.Right);
            }

            return 1 + Math.Max(leftHeight, rightHeight);
        }
    }
}
