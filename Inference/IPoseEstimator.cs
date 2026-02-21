using System;
using PicMotion.Core;

namespace PicMotion.Inference
{
    /// <summary>
    /// 前処理済み画像からポーズランドマークを推定するエスティメータのインターフェース。
    /// </summary>
    public interface IPoseEstimator : IDisposable
    {
        /// <summary>
        /// 推論を実行し、ランドマーク座標の配列を返す。
        /// 人物が検出できない場合はnullを返す。
        /// </summary>
        PoseLandmark[] Estimate(PreprocessedImage image);
    }
}
