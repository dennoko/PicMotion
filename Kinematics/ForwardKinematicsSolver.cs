using System.Collections.Generic;
using PicMotion.Core;
using UnityEngine;

namespace PicMotion.Kinematics
{
    /// <summary>
    /// ランドマーク位置座標からHumanoidボーンの回転を計算する順運動学ソルバ。
    /// Tポーズのデフォルト方向からターゲット方向への差分回転を算出する。
    /// </summary>
    public class ForwardKinematicsSolver : IKinematicsSolver
    {
        private const float MinScore = 0.3f;

        public HumanoidBoneRotation[] Solve(PoseLandmark[] landmarks, Animator animator)
        {
            var positions = ComputeJointPositions(landmarks);
            var rotations = new List<HumanoidBoneRotation>();

            // 体幹
            TryAddRotation(rotations, HumanBodyBones.Hips,
                positions, "Hips", "Spine", Vector3.up);
            TryAddRotation(rotations, HumanBodyBones.Spine,
                positions, "Hips", "Spine", Vector3.up);
            TryAddRotation(rotations, HumanBodyBones.Chest,
                positions, "Spine", "Chest", Vector3.up);
            TryAddRotation(rotations, HumanBodyBones.Neck,
                positions, "Chest", "Neck", Vector3.up);
            TryAddRotation(rotations, HumanBodyBones.Head,
                positions, "Neck", "Head", Vector3.up);

            // 腕
            TryAddRotation(rotations, HumanBodyBones.LeftUpperArm,
                positions, "LeftShoulder", "LeftElbow", Vector3.left);
            TryAddRotation(rotations, HumanBodyBones.LeftLowerArm,
                positions, "LeftElbow", "LeftWrist", Vector3.left);
            TryAddRotation(rotations, HumanBodyBones.RightUpperArm,
                positions, "RightShoulder", "RightElbow", Vector3.right);
            TryAddRotation(rotations, HumanBodyBones.RightLowerArm,
                positions, "RightElbow", "RightWrist", Vector3.right);

            // 脚
            TryAddRotation(rotations, HumanBodyBones.LeftUpperLeg,
                positions, "LeftHip", "LeftKnee", Vector3.down);
            TryAddRotation(rotations, HumanBodyBones.LeftLowerLeg,
                positions, "LeftKnee", "LeftAnkle", Vector3.down);
            TryAddRotation(rotations, HumanBodyBones.RightUpperLeg,
                positions, "RightHip", "RightKnee", Vector3.down);
            TryAddRotation(rotations, HumanBodyBones.RightLowerLeg,
                positions, "RightKnee", "RightAnkle", Vector3.down);

            return rotations.ToArray();
        }

        /// <summary>
        /// ランドマーク配列から名前付きジョイント位置のマップを構築する。
        /// 中間ジョイント(Hips, Spine, Chest, Neck)は補間で生成する。
        /// </summary>
        private Dictionary<string, Vector3> ComputeJointPositions(PoseLandmark[] lm)
        {
            var p = new Dictionary<string, Vector3>();

            // 直接マッピング
            TryAdd(p, "Nose",           lm, LandmarkIndex.Nose);
            TryAdd(p, "LeftShoulder",   lm, LandmarkIndex.LeftShoulder);
            TryAdd(p, "RightShoulder",  lm, LandmarkIndex.RightShoulder);
            TryAdd(p, "LeftElbow",      lm, LandmarkIndex.LeftElbow);
            TryAdd(p, "RightElbow",     lm, LandmarkIndex.RightElbow);
            TryAdd(p, "LeftWrist",      lm, LandmarkIndex.LeftWrist);
            TryAdd(p, "RightWrist",     lm, LandmarkIndex.RightWrist);
            TryAdd(p, "LeftHip",        lm, LandmarkIndex.LeftHip);
            TryAdd(p, "RightHip",       lm, LandmarkIndex.RightHip);
            TryAdd(p, "LeftKnee",       lm, LandmarkIndex.LeftKnee);
            TryAdd(p, "RightKnee",      lm, LandmarkIndex.RightKnee);
            TryAdd(p, "LeftAnkle",      lm, LandmarkIndex.LeftAnkle);
            TryAdd(p, "RightAnkle",     lm, LandmarkIndex.RightAnkle);

            // 仮想ジョイント（中間点の補間）
            if (p.ContainsKey("LeftHip") && p.ContainsKey("RightHip"))
                p["Hips"] = (p["LeftHip"] + p["RightHip"]) * 0.5f;

            if (p.ContainsKey("LeftShoulder") && p.ContainsKey("RightShoulder"))
                p["Neck"] = (p["LeftShoulder"] + p["RightShoulder"]) * 0.5f;

            if (p.ContainsKey("Hips") && p.ContainsKey("Neck"))
            {
                p["Spine"] = Vector3.Lerp(p["Hips"], p["Neck"], 0.33f);
                p["Chest"] = Vector3.Lerp(p["Hips"], p["Neck"], 0.66f);
            }

            if (p.ContainsKey("Nose"))
                p["Head"] = p["Nose"];

            return p;
        }

        private void TryAdd(
            Dictionary<string, Vector3> map, string name,
            PoseLandmark[] landmarks, int index)
        {
            if (index < landmarks.Length && landmarks[index].Score >= MinScore)
                map[name] = landmarks[index].ToUnityPosition();
        }

        private void TryAddRotation(
            List<HumanoidBoneRotation> results,
            HumanBodyBones bone,
            Dictionary<string, Vector3> positions,
            string parentKey, string childKey,
            Vector3 tposeDirection)
        {
            if (!positions.ContainsKey(parentKey) || !positions.ContainsKey(childKey))
                return;

            var dir = (positions[childKey] - positions[parentKey]).normalized;
            if (dir.sqrMagnitude < 0.0001f) return;

            var rotation = Quaternion.FromToRotation(tposeDirection, dir);
            results.Add(new HumanoidBoneRotation(bone, rotation));
        }
    }
}
