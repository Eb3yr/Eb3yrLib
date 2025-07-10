using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Trees
{
	// Make a non-binary tree one later. It shouldn't be very complicated for level order, pre order and post order. Not sure about in-order. Maybe that splits on the midpoint?
	// That would require storing a collection of nodes instead of Left and Right. Pretty simple change
	public static class BinaryTraversal
	{
		public static IEnumerable<IBinaryNode<TSelf, TValue>> LevelOrder<TSelf, TValue>(this IBinaryNode<TSelf, TValue> root) where TSelf : IBinaryNode<TSelf, TValue>
		{
			Queue<IBinaryNode<TSelf, TValue>> q = new([root]);

			while (q.Count > 0)
			{
				IBinaryNode<TSelf, TValue> popped = q.Dequeue();
				yield return popped;
				if (popped.Left is not null)
					q.Enqueue(popped.Left);
				if (popped.Right is not null)
					q.Enqueue(popped.Right);
			}
		}

		// Level order traversal that separates each level of nodes into its own enumerable, in order from top to bottom
		public static IEnumerable<IEnumerable<IBinaryNode<TSelf, TValue>>> LevelOrderDiscrete<TSelf, TValue>(this IBinaryNode<TSelf, TValue> root) where TSelf : IBinaryNode<TSelf, TValue>
		{
			Queue<IBinaryNode<TSelf, TValue>> q = new([root]);

			while (q.Count > 0)
			{
				IBinaryNode<TSelf, TValue>[] nodes = new IBinaryNode<TSelf, TValue>[q.Count];
				for (int i = 0; i < nodes.Length; i++)	// q.Count will change for each iteration as enqueue and dequeue get called. Store the initial value as a local or use nodes.Length 
				{
					IBinaryNode<TSelf, TValue> popped = q.Dequeue();
					nodes[i] = popped;
					if (popped.Left is not null)
						q.Enqueue(popped.Left);
					if (popped.Right is not null)
						q.Enqueue(popped.Right);
				}
				yield return nodes;
			}
		}

		public static IEnumerable<IBinaryNode<TSelf, TValue>> PreOrder<TSelf, TValue>(this IBinaryNode<TSelf, TValue> root) where TSelf : IBinaryNode<TSelf, TValue>
		{
			throw new NotImplementedException();
		}

		public static IEnumerable<IBinaryNode<TSelf, TValue>> InOrder<TSelf, TValue>(this IBinaryNode<TSelf, TValue> root) where TSelf : IBinaryNode<TSelf, TValue>
		{
			throw new NotImplementedException();
		}

		public static IEnumerable<IBinaryNode<TSelf, TValue>> PostOrder<TSelf, TValue>(this IBinaryNode<TSelf, TValue> root) where TSelf : IBinaryNode<TSelf, TValue>
		{
			throw new NotImplementedException();
		}
	}
}
