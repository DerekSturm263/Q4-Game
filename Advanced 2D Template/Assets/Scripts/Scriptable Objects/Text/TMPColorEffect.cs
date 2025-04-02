using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Text
{
    [CreateAssetMenu(fileName = "New Color Text Effect", menuName = "TextMeshPro/Text Effects/Color Text Effect")]
    public class TMPColorEffect : TMPTextEffect
    {
        [SerializeField] private Gradient _gradient;

        [SerializeField] private float _speed = 1;
        [SerializeField] private float _spacing = 0.1f;

        [SerializeField] private bool _ignoreR = true;
        [SerializeField] private bool _ignoreG = true;
        [SerializeField] private bool _ignoreB = true;

        public override bool ModifyTextMesh(TMPro.TMP_TextInfo textInfo, List<Vector3> allVertices, float deltaTime, float time)
        {
            int j = 0;
            TMPro.TMP_MeshInfo meshInfo = textInfo.meshInfo[j];

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMPro.TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (charInfo.character.CompareTo(' ') == 0)
                    continue;

                Color32 color = _gradient.Evaluate((time * _speed) - (i * _spacing));

                int bottomLeft = charInfo.vertexIndex;
                int topLeft = charInfo.vertexIndex + 1;
                int topRight = charInfo.vertexIndex + 2;
                int bottomRight = charInfo.vertexIndex + 3;

                if (bottomLeft >= meshInfo.colors32.Length)
                    meshInfo = textInfo.meshInfo[++j];

                if (_ignoreR)
                    color.r = meshInfo.colors32[bottomLeft].r;
                if (_ignoreG)
                    color.g = meshInfo.colors32[bottomLeft].g;
                if (_ignoreB)
                    color.b = meshInfo.colors32[bottomLeft].b;

                meshInfo.colors32[bottomLeft] = color;
                meshInfo.colors32[topLeft] = color;

                meshInfo.colors32[topRight] = color;
                meshInfo.colors32[bottomRight] = color;
            }

            return meshInfo.colors32[^5] == _gradient.Evaluate(1);
        }
    }
}
