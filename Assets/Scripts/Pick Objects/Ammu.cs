﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammu 
    : Pickable
{
    [Header("Benefits")]
    public int ammunition;

    public override void Use(Protagonist trigger)
    {
        if (!this.isWrapped)
        // If it's unwrapped, then the protagonist can take the exposed item
        {
            // Consume
            trigger.Profit(0, 0, this.ammunition);
            // The moving objects can consider this cell as walkable
            trigger.GameManager.CaveGenerator.MarkCellAsWalkable(this.cell);
            // Consume and dispsoe
            trigger.GameManager.Dispose(this);      // Dispose of this object right away
        }
    }
}
