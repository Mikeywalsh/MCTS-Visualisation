using UnityEngine;

/// <summary>
/// A struct representing a colored line <para/>
/// Contains the lines starting point, destination point and color
/// </summary>
public struct ColoredLine {

    /// <summary>
    /// The starting point of this line
    /// </summary>
    public Vector3 From;

    /// <summary>
    /// The destination point of this line
    /// </summary>
    public Vector3 To;

    /// <summary>
    /// The color of this line
    /// </summary>
    public Color LineColor;

    /// <summary>
    /// Creates a new colored line with a given start and destination point and color
    /// </summary>
    /// <param name="from">The starting point of this line</param>
    /// <param name="to">The destination point of this line</param>
    /// <param name="lineColor">The color of this line</param>
    public ColoredLine(Vector3 from, Vector3 to, Color lineColor)
    {
        From = from;
        To = to;
        LineColor = lineColor;
    }
}
