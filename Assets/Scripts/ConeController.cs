using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeController : MonoBehaviour
{
    // Start is called before the first frame update
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
                renderer.material = newMaterial;
            }
        }
    }
}
