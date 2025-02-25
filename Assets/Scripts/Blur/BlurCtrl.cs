using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurCtrl : MonoBehaviour
{
    private Material blurMat;
    public bool isBlur = false;
    [Range(0, 4)]
    public float blurSize = 0;
    [Range(-3, 3)]
    public float blurOffset = 1;
    [Range(1, 3)]
    public int blurType = 3;
  
    private Material GetBlurMat(int bType)
    {
        if (bType == 1)
        {
            return new Material(Shader.Find("Hidden/AzhaoBoxBlur"));
        }
        else if (bType == 2)
        {
            return new Material(Shader.Find("Hidden/AzhaoGaussianBlur"));
        }
        else if (bType == 3)
        {
            return new Material(Shader.Find("Hidden/AzhaoKawaseBlur"));
        }
        else
        {
            return null;
        }
    }

    private void ReleaseRT(RenderTexture rt)
    {
        if (rt != null)
        {
            RenderTexture.ReleaseTemporary(rt);
        }
    }

    private bool CheckNeedCreateBlurMat(Material mat, int bType)
    {
        if (mat == null)
        {
            return true;
        }
        if (mat.shader == null)
        {
            return true;
        }
        if (bType == 1)
        {
            if (mat.shader.name != "Hidden/AzhaoBoxBlur")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (bType == 2)
        {
            if (mat.shader.name != "Hidden/AzhaoGaussianBlur")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (bType == 3)
        {
            if (mat.shader.name != "Hidden/AzhaoKawaseBlur")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private void BlurFun(RenderTexture source, RenderTexture destination, float blurTime, int bType, float offset)
    {
        if (CheckNeedCreateBlurMat(blurMat, bType) == true)
        {
            blurMat = GetBlurMat(bType);
        }
        if (blurMat == null || blurMat.shader == null || blurMat.shader.isSupported == false)
        {
            return;
        }
        blurMat.SetFloat("_BlurOffset", offset);
        float width = source.width;
        float height = source.height;
        int w = Mathf.FloorToInt(width);
        int h = Mathf.FloorToInt(height);
        RenderTexture rt1 = RenderTexture.GetTemporary(w, h);
        RenderTexture rt2 = RenderTexture.GetTemporary(w, h);
        Graphics.Blit(source, rt1);
//降采样
        for (int i = 0; i < blurTime; i++)
        {
            ReleaseRT(rt2);
            width = width / 2;
            height = height / 2;
            w = Mathf.FloorToInt(width);
            h = Mathf.FloorToInt(height);
            rt2 = RenderTexture.GetTemporary(w, h);
            Graphics.Blit(rt1, rt2, blurMat, 0);
            width = width / 2;
            height = height / 2;
            w = Mathf.FloorToInt(width);
            h = Mathf.FloorToInt(height);
            ReleaseRT(rt1);
            rt1 = RenderTexture.GetTemporary(w, h);
            Graphics.Blit(rt2, rt1, blurMat, 1);
        }
//升采样
        for (int i = 0; i < blurTime; i++)
        {
            ReleaseRT(rt2);
            width = width * 2;
            height = height * 2;
            w = Mathf.FloorToInt(width);
            h = Mathf.FloorToInt(height);
            rt2 = RenderTexture.GetTemporary(w, h);
            Graphics.Blit(rt1, rt2, blurMat, 0);
            width = width * 2;
            height = height * 2;
            w = Mathf.FloorToInt(width);
            h = Mathf.FloorToInt(height);
            ReleaseRT(rt1);
            rt1 = RenderTexture.GetTemporary(w, h);
            Graphics.Blit(rt2, rt1, blurMat, 1);
        }
        Graphics.Blit(rt1, destination);
        ReleaseRT(rt1);
        rt1 = null;
        ReleaseRT(rt2);
        rt2 = null;
        return;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (isBlur == true)
        {
            if (blurSize > 0)
            {
                BlurFun(source, source, blurSize, blurType, blurOffset);
            }
            Graphics.Blit(source, destination);

        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
