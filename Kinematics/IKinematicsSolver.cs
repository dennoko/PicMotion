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
        /// ランドマーク座標と参照アバターから、各ボーンのローカル回転を計算する。
        /// </summary>
        /// <param name="landmarks">推定済みランドマーク座標（深度付き）</param>
        /// <param name="referenceAvatar">基準姿勢(Tポーズ)の取得元アバター</param>
        /// <returns>各ボーンの計算済みローカル回転の配列</returns>
        HumanoidBoneRotation[] Solve(PoseLandmark[] landmarks, Avatar referenceAvatar);
    }
}
