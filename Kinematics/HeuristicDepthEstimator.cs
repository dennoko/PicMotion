using PicMotion.Core;
using UnityEngine;

namespace PicMotion.Kinematics
{
    /// <summary>
    /// ヒューリスティックによるZ軸（深度）推定。
    /// ボーンの見かけの長さ比率から幾何学的にZ値を推定する。
    /// 原理: Tポーズでのボーン長が既知のとき、見かけの2D長が短いほど手前/奥にある。
    /// </summary>
    public class HeuristicDepthEstimator : IDepthEstimator
    {
        private const float MinScore = 0.3f;

        // 各ボーン（親→子）の定義と、Tポーズでの見かけの長さ比率の基準
        private static readonly (int parentIdx, int childIdx)[] BonePairs =
        {
            (LandmarkIndex.LeftShoulder, LandmarkIndex.LeftElbow),
            (LandmarkIndex.LeftElbow, LandmarkIndex.LeftWrist),
            (LandmarkIndex.RightShoulder, LandmarkIndex.RightElbow),
            (LandmarkIndex.RightElbow, LandmarkIndex.RightWrist),
            (LandmarkIndex.LeftHip, LandmarkIndex.LeftKnee),
            (LandmarkIndex.LeftKnee, LandmarkIndex.LeftAnkle),
            (LandmarkIndex.RightHip, LandmarkIndex.RightKnee),
            (LandmarkIndex.RightKnee, LandmarkIndex.RightAnkle),
        };

        public void EstimateDepth(PoseLandmark[] landmarks)
        {
            if (landmarks == null || landmarks.Length < LandmarkIndex.BodyCount)
                return;

            // 肩幅を基準長とする（正面を向いているときにほぼ一定の長さ）
            float shoulderWidth = ComputeApparent2DLength(
                landmarks, LandmarkIndex.LeftShoulder, LandmarkIndex.RightShoulder);

            if (shoulderWidth < 0.01f) return;

            // 各ボーンペアについて、見かけの長さからZ値を推定
            foreach (var (parentIdx, childIdx) in BonePairs)
            {
                EstimateBoneDepth(landmarks, parentIdx, childIdx, shoulderWidth);
            }
        }

        /// <summary>
        /// 1本のボーンについて、見かけの2D長と想定3D長を比較し、子関節のZを推定。
        /// 想定3D長は肩幅に対する比率で近似する。
        /// </summary>
        private void EstimateBoneDepth(
            PoseLandmark[] lm, int parentIdx, int childIdx, float shoulderWidth)
        {
            if (parentIdx >= lm.Length || childIdx >= lm.Length) return;
            if (lm[parentIdx].Score < MinScore || lm[childIdx].Score < MinScore) return;

            float apparent2D = ComputeApparent2DLength(lm, parentIdx, childIdx);

            // 上腕・前腕は肩幅の約0.7倍、大腿・下腿は肩幅の約1.2倍
            float expectedRatio = IsArmBone(parentIdx) ? 0.7f : 1.2f;
            float expected3D = shoulderWidth * expectedRatio;

            if (expected3D < 0.01f) return;

            // 見かけが想定より短い → 奥行き方向にオフセットがある
            float ratio = Mathf.Clamp01(apparent2D / expected3D);

            // Z = sqrt(expected^2 - apparent^2) ※直角三角形
            float zOffset = 0f;
            if (ratio < 0.95f)
            {
                float apparent = apparent2D;
                zOffset = Mathf.Sqrt(
                    Mathf.Max(0, expected3D * expected3D - apparent * apparent));
            }

            // 親のZ値 + オフセット（方向は肘/膝の曲がり方から推定が必要だが、
            // 簡易版ではオフセットのみ適用）
            lm[childIdx].Depth = lm[parentIdx].Depth + zOffset;
        }

        private float ComputeApparent2DLength(PoseLandmark[] lm, int a, int b)
        {
            return Vector2.Distance(lm[a].Position, lm[b].Position);
        }

        private bool IsArmBone(int parentIdx)
        {
            return parentIdx == LandmarkIndex.LeftShoulder
                || parentIdx == LandmarkIndex.LeftElbow
                || parentIdx == LandmarkIndex.RightShoulder
                || parentIdx == LandmarkIndex.RightElbow;
        }
    }
}
