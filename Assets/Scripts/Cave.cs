using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave
    : PathGenerator
{
    private GroundTheme caveTheme;

    // Overriding the constructor
    public Cave(int width, int length, int fillRate, FieldCell cell, Transform container, GroundTheme theme)
        : base(width, length, fillRate, cell, container)
    {
        this.caveTheme = theme;
    }

    // This one is responsible for putting the "skin" to our cave
    public void Skin()
    {
        List<Direction> emptyEdges, existedEdges;
        // Decorize the outer division
        foreach (FieldCell cell in this.divisions[FieldCellType.Outer])
        {
            // Reset array
            existedEdges = new List<Direction>();
            emptyEdges = new List<Direction>();
            // Loop through cell's edges
            foreach (Direction edgeDir in DirectionGuide.EnumDirections)
                if (cell.Edges[(int)edgeDir] == null)
                    emptyEdges.Add(edgeDir);
                else
                    existedEdges.Add(edgeDir);
            // Detect which edge
            if (emptyEdges.Count == 1)
                cell.SetSprite(this.caveTheme.getMain(emptyEdges[0]));
            else if (emptyEdges.Count == 2)
                cell.SetSprite(this.caveTheme.getCorner(emptyEdges[0], emptyEdges[1]));
            //else if (emptyEdges.Count == 3)
            //{
            //    int dInt = (int)existedEdges[0] + 2;
            //    Direction d = dInt >= DirectionGuide.EnumDirections.Length ? (Direction)(dInt - 4) : (Direction)dInt;
            //    cell.SetSprite(this.caveTheme.getMain(d));
            //}
            //else
            //    throw new Exception(cell.name + " is expected to be an OUTER CELL.");
        }
        // Decorize what's left
        foreach (FieldCell cell in this.divisions[FieldCellType.Middle])
            cell.SetSprite(this.caveTheme.getDefault());
        foreach (FieldCell cell in this.divisions[FieldCellType.Inner])
            cell.SetSprite(this.caveTheme.getDefault());
    }
    // For creating objects randomly
    public List<T> Instantiate<T> (FieldCellType division, T prefab, int number)
        where T : MonoBehaviour
    {
        if (this.divisions[division].Count == 0)
            return null;
        // Declaration
        List<T> source = new List<T>();
        int counter = 0;
        FieldCell cell;
        // Operation
        while (counter < number)
        {
            do
            {
                int index = UnityEngine.Random.Range(0, this.divisions[division].Count);
                // Get a random cell
                cell = this.divisions[division][index];
            }
            while( !cell.IsWalkable && this.FindWalkableCells(division).Count > 0 );
            // If the cell is still null after the research well we don't neet to waste time
            if (cell == null) break;
            // Instantiate
            T go = GameObject.Instantiate(prefab, cell.Position.ToVector2(), Quaternion.identity);
            // Unmark this cell as walkable
            this.MarkCellAsUnwalkable(cell);
            // Add object to the list
            source.Add(go);
            // Nullify cell
            cell = null;

            counter++;
        }
        // Return output
        return source;
    }
    public List<T> Instantiate<T>(FieldCellType division, List<T> prefabs, int number)
        where T : MonoBehaviour
    {
        List<T> output = new List<T>();
        T prefab;

        for (int i = 0; (i < prefabs.Count) && (number > 0); i++)
        {
            // Get prefab randomly
            prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Count)];

            // Quantity of this prefab
            int q = (i == prefabs.Count - 1) ? number : UnityEngine.Random.Range(0, number);

            if (prefab == null)
                throw new Exception("Incorrect prefab!");

            foreach (T ins in this.Instantiate<T>(division, prefab, q))
                output.Add(ins); // Add instance to the output list

            // Update total number
            number -= q;
        }

        return output;
    }
}
