using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece
{
    public ChessPieces PieceType { get; private set; }
    public int XPos { get; private set; }
    public int YPos { get; private set; }

    public ChessPiece(ChessPieces pieceType, int xPos, int yPos)
    {
        PieceType = pieceType;
        XPos = xPos;
        YPos = yPos;
    }

    public static bool operator ==(ChessPiece p1, ChessPiece p2)
    {
        //If one piece is null and the other isn't, return false, if they are both null, return true
        if (object.ReferenceEquals(p1, null) || object.ReferenceEquals(p2, null))
        {
            return object.ReferenceEquals(p1, null) && object.ReferenceEquals(p2, null);
        }

        return  p1.PieceType == p2.PieceType &&
                p1.XPos == p2.XPos &&
                p1.YPos == p2.YPos;
    }

    public static bool operator !=(ChessPiece p1, ChessPiece p2)
    {
        return !(p1 == p2);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is ChessPiece))
            return false;

        return this == (ChessPiece)obj;
    }

    /// <summary>
    /// Returns a 4 digit hash code which uniquely describes a ChessPiece
    /// For example, a White Queen at position 3,4 would have a hashcode of - 1534
    /// </summary>
    /// <returns>A hashcode uniquely describing this chess piece</returns>
    public override int GetHashCode()
    {
        return int.Parse(((int)PieceType).ToString() + XPos.ToString() + YPos.ToString());
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
