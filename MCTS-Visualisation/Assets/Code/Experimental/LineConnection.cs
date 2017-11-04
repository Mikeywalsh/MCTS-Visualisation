using UnityEngine;

public class LineConnection {

	public HashNode ConnectedNode { get; private set; }

    public LineRenderer Line { get; private set; }

    public LineConnection(HashNode connectedNode, LineRenderer line)
    {
        ConnectedNode = connectedNode;
        Line = line;
    }
}
