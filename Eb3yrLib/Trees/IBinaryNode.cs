namespace Eb3yrLib.Trees
{
	public interface IBinaryNode<TSelf, TValue> where TSelf : IBinaryNode<TSelf, TValue>
	{
		/// <summary>The left node below thist node in the binary tree</summary>
		public TSelf? Left { get; }
		/// <summary>The right node below this node in the binary tree</summary>
		public TSelf? Right { get; }
		/// <summary>Value of the node</summary>
		public TValue Value { get; }
	}
}
