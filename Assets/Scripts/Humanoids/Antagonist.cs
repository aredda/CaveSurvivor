using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antagonist 
    : Humanoid
{
    [Header("Antagonist Fatality")]
    public int damage = 10;

    public virtual void Chase(Humanoid target)
    {
        if (!this.isMoving) // Only if he is not moving
        {
            if (target == null)
                throw new Exception("Catched Bug: No target to pursue!");

            FieldCell s = this.GameManager.CaveGenerator.FindCell(this.Position);
            FieldCell e = this.GameManager.CaveGenerator.FindCell(target.Position);
            List<FieldCell> w = this.gameManager.CaveGenerator.FindWalkableCells();

            // Find path
            List<FieldCell> path = PathFinder.FindShortestPath(w, s, e);

            if (path == null)
                return;

            // Find direction
            IntVector2 d = path[1].Position - s.Position;
            // Convert direction
            int dIndex = DirectionGuide.getDirectionIndex(d);
            if (dIndex == -1)
                return;
            // Means the antagonist is only one step closer to the protagonist
            if (path.Count == 2)
            {
                if ( DirectionGuide.getDirection(this.faceDirection) + this.Position == target.Position )
                    // Harm the player
                    target.TakeDamage(this.damage);
                else
                    // Just face the target for now
                    StartCoroutine(base.IFace(DirectionGuide.getRotation((Direction)dIndex, faceDirection)));
            }
            // Move
            base.Move((Direction)dIndex);
        }
    }
}
