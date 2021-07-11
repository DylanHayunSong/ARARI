using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TensorFlowLite
{
    public class TexToTex : BaseImagePredictor<float>
    {
        public float[,,] output0; // height, width, 2

        RenderTexture outputTex;
        ComputeShader compute;
        ComputeBuffer outputBuffer;

        public TexToTex(string modelPath, ComputeShader compute) : base(modelPath, false)
        {
            this.compute = compute;

            output0 = new float[height, width, channels];

            outputTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
            outputTex.enableRandomWrite = true;
            outputTex.Create();

            outputBuffer = new ComputeBuffer(width * height, sizeof(float) * 3);

            Debug.Log(compute);
        }
        public override void Dispose ()
        {
            base.Dispose();
            if (outputTex != null)
            {
                outputTex.Release();
                Object.Destroy(outputTex);
            }
            outputBuffer?.Dispose();
        }

        public override void Invoke (Texture inputTex)
        {
            ToTensor(inputTex, input0);

            interpreter.SetInputTensorData(0, input0);
            interpreter.Invoke();
            interpreter.GetOutputTensorData(0, output0);
        }

        public RenderTexture GetResultTexture ()
        {
            outputBuffer.SetData(output0);
            compute.SetBuffer(0, "InputTensor", outputBuffer);
            compute.SetTexture(0, "OutputTexture", outputTex);

            compute.Dispatch(0, width / 8, height / 8, 1);
            return outputTex;
        }
    }
}