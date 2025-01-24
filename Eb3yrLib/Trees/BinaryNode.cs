using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Trees
{
	public class BinaryNode<T>(T value, BinaryNode<T>? left, BinaryNode<T>? right) : IBinaryNode<BinaryNode<T>, T>
	{
		public BinaryNode<T>? Left { get; set; } = left;
		public BinaryNode<T>? Right { get; set; } = right;
		public T Value { get; set; } = value;
	}
}
