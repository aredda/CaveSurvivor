using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionGuide 
{
    // Directions reference
    public static IntVector2[] directions = 
    {
        IntVector2.Up,  // Up
        IntVector2.Right,  // Right
        IntVector2.Down, // Down
        IntVector2.Left  // Left        
    };
    public static List<IntVector2> Directions
    {
        get 
        {
            // Convert to list
            List<IntVector2> list = new List<IntVector2>();            
            foreach (IntVector2 v in directions)
                list.Add(v);

            return list; 
        }
    }
    // Rotations
    public static float[] rotations =
    {
        90,     // Face Up
        0,      // Face Right
        270,    // Face Down
        180     // Face Left
    };
    // Get direction using the Direction enum
    public static IntVector2 getDirection(Direction dir)
    {
        return directions[(int)dir];
    }
    public static int getDirectionIndex(IntVector2 dir)
    {
        for (int i = 0; i < directions.Length; i++)
            if (directions[i].Equals(dir))
                return i;
        return -1;
    }
    // Get the rotation angle
    public static float getRotation(Direction dir)
    {
        return rotations[(int)dir];
    }
    public static float getRotation(Direction toFace, Direction current)
    {
        int i = (int)current;
        int j = (int)toFace;

        if (current == Direction.Down && toFace == Direction.Right
            || current == Direction.Right && toFace == Direction.Up)
            return rotations[i] + 90;
        if (current == Direction.Right && toFace == Direction.Down)
            return rotations[i] - 90;

        return rotations[j];
    }
    // Get a random direction
    public static IntVector2 randomDirection()
    {
        return directions[UnityEngine.Random.Range(0, directions.Length)];
    }
    // Get directions in form of enums
    public static Direction[] EnumDirections 
    {
        get {
            return Enum.GetValues(typeof(Direction)) as Direction[];
        }
    }
}

public enum Direction
{
    Up = 0, 
    Right, 
    Down, 
    Left
}
