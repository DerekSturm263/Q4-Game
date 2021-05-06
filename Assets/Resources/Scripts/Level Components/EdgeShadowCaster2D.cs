using UnityEngine;
using System.Reflection;
using UnityEngine.Experimental.Rendering.Universal;

[ExecuteInEditMode]

public class EdgeShadowCaster2D : MonoBehaviour
{
    private EdgeCollider2D col;
    private ShadowCaster2D shadowCaster;

    private FieldInfo meshField;
    private FieldInfo shapePathField;
    private MethodInfo generateShadowMeshMethod;

    private void Awake()
    {
        try
        {
            meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
            shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);

            generateShadowMeshMethod = typeof(ShadowCaster2D)
                                        .Assembly
                                        .GetType("UnityEngine.Experimental.Rendering.Universal.ShadowUtility")
                                        .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

            shadowCaster = gameObject.AddComponent<ShadowCaster2D>();
            col = gameObject.GetComponent<EdgeCollider2D>();

            Vector3[] points = Reduce(FromVector2(col.points), 4);

            shapePathField.SetValue(shadowCaster, points);
            meshField.SetValue(shadowCaster, new Mesh());
            generateShadowMeshMethod.Invoke(shadowCaster, new object[] { meshField.GetValue(shadowCaster), shapePathField.GetValue(shadowCaster) });

            DestroyImmediate(this);
        }
        catch
        {
            Debug.LogError("Shadow Caster Could Not Be Added");
        }
    }

    private static Vector3[] FromVector2(Vector2[] ogPoints)
    {
        Vector3[] newPoints = new Vector3[ogPoints.Length];

        for (int i = 0; i < ogPoints.Length; ++i)
        {
            newPoints[i] = ogPoints[i];
        }

        return newPoints;
    }

    private static Vector3[] Reduce(Vector3[] ogPoints, int step)
    {
        Vector3[] newPoints = new Vector3[ogPoints.Length / step];

        for (int i = 0, j = 0; j < newPoints.Length; i += step, ++j)
        {
            newPoints[j] = ogPoints[i];
        }

        return newPoints;
    }
}
