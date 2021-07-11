using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using Cysharp.Threading.Tasks;
using static TensorFlowLite.PoseNet;
using System;

public class PoseNetSample : MonoBehaviour
{
    [Header("Body")]
    [SerializeField, FilePopup("*.tflite")] string fileName = "posenet_mobilenet_v1_100_257x257_multi_kpt_stripped.tflite";
    [SerializeField] RawImage cameraView = null;
    [SerializeField, Range(0f, 1f)] float threshold = 0.5f;
    [SerializeField, Range(0f, 1f)] float lineThickness = 0.01f;
    [SerializeField] bool runBackground;
    [SerializeField] PosToObj[] posToObj;

    [Header("Face")]
    [SerializeField, FilePopup("*.tflite")] string faceModelFile = "face_detection_back";
    [SerializeField, FilePopup("*.tflite")] string faceMeshModelFile = "face_landmark";
    [SerializeField] bool useLandmarkToDetection = true;
    [SerializeField] Material faceMaterial = null;

    

    WebCamTexture webcamTexture;
    PoseNet poseNet;
    Vector3[] corners = new Vector3[4];
    PrimitiveDraw draw;
    UniTask<bool> task;
    PoseNet.Result[] results;
    CancellationToken cancellationToken;

    FaceDetect faceDetect;
    FaceMesh faceMesh;
    Vector3[] rtCorners = new Vector3[4];
    MeshFilter faceMeshFilter;
    //MeshFilter faceMeshFilter2;
    Vector3[] faceKeypoints;
    FaceDetect.Result detection;
    GameObject go = null;
    //GameObject go2 = null;

    TextureResizer resizer;
    TextureResizer.ResizeOptions resizeOptions;



    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        poseNet = new PoseNet(path);

        // Init camera

#if UNITY_EDITOR
        WebCamDevice[] devices = WebCamTexture.devices;
        string cameraName = devices[1].name;
#else
        string cameraName = WebCamUtil.FindName();
#endif
        webcamTexture = new WebCamTexture(cameraName, 1280, 720, 30);
        webcamTexture.Play();
        cameraView.texture = webcamTexture;
        resizer = new TextureResizer();
        resizeOptions = new TextureResizer.ResizeOptions()
        {
            aspectMode = TextureResizer.AspectMode.Fill,
            rotationDegree = 0,
            mirrorHorizontal = false,
            mirrorVertical = false,
            width = 720,
            height = 720,
        };

        draw = new PrimitiveDraw()
        {
            color = Color.green,
        };

        cancellationToken = this.GetCancellationTokenOnDestroy();

        string detectionPath = Path.Combine(Application.streamingAssetsPath, faceModelFile);
        faceDetect = new FaceDetect(detectionPath);

        string faceMeshPath = Path.Combine(Application.streamingAssetsPath, faceMeshModelFile);
        faceMesh = new FaceMesh(faceMeshPath);

        // Create Face Mesh Renderer
        {
            go = new GameObject("Face");
            go.transform.SetParent(transform);
            var faceRenderer = go.AddComponent<MeshRenderer>();
            faceRenderer.material = faceMaterial;

            faceMeshFilter = go.AddComponent<MeshFilter>();
            faceMeshFilter.sharedMesh = FaceMeshBuilder.CreateMesh();

            faceKeypoints = new Vector3[FaceMesh.KEYPOINT_COUNT];
        }
        //{
        //    go2 = new GameObject("Face");
        //    go2.transform.SetParent(cameraView.transform.GetChild(0));
        //    var faceRenderer = go2.AddComponent<MeshRenderer>();
        //    faceRenderer.material = faceMaterial;

