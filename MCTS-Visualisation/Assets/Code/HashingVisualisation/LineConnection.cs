using UnityEngine;

namespace MCTS.Visualisation.Hashing
{
    /// <summary>
    /// A container class which contains a line renderer and its target <see cref="HashNode"/>
    /// </summary>
    public class LineConnection
    {
        /// <summary>
        /// The target object of the <see cref="Line"/> being rendered
        /// </summary>
        public HashNode ConnectedNode { get; private set; }

        /// <summary>
        /// The <see cref="LineRenderer"/> used to render the line between an object and the target <see cref="ConnectedNode"/>
        /// </summary>
        public LineRenderer Line { get; private set; }

        /// <summary>
        /// Creates a new LineConnection with the provided connected node and line renderer
        /// </summary>
        /// <param name="connectedNode">The target object of the line being rendered</param>
        /// <param name="line">The linerenderer used to render the line between an object and the target object</param>
        public LineConnection(HashNode connectedNode, LineRenderer line)
        {
            ConnectedNode = connectedNode;
            Line = line;
        }
    }
}