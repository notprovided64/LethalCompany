using UnityEngine;
using UnityEngine.UI;

namespace DirectionalIndicators;

public class TrigHelpers
{
    public static float CalculateAzimuthAngle(Vector3 playerForward, Vector3 soundDirection)
    {
        soundDirection = new Vector3(soundDirection.x, 0, soundDirection.z).normalized;
        float angle = Vector3.SignedAngle(soundDirection, playerForward, Vector3.up);

        return angle;
    }

    public static Vector2 GetVector(float angleDegrees)
    {
        float magnitude = 118f;

        float angleRadians = (angleDegrees + 90) * Mathf.Deg2Rad;

        // Calculate x and y components using trigonometry
        float x = magnitude * Mathf.Cos(angleRadians);
        float y = magnitude * Mathf.Sin(angleRadians);

        return new Vector2(x, y);
    }
}

public class CircleDrawer : MonoBehaviour
{
    public static void DrawCircle(RectTransform rectTransform, Color color, float radius)
    {
        // Create a square texture
        int size = Mathf.Max((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        Texture2D circleTexture = new Texture2D(size, size);

        // Calculate the center of the texture
        Vector2 center = new Vector2(size / 2f, size / 2f);

        // Fill the texture with the circle
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 point = new Vector2(x, y) - center;
                if (point.magnitude <= radius)
                {
                    circleTexture.SetPixel(x, y, color);
                }
                else
                {
                    circleTexture.SetPixel(x, y, Color.clear);
                }
            }
        }
        circleTexture.Apply();

        // Create the circle game object and sprite
        GameObject circleGameObject = new GameObject("Circle", typeof(Image));
        RectTransform circleRectTransform = circleGameObject.GetComponent<RectTransform>();
        Image circleImage = circleGameObject.GetComponent<Image>();
        circleRectTransform.SetParent(rectTransform, false);
        circleRectTransform.localPosition = Vector3.zero;
        circleImage.sprite = Sprite.Create(circleTexture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    public static void DrawCenterCircle(RectTransform rectTransform)
    {
        float radius = 400;
        // Create a square texture
        int size = Mathf.Max((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        Texture2D circleTexture = new Texture2D(size, size, TextureFormat.ARGB32, false);

        // Calculate the center of the texture
        Vector2 center = new Vector2(size / 2f, size / 2f);

        Color color = Color.Lerp(Color.clear, Color.red, 0.3f);

        // Fill the texture with the circle
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 point = new Vector2(x, y) - center;
                if ((point.magnitude <= radius) && (point.magnitude > 0.9*radius))
                {
                    circleTexture.SetPixel(x, y, color);
                }
                else
                {
                    circleTexture.SetPixel(x, y, Color.clear);
                }
            }
        }
        circleTexture.Apply();

        // Create the circle game object and sprite
        GameObject circleGameObject = new GameObject("Circle", typeof(Image));
        RectTransform circleRectTransform = circleGameObject.GetComponent<RectTransform>();
        Image circleImage = circleGameObject.GetComponent<Image>();
        circleGameObject.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        circleRectTransform.SetParent(rectTransform, false);
        circleRectTransform.localPosition = Vector3.zero;
        circleImage.sprite = Sprite.Create(circleTexture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}