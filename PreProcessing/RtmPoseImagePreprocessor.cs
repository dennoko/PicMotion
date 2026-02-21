using PicMotion.Core;
using UnityEngine;

namespace PicMotion.PreProcessing
{
    /// <summary>
    /// RTMPose用の画像プリプロセッサ。
    /// Texture2DをRTMPoseが要求する形式（mean/std正規化）に変換する。
    /// Barracuda向けにNHWC形式で出力する。
    /// </summary>
    public class RtmPoseImagePreprocessor : IImagePreprocessor
    {
        /// <summary> RTMPoseのデフォルト入力幅。 </summary>
        public int InputWidth { get; }

        /// <summary> RTMPoseのデフォルト入力高さ。 </summary>
        public int InputHeight { get; }

        // ImageNet由来の正規化パラメータ（RTMPose標準）
        private static readonly float[] Mean = { 123.675f, 116.28f, 103.53f };
        private static readonly float[] Std  = { 58.395f, 57.12f, 57.375f };

        public RtmPoseImagePreprocessor(int inputWidth = 192, int inputHeight = 256)
        {
            InputWidth = inputWidth;
            InputHeight = inputHeight;
        }

        public PreprocessedImage Process(Texture2D sourceImage)
        {
            var resized = ResizeTexture(sourceImage, InputWidth, InputHeight);
            var pixels = resized.GetPixels32();

            // Barracuda向け: NHWC形式で正規化されたピクセルデータを生成
            var pixelData = NormalizeToNHWC(pixels, InputWidth, InputHeight);

            Object.DestroyImmediate(resized);

            return new PreprocessedImage(pixelData, InputWidth, InputHeight, 3);
        }

        private Texture2D ResizeTexture(Texture2D source, int width, int height)
        {
            var rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(source, rt);

            var previous = RenderTexture.active;
            RenderTexture.active = rt;

            var resized = new Texture2D(width, height, TextureFormat.RGBA32, false);
            resized.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            resized.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);

            return resized;
        }

        /// <summary>
        /// Color32配列をNHWC形式のfloat配列に変換し、mean/std正規化を適用する。
        /// NHWC = (Height, Width, Channels) — Barracudaのデフォルト形式。
        /// </summary>
        private float[] NormalizeToNHWC(Color32[] pixels, int width, int height)
        {
            var data = new float[height * width * 3];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Unity のGetPixels32 は左下原点のため、Y座標を反転
                    int srcIdx = (height - 1 - y) * width + x;
                    int dstIdx = (y * width + x) * 3;

                    var pixel = pixels[srcIdx];

                    // NHWC: 各ピクセルの後にRGB値を連続配置 + mean/std正規化
                    data[dstIdx + 0] = (pixel.r - Mean[0]) / Std[0]; // R
                    data[dstIdx + 1] = (pixel.g - Mean[1]) / Std[1]; // G
                    data[dstIdx + 2] = (pixel.b - Mean[2]) / Std[2]; // B
                }
            }

            return data;
        }
    }
}
