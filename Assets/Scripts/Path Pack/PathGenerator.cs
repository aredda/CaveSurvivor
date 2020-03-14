using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class PathGenerator
{
    // Input
    protected int fieldWidth, fieldLength;
    protected int fieldFillRate;
    protected FieldCell fieldCell;
    protected Transform fieldContainer;

    // Output
    protected List<FieldCell> cells = new List<FieldCell>();
    protected Dictionary<FieldCellType, List<FieldCell>> divisions;
    protected List<FieldCell> walkable = new List<FieldCell>();

    // Constructors
    public PathGenerator(int width, int length, int fillRate, FieldCell cellPrefab)
    {
        this.fieldWidth = width;
        this.fieldLength = length;
        this.fieldFillRate = fillRate;
        this.fieldCell = cellPrefab;
    }
    public PathGenerator(int width, int length, int fillRate, FieldCell cellPrefab, Transform parent)
        : this(width, length, fillRate, cellPrefab)
    {
        this.fieldContainer = parent;
    }

    #region Main Methods

    // Random cell generating
    public void Generate()
    {
        // Calculate the number of cells
        int cellCount = (int)((this.fieldWidth * this.fieldLength) * ((double)this.fieldFillRate / 100));
        int cellCounter = 0;
        // Starting point
        int x = Rnd.Range(0, this.fieldWidth);
        int y = Rnd.Range(0, this.fieldLength);
        IntVector2 p = new IntVector2(x, y);
        // Create the first cell
        createCell(p);
        // Increment the cell counter
        cellCounter++;
        // Generating operation
        IntVector2 d = IntVector2.Zero;
        List<IntVector2> chosenPosition = new List<IntVector2>() { p };
        while (cellCounter < cellCount)
        {
            // Clone the directions list
            List<IntVector2> dirs = DirectionGuide.Directions;
            // Search for the available direction
            do
            {
                // Reset direction
                d = IntVector2.Zero;
                // Ceck if the list got empty
                if (dirs.Count == 0)
                    break;
                // Get a random direction
                d = DirectionGuide.randomDirection();
                // Remove from the cloned list
                dirs.Remove(d);
            }
            // If this position is occupied or out of bounds, keep searching for another direction
            while(chosenPosition.Contains(p + d));
            // If we haven't found any available direction
            // Let us just find an available position
            if (d.Equals(IntVector2.Zero))
            {
                do
                {
                    // Find an available position
                    x = Rnd.Range(0, this.fieldWidth);
                    y = Rnd.Range(0, this.fieldLength);

                    p = new IntVector2(x, y);
                }
                while (chosenPosition.Contains(p));
            }
            else
            {
                // Create cell
                createCell(p + d);
                // Update progress position
                p += d;
                // Save this position
                chosenPosition.Add(p);
                // Increment counter
                cellCounter++;
            }
        }
        // Setting edges
        setEdges();
        // Cell Division
        divideCells();
    }
    // Clear field
    public void Dispose()
    {
        if (this.cells.Count == 0)
            return;

        foreach (FieldCell fc in this.cells)
            GameObject.Destroy(fc.gameObject);

        this.cells.Clear();
    }
    // Assign cell's adjacent cells
    protected void setEdges()
    {
        foreach (FieldCell fc in this.cells)
            for (int i = 0; i < DirectionGuide.Directions.Count; i++)
                fc.Edges[i] = FindCell(fc.Position + DirectionGuide.Directions[i]);
    }
    // Shortcut for creating a cell
    protected FieldCell createCell(IntVector2 p)
    {
        // Instantiate
        FieldCell fc = GameObject.Instantiate(this.fieldCell, p.ToVector2(), Quaternion.identity) as FieldCell;
        fc.Position = p;
        // Rename the cell
        fc.name = "Cell X:(" + p.x + ") Y:(" + p.y + ")";
        // Append it to the parent
        fc.transform.SetParent(this.fieldContainer);
        // Save it to the cell list
        this.cells.Add(fc);
        // Mark it as walkable
        this.MarkCellAsWalkable(fc);
        // Return 
        return fc;
    }
    // Rank cells
    protected void divideCells()
    {
        // Get division count
        int divisionCount = Enum.GetValues(typeof(FieldCellType)).Length;
        // Prepare 
        this.divisions = new Dictionary<FieldCellType, List<FieldCell>>();
        for (int i = 0; i < divisionCount; i++)
            this.divisions.Add((FieldCellType)i, new List<FieldCell>());
        // Divide
        for (int i = 0; i < divisionCount; i++)
        {
            // Loop through cells
            foreach (FieldCell c in this.cells)
            {
                bool belongs = false;
                // Check if this cell belongs to previous divisions
                for (int k = i - 1; k >= 0; k--)
                    belongs |= this.divisions[(FieldCellType)k].Contains(c);
                // If it belongs then don't waste time and move on to the next cell
                if (belongs) continue;
                // Loop through edges
                foreach (FieldCell e in c.Edges)
                {
                    // Reset checker
                    belongs = false;
                    // Previous division
                    int j = i - 1;
                    // Check if that edge belongs to previous division if and only if there is a previous division
                    if (j >= 0)
                        belongs = this.divisions[(FieldCellType)(j)].Contains(e);
                    // Final settings
                    // "e == null" in case of a null edge, there is no need to consider 'belongs' cuz
                    //      it's definetly an outer cell 
                    if (belongs || e == null)
                    {
                        // Send it to its division
                        this.divisions[(FieldCellType)i].Add(c);
                        // Change cell type
                        c.Type = (FieldCellType)i;
                    }
                }
            }
        }
        // The core division is the last field cell type
        // However, there will be some remaining cells without marking, all we have to do
        // is to get the remaining cells and assign them as the core division
        foreach (FieldCell fc in this.cells)
        {
            bool belongs = false;
            // Check if this cell belongs to some division
            foreach ( KeyValuePair<FieldCellType, List<FieldCell>> kvp in this.divisions )
                if (kvp.Value.Contains(fc))
                    belongs = true;
            // If it is without a dvision then
            if (!belongs)
            {
                // Add it to the core
                this.divisions[(FieldCellType)(this.divisions.Count - 1)].Add(fc);
                // Update fc's type
                fc.Type = (FieldCellType)(this.divisions.Count - 1);
            }
        }

        // Conditions:
        // Outer    :   At least having a null edge

        // Middle   :   Not being an outer
        //                  [ cell == BelongsTo (i-1) division ]
        //              At least having an outer cell edge

        // Inner    :   Not being an outer nor a middle
        //                  [ cell == BelongsTo (i-1) division ]
        //                  Or [ cell == BelongsTo (i-2) division ]
        //              At least having a middle cell or an inner cell
    }

    #endregion

    #region Secondary Methods
    // Colorize cells based on their type (TESTIN METHOD)
    public void Colorize(FieldCellType cellType, Color color)
    {
        if (this.divisions.Count == 0)
                throw new Exception("There are no divisions");

        foreach (FieldCell cell in this.divisions[cellType])
            cell.SetSpriteRendererColor(color);
    }
    public void Colorize(List<Color> colors)
    {
        List<int> chosen = new List<int>();

        foreach (KeyValuePair<FieldCellType, List<FieldCell>> kvp in this.divisions)
        {
            int colorIndex;
            do
            {
                colorIndex = UnityEngine.Random.Range(0, colors.Count);
                chosen.Add(colorIndex);
            }
            while (chosen.Contains(colorIndex));
            // Colorize
            this.Colorize(kvp.Key, colors[colorIndex]);
        }
    }
    // Give the cell some components
    public void AddComponent(FieldCellType cellType, Type component)
    {
        foreach (FieldCell fc in this.divisions[cellType])
            if (fc.gameObject.GetComponent (component) == null)
                fc.gameObject.AddComponent(component);
    }
    public void AddComponent(List<FieldCellType> cellTypes, Type component)
    {
        foreach (FieldCellType type in cellTypes)
            AddComponent(type, component);
    }
    #endregion

    #region Additional Generating Methods

    // Generate rectangular field
    public void GenerateRectangularField(bool walkablity)
    {
        for (int x = 0; x < this.fieldWidth; x++)
            for (int y = 0; y < this.fieldLength; y++)
                this.MarkCellAsWalkable(createCell(new IntVector2(x, y)));

        // Assign edges
        this.setEdges();
        // Make divisions
        this.divideCells();
    }
    // Improve field methods
    public void ShredField(int shredRate)
    {
        // Retrieve the outer cells
        List<FieldCell> outers = this.divisions[FieldCellType.Outer];

        int shredCount = (outers.Count * shredRate) / 100;
        int counter = 0;

        FieldCell victim;
        while (counter < shredCount)
        {
            int rnd = UnityEngine.Random.Range(0, outers.Count);

            if (rnd >= outers.Count)
                break;

            victim = outers[rnd];
            // Remove from the main list & the division  list
            this.cells.Remove(victim);
            outers.RemoveAt(rnd);
            // Destroy cell
            MonoBehaviour.Destroy(victim.gameObject);
            // Update field edges & redivide cells
            this.setEdges();
            this.divideCells();

            counter++;
        }
    }

    #endregion

    #region Searching Methods

    // Find Cell
    public FieldCell FindCell(IntVector2 p)
    {
        foreach (FieldCell fc in this.cells)
            if (fc.Position.Equals(p))
                return fc;
        return null;
    }
    // Retrieve the list of walkable cells
    public void MarkCellAsWalkable(FieldCell cell)
    {
        if (!this.walkable.Contains(cell))
            this.walkable.Add(cell);
        cell.IsWalkable = true;
    }
    public void MarkCellAsUnwalkable(FieldCell cell)
    {
        if (this.walkable.Contains(cell))
            this.walkable.Remove(cell);
        cell.IsWalkable = false;
    }
    public List<FieldCell> FindWalkableCells()
    {
        // Return
        return walkable;
    }
    public List<FieldCell> FindWalkableCells(FieldCellType division)
    {
        // Output
        List<FieldCell> filter = new List<FieldCell>();
        // Loop through
        foreach (FieldCell fc in this.walkable)
            if (fc.Type == division)
                filter.Add(fc);
        // Return
        return filter;
    }
    // Retrieving the list of cells which belong to a specific division
    public List<FieldCell> FindDivision(FieldCellType divison)
    {
        if (!this.divisions.ContainsKey(divison))
            throw new Exception("Catched Error: This divison does not exists!");

        return this.divisions[divison];
    }

    #endregion
}

public enum FieldCellType
{
    /*
     WARNING:
     * You shoud put in consideration the order of your layers
     * from the extreme outer to the core
     * that's how you should edit your layers
     */
    Outer = 0, Middle, Inner
}
