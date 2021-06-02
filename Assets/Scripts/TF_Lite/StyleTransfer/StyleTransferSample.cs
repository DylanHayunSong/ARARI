using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;

public class StyleTransferSample : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")] string predictionFileName = "style_predict_quantized_256.tflite";
    [SerializeField, FilePopup("*.tflite")] string transferFileName = "style_transfer_quantized_dynamic.tflite";
    [SerializeField] Texture2D styleImage = null;
    [SerializeField] RawImage preview = null;
    [SerializeField] RawImage preview2 = null;
    [SerializeField] ComputeShader compute = null;

    WebCamTexture webcamTexture;
    StyleTransfer styleTransfer;
    float[] styleBottleneck;
    bool test = false;
    int i = 0;

    void Start ()
    {
        // Predict style bottleneck;
        string predictionModelPath = Path.Combine(Application.streamingAssetsPath, predictionFileName);
        using (var predict = new StylePredict(predictionModelPath))
        {
            predict.Invoke(styleImage);
            styleBottleneck = predict.GetStyleBottleneck();
        }

        string transferModelPath = Path.Combine(Application.streamingAssetsPath, transferFileName);
        styleTransfer = new StyleTransfer(transferModelPath, styleBottleneck, compute);

        // Init camera
        WebCamDevice[] devices = WebCamTexture.devices;
        string cameraName = devices[1].name;
        //string cameraName = WebCamUtil.FindName();
        webcamTexture = new WebCamTexture(cameraName, 640, 480, 30);
        webcamTexture.Play();
        preview.texture = webcamTexture;
        preview.transform.eulerAngles = new Vector3(0, 0, 90);
    }

    void OnDestroy ()
    {
        styleTransfer?.Dispose();
    }

    void Update ()
    {
        //RenderTexture tests =  styleTransfer.GetResultTexture();
        //if (Input.GetMouseButtonDown(0) && test == false)
        //{
        //    Trans();
        //    test = true;
        //    preview.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        //}

        //if (Input.GetMouseButtonUp(0) && test == true)
        //{
        //    preview.texture = webcamTexture;
        //    test = false;
        //    preview.gameObject.transform.eulerAngles = new Vector3(0, 0, -90f);
        //}

        //preview.texture = styleTransfer.GetResultTexture();

        if (Input.GetMouseButton(0))
        {
            Trans();
        }

        // styleTransfer.Invoke(sampleTexture);


        // preview.uvRect = TextureToTensor.GetUVRect(
        //     (float)webcamTexture.width / (float)webcamTexture.height,
        //     1,
        //     TextureToTensor.AspectMode.Fill);
    }

    void Trans ()
    {
        styleTransfer.Invoke(webcamTexture);
        preview2.texture = styleTransfer.GetResultTexture();
    }
}
