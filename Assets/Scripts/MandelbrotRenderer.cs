using System;
using UnityEngine;

public class MandelbrotRenderer : MonoBehaviour
{
    public ComputeShader computeShader; 
    private RenderTexture renderTexture; // Will store the output
    public int textureSize = 512;       // Resolution of the texture
    
    public Vector2 variableC;
    public float zoom = 1.0f;           // Zoom level
    public Vector2 offset = Vector2.zero; // Offset in the complex plane
    public int maxIterations = 50;    // Max iterations for Mandelbrot
    public float escapeRadius = 2.0f;  // Escape radius for Mandelbrot
    public bool movingAnimation;
    public float animationSpeed = 0.0f;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 75;
    }
    void Start()
    {
        InitializeRenderTexture();
        variableC = new Vector2(-1.3f, 0.00525f);
        animationSpeed = 0.001f;
    }

    void Update()
    {
        ReadInput();
        if (movingAnimation)
        {
            UpdateAnimation();
        }

        // Set shader parameters
        computeShader.SetTexture(0, "Result", renderTexture); // Bind texture
        computeShader.SetVector("resolution", new Vector2(textureSize, textureSize));
        computeShader.SetFloat("zoom", zoom);
        computeShader.SetVector("offset", offset);
        computeShader.SetInt("maxIterations", maxIterations);
        computeShader.SetFloat("escapeRadius", escapeRadius);
        computeShader.SetVector("secondTerm", variableC);

        // Calculate thread groups (match texture size)
        int threadGroupsX = Mathf.CeilToInt(textureSize / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(textureSize / 8.0f);

        // Dispatch the compute shader
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
    }

    void InitializeRenderTexture()
    {
        if (renderTexture != null) renderTexture.Release(); // Release existing texture

        renderTexture = new RenderTexture(textureSize, textureSize, 0);
        renderTexture.enableRandomWrite = true; // Enable random write for compute shaders
        renderTexture.Create();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Blit the compute shader output to the screen
        Graphics.Blit(renderTexture, dest);
    }

    void UpdateAnimation()
    {
        variableC.x =  1*Mathf.Sin(Time.time * animationSpeed);
        variableC.y =  1*Mathf.Cos(Time.time * animationSpeed);
    }

    void ReadInput()
    {
        float speed = 0.1f / zoom;
        // Arrow keys for movement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("Up Arrow Pressed");
            offset += Vector2.up * speed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("Down Arrow Pressed");
            offset += Vector2.down * speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("Left Arrow Pressed");
            offset += Vector2.left * speed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("Right Arrow Pressed");
            offset += Vector2.right * speed;
        }

        // Space key zooming
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Space Key Pressed");
            zoom+=1*animationSpeed;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Debug.Log("Space Key Pressed");
            if (zoom>1) {zoom=1;}
        }

        // Keys "U", "J", "I", and "K"
        if (Input.GetKey(KeyCode.U))
        {
            Debug.Log("Key U Pressed");
            variableC.x += 0.001f;
        }
        if (Input.GetKey(KeyCode.J))
        {
            Debug.Log("Key J Pressed");
            variableC.x -= 0.001f;
        }
        if (Input.GetKey(KeyCode.I))
        {
            Debug.Log("Key I Pressed");
            variableC.y += 0.001f;
        }
        if (Input.GetKey(KeyCode.K))
        {
            Debug.Log("Key K Pressed");
            variableC.y -= 0.001f;
        }
        
        if (Input.GetKey(KeyCode.R))
        {
            Debug.Log("Key K Pressed");
            variableC = new Vector2(0.0f, 0.0f);
            offset = new Vector2(0.0f, 0.0f);
            zoom = 0;
        }
    }
}