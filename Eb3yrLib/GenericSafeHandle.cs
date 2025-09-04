namespace Eb3yrLib
{
	/// <summary>A safe handle that receives a delegate for deletion.</summary>
	/// <remarks>Pointers cannot be marshalled to this handle because a delegate to delete the unmanaged resource must be provided upon creation.</remarks>
	public sealed class GenericSafeHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
	{
		public readonly Action<nint> _releaseDelegate;

		// While documentation advises us to implement a parameterless constructor so that a native method returning a pointer can be marshalled as a SafeHandle,
		// we deliberately do not implement it so as to cause a compile-time error, as we require a releaseDelegate to be provided upon creation.

		public GenericSafeHandle(nint pointer, Action<nint> releaseDelegate) : base(true)
		{
			SetHandle(pointer);
			_releaseDelegate = releaseDelegate;
		}

		protected override bool ReleaseHandle()
		{
			if (!IsInvalid)
				_releaseDelegate.Invoke(handle);

			return true;
		}
	}
}
