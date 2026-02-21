using PicMotion.Core;
using UnityEngine;

namespace PicMotion.PreProcessing
{
    /// <summary>
    /// モック用の画像プリプロセッサ。
    /// ダミーのピクセルデータを返す（パイプライン疎通確認用）。
    /// </summary>
    public class MockImagePreprocessor : IImagePreprocessor
    {
        private const int DefaultSize = 256;

        public PreprocessedImage Process(Texture2D sourceImage)
        {
            int width = sourceImage != null ? sourceImage.width : DefaultSize;
            int height = sourceImage != null ? sourceImage.height : DefaultSize;

            // ダミーの正規化ピクセルデータを生成
            var pixelData = new float[DefaultSize * DefaultSize * 3];
            return new PreprocessedImage(pixelData, DefaultSize, DefaultSize);
        }
    }
}
