using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Text
{
    public abstract class TMPTextEffect : ScriptableObject
    {
        public abstract bool ModifyTextMesh(TMPro.TMP_TextInfo textInfo, List<Vector3> allVertices, float deltaTime, float time);
    }
}
