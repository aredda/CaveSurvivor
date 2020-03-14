using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTheme : MonoBehaviour 
{
    [Header("Sprite References")]
    public Sprite Default;
    public Sprite[] Main = new Sprite[4];
    public Sprite[] Corner = new Sprite[4];

    // Get a main sprite
    public Sprite getMain(Direction d)
    {
        return this.Main[(int)d];
    }
    // Get a corner sprite
    public Sprite getCorner(Direction d1, Direction d2)
    {
        switch (d1)
        {
            case Direction.Up:
                if(d2 == Direction.Right)
                    return this.Corner[0];
                else if(d2 == Direction.Left)
                    return this.Corner[3];
                break;
            case Direction.Down:
                if(d2 == Direction.Right)
                    return this.Corner[1];
                else if (d2 == Direction.Left)
                    return this.Corner[2];
                break;
            case Direction.Right:
                if (d2 == Direction.Up)
                    return this.Corner[0];
                else if (d2 == Direction.Down)
                    return this.Corner[1];
                break;
            case Direction.Left:
                if (d2 == Direction.Up)
                    return this.Corner[3];
                else if (d2 == Direction.Down)
                    return this.Corner[2];
                break;
        }
        return this.Default;
    }
    // Get main 
    public Sprite getDefault()
    {
        return this.Default;
    }
}