        //    faceMeshFilter2 = go2.AddComponent<MeshFilter>();
        //    faceMeshFilter2.sharedMesh = FaceMeshBuilder.CreateMesh();
        //}
    }


    void OnDestroy()
    {
        webcamTexture?.Stop();
        poseNet?.Dispose();
        faceDetect?.Dispose();
        faceMesh?.Dispose();
        draw?.Dispose();
    }

    void Update()
    {
        //cameraView.texture = resizer.Resize(webcamTexture, resizeOptions);
        //cameraView.texture = webcamTexture;
        //cameraView.material = resizer.material;


        if (runBackground)
        {
            if (task.Status.IsCompleted())
            {
                task = InvokeAsync();
            }
        }
        else
        {
            poseNet.Invoke(webcamTexture);
            results = poseNet.GetResults();
            cameraView.material = poseNet.transformMat;
        }

        if (results != null)
        {
            DrawResult();
        }

        if (detection == null || !useLandmarkToDetection)
        {
            faceDetect.Invoke(poseNet.inputTex);
            //cameraView.material = faceDetect.transformMat;
            detection = faceDetect.GetResults().FirstOrDefault();

            if (detection == null)
            {
                return;
            }
        }

        faceMesh.Invoke(poseNet.inputTex, detection);
        var meshResult = faceMesh.GetResult();

        if (meshResult.score < 0.5f)
        {
            detection = null;
            return;
        }

        DrawResults(detection, meshResult);

        if (useLandmarkToDetection)
        {
            detection = faceMesh.LandmarkToDetection(meshResult);
        }
    }

    void DrawResults (FaceDetect.Result detection, FaceMesh.Result face)
    {
        cameraView.rectTransform.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        //// Draw Face Detection
        //{
        //    draw.color = Color.blue;
        //    Rect rect = MathTF.Lerp(min, max, detection.rect, true);
        //    draw.Rect(rect, 0.05f);
        //    foreach (Vector2 p in detection.keypoints)
        //    {
        //        draw.Point(MathTF.Lerp(min, max, new Vector3(p.x, 1f - p.y, 0)), 0.1f);
        //    }
        //}
        //draw.Apply();

        // Draw face
        //draw.color = Color.green;
        float zScale = (max.x - min.x) / 2;
        for (int i = 0; i < face.keypoints.Length; i++)
        {
            Vector3 p = MathTF.Lerp(min, max, face.keypoints[i]);
            p.z = face.keypoints[i].z * zScale;
            faceKeypoints[i] = p;
            //draw.Point(p, 0.05f);
        }
        //draw.Apply();

        // Update Mesh
        FaceMeshBuilder.UpdateMesh(go, faceMeshFilter.sharedMesh, faceKeypoints);
        //FaceMeshBuilder.UpdateMesh(go2, faceMeshFilter2.sharedMesh, faceKeypoints);
    }

    void DrawResult()
    {
        var rect = cameraView.GetComponent<RectTransform>();
        rect.GetWorldCorners(corners);
        Vector3 min = corners[0];
        Vector3 max = corners[2];

        var connections = PoseNet.Connections;
        int len = connections.GetLength(0);
        for(int i = 0; i < posToObj.Length; i++)
        {
            posToObj[i].obj.SetActive(false);
        }

        for (int i = 0; i < len; i++)
        {
            var a = results[(int)connections[i, 0]];
            var b = results[(int)connections[i, 1]];


            for (int j = 0; j < posToObj.Length; j++)
            {
                if (posToObj[j].part == b.part)
                {
                    posToObj[j].obj.SetActive(true);
                    posToObj[j].obj.name = b.part.ToString();
                    posToObj[j].obj.transform.localPosition = MathTF.Lerp(min, max, new Vector3(b.x, 1f - b.y, 0));
                }
            }

            if (a.confidence >= threshold && b.confidence >= threshold)
            {                
                draw.Line3D(
                    MathTF.Lerp(min, max, new Vector3(a.x, 1f - a.y, 0)),
                    MathTF.Lerp(min, max, new Vector3(b.x, 1f - b.y, 0)),
                    lineThickness
                );
            }
        }

        draw.Apply();
    }

    async UniTask<bool> InvokeAsync()
    {
        results = await poseNet.InvokeAsync(webcamTexture, cancellationToken);
        cameraView.material = poseNet.transformMat;
        return true;
    }

    [Serializable]
    struct PosToObj
    {
        public Part part;
        public GameObject obj;
    }
}
