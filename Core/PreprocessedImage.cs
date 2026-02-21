namespace PicMotion.Core
{
    /// <summary>
    /// AIモデルへの入力用に前処理された画像データ。
    /// 推論エンジン非依存のフロート配列として保持する。
    /// </summary>
    public class PreprocessedImage
    {
        /// <summary> 正規化されたピクセルデータ。 </summary>
        public float[] PixelData { get; }

        /// <summary> 前処理後の画像幅。 </summary>
        public int Width { get; }

        /// <summary> 前処理後の画像高さ。 </summary>
        public int Height { get; }

        /// <summary> チャンネル数 (通常3: RGB)。 </summary>
        public int Channels { get; }

        public PreprocessedImage(float[] pixelData, int width, int height, int channels = 3)
        {
            PixelData = pixelData;
            Width = width;
            Height = height;
            Channels = channels;
        }
    }
}
