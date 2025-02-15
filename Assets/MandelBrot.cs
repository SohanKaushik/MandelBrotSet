using System.Numerics;
using Unity.Collections;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    // -> Formula:  Z(n + 1) = Z^2n + c

    [SerializeField] ComputeShader _shader;
    private ComputeBuffer _buffer;

    [SerializeField] int MAX_ITERATIONS = 100;
    [SerializeField] int _zoom;

    [SerializeField] UnityEngine.Vector2 _offset;

    [Header("Texture")]
    [SerializeField] int _width = 100;
    [SerializeField] int _height = 100;

    private RenderTexture _texture;
    private int kernel;

    void Awake()
    {

        _texture = new RenderTexture(_width, _height, 0);
        _texture.enableRandomWrite = true;
        _texture.Create();

        _shader.FindKernel("CSMain");
        _buffer = new ComputeBuffer(_width * _height, sizeof(int));
        _shader.SetInt("MAX_ITERATIONS", MAX_ITERATIONS);
        _shader.SetInt("_Width", _width);
        _shader.SetInt("_Height", _height);


        _shader.SetTexture(0, "result", _texture);
        GetComponent<Renderer>().material.mainTexture = _texture;

    }
    private void Update()
    {
        _shader.SetFloat("_Zoom", _zoom);
        _shader.SetVector("_Offset", _offset);
        _shader.Dispatch(0, Mathf.CeilToInt(_width / 8.0f), Mathf.CeilToInt(_height / 8.0f), 1);
    }

    private void OnDisable()
    {
        _buffer.Release();
        _buffer = null;
    }
}
