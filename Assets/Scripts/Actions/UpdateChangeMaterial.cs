using UnityEngine;

/// <summary>
/// This script makes it easier to toggle between a new material, and the objects original material.
/// </summary>
public class UpdateChangeMaterial : MonoBehaviour
{
    [Tooltip("The material that's switched to.")]
    public Material otherMaterial = null;

    private bool usingOther = false;
    private MeshRenderer meshRenderer = null;
    private Material originalMaterial = null;

    private Material[] materials;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
        originalMaterial = materials[1];
    }

    public void SetOtherMaterial()
    {
        usingOther = true;
        materials[1] = otherMaterial;
        meshRenderer.materials = materials;
    }

    public void SetOriginalMaterial()
    {
        usingOther = false;
        materials[1] = originalMaterial;
        meshRenderer.materials = materials;
    }

    public void ToggleMaterial()
    {
        usingOther = !usingOther;

        if (usingOther)
        {
            materials[1] = otherMaterial;
            meshRenderer.materials = materials;
        }
        else
        {
            materials[1] = originalMaterial;
            meshRenderer.materials = materials;
        }
    }
}
