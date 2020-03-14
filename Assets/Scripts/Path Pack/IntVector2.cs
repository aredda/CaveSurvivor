using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntVector2
{
    public int x, y;

    public readonly static IntVector2 Zero = new IntVector2(0, 0);
    public readonly static IntVector2 Up = new IntVector2(0, 1);
    public readonly static IntVector2 Right = new IntVector2(1, 0);
    public readonly static IntVector2 Down = new IntVector2(0, -1);
    public readonly static IntVector2 Left = new IntVector2(-1, 0);

    // Constructor
    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public IntVector2(Vector2 v)
    {
        this.x = (int)v.x;
        this.y = (int)v.y;
    }

    public override string ToString()
    {
        return "(" + this.x + ", " + this.y + ")";
    }

    // Converto to vector2
    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    // Addition Operator
    public static IntVector2 operator + (IntVector2 a, IntVector2 b)
    {
        a.x += b.x;
        a.y += b.y;

        return a;
    } // IntVector2 + IntVector2
    public static Vector2 operator + (IntVector2 a, Vector2 b)
    {
        b.x += a.x;
        b.y += a.y;

        return b;
    } // Vector2 + IntVector2

    // Subsract Operator
    public static IntVector2 operator - (IntVector2 a, IntVector2 b)
    {
        a.x -= b.x;
        a.y -= b.y;

        return a;
    }

    // Compare
    public static bool operator == (IntVector2 a, IntVector2 b)
    {
        return (a.x == b.x) && (a.y == b.y);
    }
    public static bool operator != (IntVector2 a, IntVector2 b)
    {
        return (a.x != b.x) || (a.y != b.y);
    }
    public override bool Equals(object obj)
    {
        IntVector2 other = (IntVector2)obj;
        return (other.x == this.x) && (other.y == this.y);
    }
}
