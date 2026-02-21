using PicMotion.Core;
using UnityEngine;

namespace PicMotion.Kinematics
{
    /// <summary>
    /// ヒューリスティックによるZ軸（深度）推定。
    /// ボーンの見かけの長さ比率から幾何学的にZ値を推定する初期実装。
    /// 将来的にMiDaS等の深度モデルベースの実装に差し替え可能（OCP準拠）。
    /// </summary>
    public class HeuristicDepthEstimator : IDepthEstimator
    {
        /// <summary>
        /// 現在のPhase 1実装ではZ値を0のまま維持する（2D平面上のポーズ）。
        /// Phase 2以降でボーン長比率に基づく推定ロジックを段階的に追加予定。
        /// </summary>
        public void EstimateDepth(PoseLandmark[] landmarks)
        {
            // Phase 1: Z値はすべて0（正面向き2Dポーズとして扱う）
            // TODO: ボーン長比率から肘・膝のZ推定を実装
            // 例: 上腕の見かけの長さが実際より短い → 肘が手前にある → Z < 0
        }
    }
}
