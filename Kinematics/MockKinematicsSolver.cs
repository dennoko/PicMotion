using PicMotion.Core;
using UnityEngine;

namespace PicMotion.Kinematics
{
    /// <summary>
    /// モック用のキネマティクスソルバ。
    /// 全Humanoidボーンにidentity回転を返す（パイプライン疎通確認用）。
    /// </summary>
    public class MockKinematicsSolver : IKinematicsSolver
    {
        /// <summary>
        /// Humanoidの主要ボーンリスト。
        /// </summary>
        private static readonly HumanBodyBones[] TargetBones = new[]
        {
            HumanBodyBones.Hips,
            HumanBodyBones.Spine,
            HumanBodyBones.Chest,
            HumanBodyBones.Neck,
            HumanBodyBones.Head,
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.LeftUpperLeg,
            HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.RightUpperLeg,
            HumanBodyBones.RightLowerLeg,
        };

        public HumanoidBoneRotation[] Solve(PoseLandmark[] landmarks, Avatar referenceAvatar)
        {
            var rotations = new HumanoidBoneRotation[TargetBones.Length];
            for (int i = 0; i < TargetBones.Length; i++)
            {
                rotations[i] = new HumanoidBoneRotation(
                    TargetBones[i], Quaternion.identity);
            }
            return rotations;
        }
    }
}
