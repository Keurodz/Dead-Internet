using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DitherShaderScript : MonoBehaviour
{
    public Shader ditherShader;

    [Range(0.0f, 1.0f)]
    public float spread = 0.5f;

    [Range(2, 16)]
    public int redColorCount = 2;
    [Range(2, 16)]
    public int greenColorCount = 2;
    [Range(2, 16)]
    public int blueColorCount = 2;

    [Range(1, 8)]
    public int downSamples = 2;

    private Material ditherMat;


    void OnEnable()
    {
        ditherMat = new Material(ditherShader);
        ditherMat.hideFlags = HideFlags.HideAndDontSave;
    }

    void OnDisable()
    {
        ditherMat = null;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        ditherMat.SetFloat("_Spread", spread);
        ditherMat.SetInt("_RedColorCount", redColorCount);
        ditherMat.SetInt("_GreenColorCount", greenColorCount);
        ditherMat.SetInt("_BlueColorCount", blueColorCount);

        int width = source.width;
        int height = source.height;


        RenderTexture currentSource = source;

        RenderTexture dither = RenderTexture.GetTemporary(
            (width / downSamples),
            (height / downSamples), 0, source.format);
        Graphics.Blit(currentSource, dither, ditherMat, 0);

        Graphics.Blit(dither, destination, ditherMat, 1);
        RenderTexture.ReleaseTemporary(dither);


    }
}