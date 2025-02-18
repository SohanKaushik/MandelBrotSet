using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Fractal : MonoBehaviour
{
    // -> Formula:  Z(n + 1) = Z^2n + c

    [SerializeField] ComputeShader _shader;
    private ComputeBuffer _buffer;

    public int MAX_ITERATIONS = 100;
    [SerializeField] float _zoom;
    [SerializeField] float _speed;

    [SerializeField] UnityEngine.Vector2 _offset;

    [Header("Texture")]
    [SerializeField] int _width = 100;
    [SerializeField] int _height = 100;
    [SerializeField] TextMeshProUGUI _text;
    private RenderTexture _texture;
    private int kernel;

    private int tile_x, tile_y;

    void Awake()
    {

        _texture = new RenderTexture(_width, _height, 0);
        _texture.enableRandomWrite = true;
        _texture.Create();


        kernel = _shader.FindKernel("CSMain");
        _buffer = new ComputeBuffer(_width * _height, sizeof(int));
        _shader.SetInt("_Width", _width);
        _shader.SetInt("_Height", _height);


        _shader.SetTexture(0, "result", _texture);
        GetComponent<Renderer>().material.mainTexture = _texture;
    }

    private void Update()
    {

        _shader.SetFloat("_Zoom", (float)_zoom);
        _shader.SetVector("_Offset", _offset);
        _shader.SetInt("MAX_ITERATIONS", MAX_ITERATIONS);

        _shader.Dispatch(0, Mathf.CeilToInt(_width /16.0f), Mathf.CeilToInt(_height / 16.0f), 1);

                                                             
        if (Input.GetKey(KeyCode.A)) { _offset.x -= _speed / _zoom * Time.deltaTime; }
        if (Input.GetKey(KeyCode.D)) { _offset.x += _speed / _zoom * Time.deltaTime; }
        if (Input.GetKey(KeyCode.W)) { _offset.y += _speed / _zoom * Time.deltaTime; }
        if (Input.GetKey(KeyCode.S)) { _offset.y -= _speed / _zoom * Time.deltaTime; }

        if ((Input.GetKey(KeyCode.E) || Input.GetMouseButton(0)) && _zoom < 200000) 
                          { _zoom += _speed * _zoom * Time.deltaTime; if (MAX_ITERATIONS < 2000) MAX_ITERATIONS += 2; }
        if ((Input.GetKey(KeyCode.Q) || Input.GetMouseButton(1)) && _zoom > 0.35) { _zoom -= _speed * _zoom * Time.deltaTime; MAX_ITERATIONS -= 2; }

        if (Input.GetKey(KeyCode.Space))
        {
            MAX_ITERATIONS = 100;
            _zoom = 1;
            _offset = new Vector2(-0.5f, 0);
        }

        _text.text = ((int)_zoom).ToString() + "x";  
    }
    private void OnDisable()
    {
        _buffer.Dispose();
    }
}
