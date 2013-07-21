using UnityEngine;
using System;

public class MasterMaterialAssigner : MonoBehaviour 
{
	public bool DebugMode = false;
    public Material masterMaterial;

    public void Start()
    {
        NormalizeMaterial();
    }

    [ContextMenu("Set child object materials to Master Material")]
    public void NormalizeMaterial()
    {
        if(masterMaterial == null)
            throw new Exception("Must specify a master material to use!");

        // Set up the array of renderer references...
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        // Replace the materials array on each renderer with an array
        // consisting of only the master material.
        Material[] replacement = new Material[1] { masterMaterial };
        foreach(MeshRenderer current in renderers)
        {
			if(DebugMode)
            	Debug.Log ("Current renderer: " + current);
			
            current.materials = replacement;
        }

        Debug.Log("Assignment successful.");
    }
}
