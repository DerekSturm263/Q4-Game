using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class UnityExtensionMethods
    {
        public static Vector2Int TopLeft(this RectInt rect) => new(rect.min.x, rect.size.y);
        public static Vector2Int TopRight(this RectInt rect) => rect.size;
        public static Vector2Int BottomRight(this RectInt rect) => new(rect.size.x, rect.min.y);
        public static Vector2Int BottomLeft(this RectInt rect) => rect.min;

        public static bool IsEmptyOrNull<T>(this ICollection<T> list) => list.All(item => item is null) || list.Count == 0;

        public static Vector2 SnapTo8Slices(this Vector2 vector2)
        {
            Vector2 dir = Vector2.zero;

            if (Vector2.Dot(vector2.normalized, Vector2.up) > 0.5f)
                dir += Vector2.up;
            if (Vector2.Dot(vector2.normalized, Vector2.down) > 0.5f)
                dir += Vector2.down;
            if (Vector2.Dot(vector2.normalized, Vector2.left) > 0.5f)
                dir += Vector2.left;
            if (Vector2.Dot(vector2.normalized, Vector2.right) > 0.5f)
                dir += Vector2.right;

            return dir.normalized;
        }

        public static Vector2 SnapTo4Slices(this Vector2 vector2)
        {
            Vector2 dir = Vector2.zero;

            if (Vector2.Dot(vector2.normalized, Vector2.up) > 0.5f)
                dir = Vector2.up;
            else if (Vector2.Dot(vector2.normalized, Vector2.down) > 0.5f)
                dir = Vector2.down;
            else if (Vector2.Dot(vector2.normalized, Vector2.left) > 0.5f)
                dir = Vector2.left;
            else if (Vector2.Dot(vector2.normalized, Vector2.right) > 0.5f)
                dir = Vector2.right;

            return dir;
        }

        public static T MapTo4Slices<T>(this Vector2 vector2, T north, T east, T south, T west)
        {
            T ret = default;

            if (Vector2.Dot(vector2.normalized, Vector2.up) > 0.5f)
                ret = north;
            else if (Vector2.Dot(vector2.normalized, Vector2.down) > 0.5f)
                ret = south;
            else if (Vector2.Dot(vector2.normalized, Vector2.left) > 0.5f)
                ret = east;
            else if (Vector2.Dot(vector2.normalized, Vector2.right) > 0.5f)
                ret = west;

            return ret;
        }

        public static bool Intersects(this BoundsInt lhs, BoundsInt rhs)
        {
            Bounds lhsNoInt = new(lhs.position, lhs.size);
            Bounds rhsNoInt = new(rhs.position, rhs.size);

            return lhsNoInt.Intersects(rhsNoInt);
        }

        public static bool ContainsBounds(this BoundsInt lhs, BoundsInt rhs)
        {
            return rhs.position.x > lhs.position.x + 1 &&
                   rhs.position.y > lhs.position.y + 1 &&
                   rhs.position.x + rhs.size.x < lhs.position.x + lhs.size.x - 2 &&
                   rhs.position.y + rhs.size.y < lhs.position.y + lhs.size.y - 2;
        }

        public static int FrameCount(this AnimationClip animClip) => (int)(animClip.length * animClip.frameRate);

        public static GameObject FindChildWithTag(this GameObject gameObject, string tag, bool includeInactive)
        {
            foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(includeInactive))
            {
                if (transform.CompareTag(tag))
                    return transform.gameObject;
            }

            return null;
        }

        [System.Flags]
        public enum Direction
        {
            Horizontal = 1 << 0,
            Vertical = 1 << 1
        }

        public static void SetSizeAuto(this RectTransform rectTransform, Direction direction, Vector2 padding = default, Vector2 extraPadding = default, bool useMaxParent = false)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            RectTransform parentTransform = rectTransform.parent.GetComponent<RectTransform>();

            if (direction.HasFlag(Direction.Horizontal))
            {
                float biggestX = 0;
                float width = 0;

                for (int i = rectTransform.childCount - 1; i >= 0; --i)
                {
                    RectTransform rect = rectTransform.GetChild(i).GetComponent<RectTransform>();
                    if (!rect.gameObject.activeSelf)
                        continue;

                    float newX = Mathf.Abs(rect.anchoredPosition.x);
                    if (newX > biggestX)
                    {
                        biggestX = newX;
                        width = rect.sizeDelta.x;

                        break;
                    }
                }

                float newSize = biggestX - width / 2 + width;

                if (useMaxParent)
                    sizeDelta.x = Mathf.Max(newSize + padding.x, parentTransform.rect.width);
                else
                    sizeDelta.x = newSize + padding.x;
            }

            if (direction.HasFlag(Direction.Vertical))
            {
                float biggestY = 0;
                float height = 0;

                for (int i = rectTransform.childCount - 1; i >= 0; --i)
                {
                    RectTransform rect = rectTransform.GetChild(i).GetComponent<RectTransform>();
                    if (!rect.gameObject.activeSelf)
                        continue;

                    float newY = Mathf.Abs(rect.anchoredPosition.y);
                    if (newY > biggestY)
                    {
                        biggestY = newY;
                        height = rect.sizeDelta.y;

                        break;
                    }
                }

                float newSize = biggestY - height / 2 + height;

                if (useMaxParent)
                    sizeDelta.y = Mathf.Max(newSize + padding.y, parentTransform.rect.height);
                else
                    sizeDelta.y = newSize + padding.y;
            }

            rectTransform.sizeDelta = sizeDelta + extraPadding;
            return;
        }

        public enum ImageType
        {
            PNG,
            JPG,
            EXR,
            TGA
        }

        public static Texture2D RenderToTexture2D(this Camera camera, RenderTexture output, TextureFormat textureFormat, bool linear, bool flipX = false, Shader shader = null, string replacementTag = "")
        {
            camera.targetTexture = output;

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = camera.targetTexture;

            if (shader)
                camera.RenderWithShader(shader, replacementTag);
            else
                camera.Render();

            if (flipX)
            {
                RenderTexture temp = RenderTexture.GetTemporary(RenderTexture.active.descriptor);

                Graphics.Blit(RenderTexture.active, temp, new Vector2(-1, 1), new Vector2(1, 0));
                Graphics.Blit(temp, RenderTexture.active);

                RenderTexture.ReleaseTemporary(temp);
            }

            Texture2D image = new(camera.targetTexture.width, camera.targetTexture.height, textureFormat, false, linear);
            image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            image.Apply();

            RenderTexture.active = currentRT;
            camera.targetTexture = null;

            return image;
        }

        public static void RenderToScreenshot(this Camera camera, string filePath, RenderTexture output, ImageType type, TextureFormat textureFormat, bool linear, bool flipX = false, Shader shader = null, string replacementTag = "")
        {
            Texture2D texture = RenderToTexture2D(camera, output, textureFormat, linear, flipX, shader, replacementTag);

            byte[] renderBytes = type switch
            {
                ImageType.PNG => texture.EncodeToPNG(),
                ImageType.JPG => texture.EncodeToJPG(),
                ImageType.EXR => texture.EncodeToEXR(),
                ImageType.TGA => texture.EncodeToTGA(),
                _ => null
            };

            System.IO.File.WriteAllBytes(filePath, renderBytes);
            Debug.Log("Screenshot taken!");
        }

        public static T GetEnabledComponent<T>(this GameObject go) where T : MonoBehaviour
        {
            return go.GetComponents<T>().First(item => item.enabled);
        }

        public static bool TryGetEnabledComponent<T>(this GameObject go, out T component) where T : MonoBehaviour
        {
            var components = go.GetComponents<T>().Where(item => item.enabled);
            component = components.FirstOrDefault();

            return components.Count() > 0;
        }

        public static T GetEnabledComponent<T>(this Transform go) where T : MonoBehaviour
        {
            return go.gameObject.GetEnabledComponent<T>();
        }

        public static bool TryGetEnabledComponent<T>(this Transform go, out T component) where T : MonoBehaviour
        {
            return go.gameObject.TryGetEnabledComponent(out component);
        }
    }
}
