using UnityEngine;

namespace MCTS.Visualisation.Tree
{
    /// <summary>
    /// Contains utility methods to further extend the functionality of <see cref="Vector3"/>
    /// </summary>
    public static class Vector3Extension
    {
        /// <summary>
        /// Rotates a vector around a pivot in an axis, by an amount of degrees
        /// </summary>
        /// <param name="point">The point being rotated</param>
        /// <param name="pivot">The point to rotate around</param>
        /// <param name="axis">The axis to rotate in</param>
        /// <param name="degrees">The amount of degrees to rotate by</param>
        /// <returns>The resultant position of the point after rotation</returns>
        public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Vector3 axis, float degrees)
        {
            return Quaternion.AngleAxis(degrees, axis) * (point - pivot) + pivot;
        }
    }
}