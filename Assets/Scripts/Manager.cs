using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour 
{
    [Header("Sub Managers")]
    public ResourceManager subResource;
    public InterfaceManager subInterface;
    public Camera subCamera;

    private Cave caveGenerator;

    [Header("Camera Settings")]
    public float cameraSpeed = 2.5f;

    [Header("Cave Requirements")]
    public FieldCell caveCell;      // Prefab

    [Header("Cave Settings")]
    public int caveWidth;
    public int caveLength;
    public int foodRate;
    public int ammuRate;
    public int antagonistRate;

    // Keeping track of all instances in the scene
    private Hashtable source = new Hashtable();

    // Getters
    public Cave CaveGenerator
    {
        get { return caveGenerator; }
    }

    // Management methods, Extension for the generic collections
    public List<T> SourceList <T> ()
    {
        return (List<T>)this.source[typeof(T)];
    }
    public void Dispose <T> (T obj)
        where T : Component
    {
        if (this.source.ContainsKey(typeof(T)))
        {
            // Retrieve list
            List<T> list = (List<T>)this.source[typeof(T)];
            // Check if this list contains the specific object
            if (list.Contains(obj))
            {
                // Remove object from source
                list.Remove(obj);
                // Destroy
                Destroy(obj.gameObject);
            }
        }
    }

    private void Start()
    {
        // Creating a container for the cave
        Transform container = (new GameObject("_Cave Container")).transform;
        container.SetParent(this.transform); // Append it to the game manager

        // Selecting a theme for our cave
        GroundTheme[] themes = this.subResource.FindResources<GroundTheme>().ToArray();
        GroundTheme theme = themes[UnityEngine.Random.Range(0, themes.Length)];

        // Instantiating the cave generator
        this.caveGenerator = new Cave(this.caveWidth, this.caveLength, 100, this.caveCell, container, theme);

        // Generate field and set skin
        this.caveGenerator.GenerateRectangularField(true);
        this.caveGenerator.Skin();

        // Prepare a protagonist
        foreach (Protagonist p in this.Prepare<Protagonist>(1, FieldCellType.Inner))
            p.GameManager = this;
        // Prepare food objects
        foreach (Food f in this.Prepare<Food>(this.foodRate, FieldCellType.Inner))
            f.Cell = this.caveGenerator.FindCell(new IntVector2 (f.transform.position));
        // Prepare ammu boxes
        foreach (Ammu a in this.Prepare<Ammu>(this.ammuRate, FieldCellType.Inner))
            a.Cell = this.caveGenerator.FindCell(new IntVector2(a.transform.position));
        // Prepare antagonists
        foreach (Antagonist a in this.Prepare<Antagonist>(this.antagonistRate, FieldCellType.Inner))
            a.GameManager = this;

        // Adjust the camera position
        this.MoveCamera(this.SourceList<Protagonist>()[0].Position.ToVector2());
    }

    private List<T> Prepare <T> (int rate, FieldCellType division)
        where T : MonoBehaviour
    {
        // Hold our type
        Type type = typeof(T);
        // Check if the wanted number is less than what the division provides
        if (rate >= this.caveGenerator.FindDivision(division).Count)
            throw new Exception("Catched Bug: " + type.ToString() + " number can't outnumber the division cells number!");
        // Create a source list for these objects
        this.source.Add(type, new List<T>());
        // Create a container
        Transform container = (new GameObject("_" + type.ToString() + " Container")).transform;
        container.SetParent(this.transform);
        // Retrieve resource list from the resource manager
        List<T> prefab = this.subResource.FindResources<T>();
        // Check if there are prefabs
        if (prefab.Count == 0) return null;
        // Instantiating operation
        this.source[type] = this.caveGenerator.Instantiate<T>(division, prefab, rate);
        // Append those instantiated objects to the container
        foreach (T item in this.SourceList<T>())
            item.transform.SetParent(container);
        // Return the list in case if u wanna add some additional functionalities
        return this.SourceList<T>();
    }

    // All instances must take a move after when the player takes a move
    public void Resume()
    {
        // Give antagonists permission to chase the protagonist
        foreach (Antagonist ant in SourceList<Antagonist>())
            if (ant.gameObject.activeInHierarchy)
                ant.Chase(this.SourceList<Protagonist>()[0]);
    }

    // A method for moving the camera
    public void MoveCamera(Vector2 newPosition)
    {
        // Stop all working coroutines
        StopAllCoroutines();
        // Start a new one
        this.StartCoroutine(this.IMoveCamera(newPosition, this.cameraSpeed));
    }

    // Coroutine for moving a camera
    private IEnumerator IMoveCamera(Vector2 position, float speed)
    {
        Vector3 oldPosition = this.subCamera.transform.position;
        Vector3 newPosition = (Vector3)position + Vector3.forward * oldPosition.z;
        speed *= Time.deltaTime;
        float distance;
        do
        {
            // Update old position
            oldPosition = this.subCamera.transform.position;
            // Update actual position
            this.subCamera.transform.position = Vector3.Lerp(oldPosition, newPosition, speed);
            // Update distance
            distance = Vector3.Distance(newPosition, this.subCamera.transform.position);

            yield return new WaitForEndOfFrame();
        }
        while ( !Mathf.Approximately(distance, 0) );
    }
}
