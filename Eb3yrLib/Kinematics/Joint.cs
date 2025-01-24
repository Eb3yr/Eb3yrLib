using System.Numerics;

namespace Eb3yrLib.Kinematics
{
    /// <summary>
    /// A joint in a chain used for inverse kinematics. Old, not recommended, use an IList of vectors to store this info with the Kinematics class instead unless your situation calls for acting upon a custom data type instead.
    /// </summary>
    [Obsolete()]
    public struct Joint
    {
        public float length;
        public Vector3 pos; // All but pos required for the Triangulation method. FABRIK calculates length from difference in start positions and therefore only needs pos.
        public Vector3 dir;
        public readonly Vector3 LengthDirection { get => dir * length; }
        public readonly float Angle { get => float.Acos(Vector3.Dot(dir, Vector3.UnitZ)); } // Angle about the 3rd axis.
    }
}
