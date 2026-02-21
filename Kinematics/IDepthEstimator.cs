using PicMotion.Core;

namespace PicMotion.Kinematics
{
    /// <summary>
    /// 2Dランドマーク座標にZ軸（深度）情報を付与するエスティメータのインターフェース。
    /// Phase 1ではヒューリスティック推定、将来的にMiDaS等の深度モデルに差し替え可能。
    /// </summary>
    public interface IDepthEstimator
    {
        /// <summary>
        /// ランドマーク配列の各要素のDepthフィールドを推定値で上書きする。
        /// </summary>
        void EstimateDepth(PoseLandmark[] landmarks);
    }
}
