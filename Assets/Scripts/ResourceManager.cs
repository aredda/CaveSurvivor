using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager 
    : MonoBehaviour 
{
    [Header ("Resource List")]
    public List<Resource> resources;

    [Header("Ready Resources List")]
    public List<Resource> preMade;

    private Dictionary<string, GameObject> ready;

    void Start()
    {
        this.ready = new Dictionary<string, GameObject>();

        GameObject g;
        foreach (Resource r in this.preMade)
        {
            g = Instantiate(r.prefab, this.transform);

            this.ready.Add(r.identity, g);
        }
    }

    public T FindResource <T> (string identity)
        where T : MonoBehaviour
    {
        foreach (Resource item in this.resources)
            if (item.identity.Equals(identity))
                return item.prefab.GetComponent<T>();
        return null;
    }
    public List<T> FindResources <T> ()
        where T : MonoBehaviour
    {
        List<T> output = new List<T>(); // Declaring the output

        foreach (Resource r in this.resources)
        {
            T cmp = r.prefab.GetComponent<T>(); // Attempt to retrieve that component
            // If there is a component of that type in our resource then add it to the list
            if (cmp != null)
                output.Add(cmp);
        }

        return output;                  // Returning the output
    }
    public T GetPreMadeResource <T> (string identity)
        where T : Component
    {
        return this.ready[identity].GetComponent<T>();
    }

    public static void PlayVFX(ParticleSystem particle, Vector2 position)
    {
        particle.transform.position = position;
        particle.Play();
    }
}
