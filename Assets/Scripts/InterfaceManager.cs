using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager 
    : MonoBehaviour 
{

	private Canvas currentInterface;

    [Header("Protagonist Controls")]
    public ProgressBar healthBar;
    public ProgressBar hungerBar;
    public Text ammuText;

    // A method that instantiates a canvas
    private Canvas NewCanvas(string name)
    {
        // Instantiate a new gameObject
        GameObject g = new GameObject(name);
        // Append it to this manager
        g.transform.SetParent(this.transform);
        // The canvas component
        Canvas c = g.AddComponent<Canvas>();
        // Configure
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        // Add other components that a canvas must have
        g.AddComponent<CanvasScaler>();
        g.AddComponent<GraphicRaycaster>();
        
        return c;
    }

}
