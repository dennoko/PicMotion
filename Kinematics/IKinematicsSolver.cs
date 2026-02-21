using PicMotion.Core;
using UnityEngine;

namespace PicMotion.Kinematics
{
    /// <summary>
    /// ランドマーク位置座標からHumanoidボーンのローカル回転を計算するソルバのインターフェース。
    /// </summary>
    public interface IKinematicsSolver
    {
        /// <summary>
        /// ランドマーク座標とアバターのAnimatorから、各ボーンの回転を計算する。
        /// </summary>
        /// <param name="landmarks">推定済みランドマーク座標（深度付き）</param>
        /// <param name="animator">基準姿勢(Tポーズ)のAnimator（一時インスタンス）</param>
        /// <returns>各ボーンの計算済み回転の配列</returns>
        HumanoidBoneRotation[] Solve(PoseLandmark[] landmarks, Animator animator);
    }
}
