using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    // Djikistra's Way
    public static List<FieldCell> FindShortestPath(List<FieldCell> walkableCells, FieldCell start, FieldCell end)
    {
        if (start == null   || 
            end == null     || 
            walkableCells == null)
            return null;
        // Avoiding game lags and glitches
        // If the target is right in font of us, there is no need to undergo a search operation
        if (Vector2.Distance(start.Position.ToVector2(), end.Position.ToVector2()) <= 1)
            return new List<FieldCell>() { start, end };
        // Unvisited & Path & Visited list, Distance & CameForm dictionaries
        List<FieldCell> unvisited = new List<FieldCell>(),
                        path = new List<FieldCell>(),
                        visited = new List<FieldCell>();
        Dictionary<FieldCell, float> distance = new Dictionary<FieldCell, float>();
        Dictionary<FieldCell, FieldCell> cameFrom = new Dictionary<FieldCell, FieldCell>();
        // + Start & End Cells are not considered as walkableCells so we had
        // to configure them manually
        unvisited.Add(start);
        unvisited.Add(end);  
        distance.Add(start, 0);
        distance.Add(end, Mathf.Infinity);
        cameFrom.Add(start, null);
        cameFrom.Add(end, null);
        // Setup lists
        foreach (FieldCell fc in walkableCells)
        {
            // Add it to the unvisited
            if (!unvisited.Contains(fc))
                unvisited.Add(fc);
            // Initialize the distance list
            if (!distance.ContainsKey(fc))
                distance.Add(fc, Mathf.Infinity);
            // Initialize the cameFrom list
            if (!cameFrom.ContainsKey(fc))
                cameFrom.Add(fc, null);
        }
        // Search operation
        FieldCell current = null;
        float minDistance = Mathf.Infinity;
        
        while (unvisited.Count != 0 && unvisited.Contains(end)) // While we haven't visited all nodes do:
        {
            // Seach for the shortest distance
            foreach (FieldCell cell in unvisited)
            {
                if (distance[cell] < minDistance)
                {
                    minDistance = distance[cell];
                    current = cell;
                }
            }
            // If we didn't find a shortest path then stop the searching operation
            if (current == null)
                return null;
            // Check if that current cell has neighbors
            if (current.Edges.Length != 0)
            {
                foreach (FieldCell neighbor in current.Edges)
                {
                    // No need to visit a cell again
                    // No need to work on a cell that is not marked as walkable
                    if (neighbor == null)
                        continue;
                    if (visited.Contains(neighbor) || (!neighbor.IsWalkable && neighbor != end))
                        continue;
                    // Calculate the distance from start cell
                    float actualDistance = Vector2.Distance(current.Position.ToVector2(), neighbor.Position.ToVector2());
                    float totalDistance = distance[current] + actualDistance;
                    // Update the distance & from where we came
                    // If the distance is less than old distance
                    if (distance[neighbor] >= totalDistance)
                    {
                        distance[neighbor] = totalDistance;
                        cameFrom[neighbor] = current;
                    }
                }
            }
            // Remove the current cell from the unvisited & make it as visited
            // Reset the minDistance
            unvisited.Remove(current);
            visited.Add(current);
            current = null;
            minDistance = Mathf.Infinity;
        }
        // Back tracking
        current = end;
        while (current != null && cameFrom.ContainsKey(current))
        {
            // Add cell to path
            path.Add(current);
            // Update current cell to the cell we came from
            current = cameFrom[current];
        }
        path.Reverse(); // Reverse the path to make it start from the start cell
        // Checking
        if (path[0] != start)
            return null;
        // Final result
        return path;
    }

}
