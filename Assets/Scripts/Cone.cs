using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone : MonoBehaviour
{
    public GameObject coneParent;
    void Start()
    {
        UpdateChildren(coneParent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material originalMaterial = renderer.material;
                Material newMaterial = new Material(originalMaterial);
                newMaterial.color = new Color(Random.value, Random.value, Random.value);
                newMaterial.shader = Shader.Find("Unlit/Color");
                renderer.material = newMaterial;
            }
        }
    }
}
