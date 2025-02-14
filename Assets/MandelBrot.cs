using System.Numerics;
using Unity.Collections;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    // -> Formula:  Z(n + 1) = Z^2n + c

    [SerializeField] int MAX_ITERATIONS = 100;
    [SerializeField] int _zoom;

    [SerializeField] UnityEngine.Vector2 _offset;

    [Header("Texture")]
    [SerializeField] int _width = 100;
    [SerializeField] int _height = 100;

    private Texture2D _texture;

    void Awake()
    {
        _texture = new Texture2D(_width, _height);
        _texture.filterMode = FilterMode.Bilinear;
        GetComponent<Renderer>().material.mainTexture = _texture;
    }

    void Start() {

        // -> Pixel Space Coordinate to Complex Plane
        float aspect = (float)_width / (float)_height;

        float xMin = (float)(-2.0 / _zoom) + _offset.x;
        float xMax = (float)(+2.0 / _zoom) + _offset.x;
        float yRange = 2.0f / _zoom;
        float yMin = (float)(yRange / aspect) + _offset.y;
        float yMax = (float)(-yRange / aspect) + _offset.y;

        // MandelBrot Set
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                float a = xMin + (xMax - xMin) * x / _width;
                float b = yMin + (yMax - yMin) * y / _height;

                int iteration = MandelBrot(a, b, MAX_ITERATIONS);
                _texture.SetPixel(x, y, GetColor(iteration));
            }
        }
        _texture.Apply();
    }

    int MandelBrot(float a, float b, int MAX_ITERATIONS)
    {
        // -> where z = 0
        int iteration = 0;
        float zx = 0, zy = 0;

        while (zx * zx + zy * zy < 4 && iteration < MAX_ITERATIONS)
        {
            float temp = zx * zx - zy * zy + a;
            zy = 2 * zx * zy + b;
            zx = temp;

            iteration++;
        }
        return iteration;
    }

    Color GetColor(int iteration)
    {
        if (iteration == MAX_ITERATIONS) return Color.black;
        float t = iteration / MAX_ITERATIONS;
        return Color.Lerp(Color.white, Color.red, t);
    }
}
