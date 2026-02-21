using System;
using PicMotion.Core;
using UnityEngine;
using Unity.Barracuda;

namespace PicMotion.Inference
{
    /// <summary>
    /// Unity Barracuda を使用した RTMPose-Wholebody のポーズエスティメータ。
    /// ONNXモデルをロードし、前処理済み画像からランドマーク座標を推定する。
    /// </summary>
    public class BarracudaRtmPoseEstimator : IPoseEstimator
    {
        private readonly Model _model;
        private readonly IWorker _worker;

        /// <summary>
        /// NNModel（.onnx をインポートしたアセット）から推論エンジンを初期化する。
        /// GPU ComputeShader が利用可能な場合は GPU、不可なら CPU にフォールバック。
        /// </summary>
        public BarracudaRtmPoseEstimator(NNModel modelAsset)
        {
            if (modelAsset == null)
                throw new ArgumentNullException(nameof(modelAsset),
                    "ONNXモデルアセットが指定されていません。");

            _model = ModelLoader.Load(modelAsset);
            var workerType = SelectWorkerType();
            _worker = WorkerFactory.CreateWorker(workerType, _model);
        }

        public PoseLandmark[] Estimate(PreprocessedImage image)
        {
            // Barracuda Tensor: (batch=1, height, width, channels=3)
            using var inputTensor = new Tensor(1, image.Height, image.Width, image.Channels, image.PixelData);

            _worker.Execute(inputTensor);

            // 出力テンソルの取得
            var outputTensor = _worker.PeekOutput();
            if (outputTensor == null)
                return null;

            var landmarks = ParseOutput(outputTensor, image.Width, image.Height);
            outputTensor.Dispose();

            return landmarks;
        }

        public void Dispose()
        {
            _worker?.Dispose();
        }

        // ── Private メソッド ──

        private WorkerFactory.Type SelectWorkerType()
        {
            if (SystemInfo.supportsComputeShaders)
            {
                Debug.Log("[PicMotion] GPU ComputePrecompiled バックエンドを使用します。");
                return WorkerFactory.Type.ComputePrecompiled;
            }

            Debug.LogWarning("[PicMotion] GPU非対応のためCPUバックエンドにフォールバックします。");
            return WorkerFactory.Type.CSharpBurst;
        }

        /// <summary>
        /// モデル出力テンソルからランドマーク座標を解析する。
        /// RTMPoseの一般的な出力形式: (1, NumKeypoints, 3) = (x, y, score)
        /// ※モデルにより形式が異なる場合はここを調整する。
        /// </summary>
        private PoseLandmark[] ParseOutput(
            Tensor output, int inputWidth, int inputHeight)
        {
            // Barracuda Tensor の形状を確認して解析方法を決定
            int totalElements = output.length;
            int numKeypoints;
            int stride;

            // 出力形状パターン: (1, N, 3) or (1, N*3) etc.
            if (output.shape.width > 1 && output.shape.channels >= 2)
            {
                // (1, H=1, W=NumKeypoints, C=3) パターン
                numKeypoints = output.shape.width;
                stride = output.shape.channels;
            }
            else
            {
                // フラットな出力: 3要素ずつ (x, y, score)
                stride = 3;
                numKeypoints = totalElements / stride;
            }

            numKeypoints = Mathf.Min(numKeypoints, LandmarkIndex.TotalCount);
            var landmarks = new PoseLandmark[numKeypoints];

            for (int i = 0; i < numKeypoints; i++)
            {
                float x = output[i * stride + 0] / inputWidth;
                float y = output[i * stride + 1] / inputHeight;
                float score = output[i * stride + 2];

                landmarks[i] = new PoseLandmark(
                    new Vector2(x, y),
                    depth: 0f,
                    score: score
                );
            }

            return landmarks;
        }
    }
}
