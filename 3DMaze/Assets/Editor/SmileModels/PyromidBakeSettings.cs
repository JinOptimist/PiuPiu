using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PyrommidBakeSettings", menuName = "Smile/Pyromid")]
public class PyromidBakeSettings : ScriptableObject
{
    [Tooltip("Mesh smile")]
    public Mesh sourceMesh;
    public int sourceSubMeshIndex;
    public Vector3 scale = new Vector3(1, 1, 1);
    public Vector3 rotation;
    public float pyromidHeight = 1;
}
