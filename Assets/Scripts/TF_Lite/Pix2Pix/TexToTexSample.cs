using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using UnityEngine.EventSystems;

public class TexToTexSample : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")] string modelName;
    [SerializeField] RawImage preview = null;
    [SerializeField] ComputeShader compute = null;
    [SerializeField] RawImage cameraView = null;

    WebCamTexture webcamTexture;
    TexToTex textotex;

    void Start ()
    {
#if UNITY_EDITOR
        WebCamDevice[] devices = WebCamTexture.devices;
        string cameraName = devices[1].name;
#else
        string cameraName = WebCamUtil.FindName();
#endif
        webcamTexture = new WebCamTexture(cameraName, 1024, 1024, 30);
        webcamTexture.Play();
        cameraView.texture = webcamTexture;

        string modelPath = Path.Combine(Application.streamingAssetsPath, modelName);
        textotex = new TexToTex(modelPath, compute);
    }

    void OnDestroy ()
    {
        textotex?.Dispose();
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (textotex != null)
            {
                textotex.Invoke(webcamTexture);
                preview.texture = textotex.GetResultTexture();
            }
        }
    }

    private void LateUpdate ()
    {
        
    }
}
