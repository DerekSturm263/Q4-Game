using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Text
{
    [CreateAssetMenu(fileName = "New Position Text Effect", menuName = "TextMeshPro/Text Effects/Position Text Effect")]
    public class TMPPositionEffect : TMPTextEffect
    {
        [SerializeField] private AnimationCurve _curve;

        [SerializeField] private float _strength = 1;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _length = 1;
        [SerializeField] private float _spacing = 0.1f;

        public override bool ModifyTextMesh(TMPro.TMP_TextInfo textInfo, List<Vector3> allVertices, float deltaTime, float time)
        {
            if (allVertices.Count == 0)
                return true;

            int j = 0;
            TMPro.TMP_MeshInfo meshInfo = textInfo.meshInfo[j];

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMPro.TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (charInfo.character.CompareTo(' ') == 0)
                    continue;

                float y = _curve.Evaluate(Mathf.Repeat((time + (i * _spacing)) * _speed, _length)) * _strength;

                int bottomLeft = charInfo.vertexIndex;
                int topLeft = charInfo.vertexIndex + 1;
                int topRight = charInfo.vertexIndex + 2;
                int bottomRight = charInfo.vertexIndex + 3;

                if (bottomLeft >= meshInfo.colors32.Length)
                    meshInfo = textInfo.meshInfo[++j];

                meshInfo.vertices[bottomLeft] = allVertices[bottomLeft] + new Vector3(0, y);
                meshInfo.vertices[topLeft] = allVertices[topLeft] + new Vector3(0, y);

                meshInfo.vertices[topRight] = allVertices[topRight] + new Vector3(0, y);
                meshInfo.vertices[bottomRight] = allVertices[bottomRight] + new Vector3(0, y);
            }

            return false;
        }
    }
}
