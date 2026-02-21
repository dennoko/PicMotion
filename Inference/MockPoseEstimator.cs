using PicMotion.Core;
using UnityEngine;

namespace PicMotion.Inference
{
    /// <summary>
    /// モック用のポーズエスティメータ。
    /// 立ちTポーズを模した固定ランドマークを返す（パイプライン疎通確認用）。
    /// </summary>
    public class MockPoseEstimator : IPoseEstimator
    {
        public PoseLandmark[] Estimate(PreprocessedImage image)
        {
            var landmarks = new PoseLandmark[LandmarkIndex.TotalCount];

            // Body keypoints (COCO-17): 立ちTポーズの模擬座標
            SetBodyKeypoints(landmarks);

            // 未使用のランドマーク (Face/Hand) はスコア0のまま
            return landmarks;
        }

        private void SetBodyKeypoints(PoseLandmark[] landmarks)
        {
            // 正規化座標 (0〜1) での模擬Tポーズ配置
            landmarks[LandmarkIndex.Nose]          = new PoseLandmark(0.50f, 0.12f, 0.95f);
            landmarks[LandmarkIndex.LeftEye]       = new PoseLandmark(0.48f, 0.10f, 0.90f);
            landmarks[LandmarkIndex.RightEye]      = new PoseLandmark(0.52f, 0.10f, 0.90f);
            landmarks[LandmarkIndex.LeftEar]       = new PoseLandmark(0.45f, 0.11f, 0.85f);
            landmarks[LandmarkIndex.RightEar]      = new PoseLandmark(0.55f, 0.11f, 0.85f);
            landmarks[LandmarkIndex.LeftShoulder]  = new PoseLandmark(0.38f, 0.25f, 0.95f);
            landmarks[LandmarkIndex.RightShoulder] = new PoseLandmark(0.62f, 0.25f, 0.95f);
            landmarks[LandmarkIndex.LeftElbow]     = new PoseLandmark(0.28f, 0.25f, 0.90f);
            landmarks[LandmarkIndex.RightElbow]    = new PoseLandmark(0.72f, 0.25f, 0.90f);
            landmarks[LandmarkIndex.LeftWrist]     = new PoseLandmark(0.18f, 0.25f, 0.90f);
            landmarks[LandmarkIndex.RightWrist]    = new PoseLandmark(0.82f, 0.25f, 0.90f);
            landmarks[LandmarkIndex.LeftHip]       = new PoseLandmark(0.44f, 0.50f, 0.95f);
            landmarks[LandmarkIndex.RightHip]      = new PoseLandmark(0.56f, 0.50f, 0.95f);
            landmarks[LandmarkIndex.LeftKnee]      = new PoseLandmark(0.44f, 0.70f, 0.90f);
            landmarks[LandmarkIndex.RightKnee]     = new PoseLandmark(0.56f, 0.70f, 0.90f);
            landmarks[LandmarkIndex.LeftAnkle]     = new PoseLandmark(0.44f, 0.90f, 0.90f);
            landmarks[LandmarkIndex.RightAnkle]    = new PoseLandmark(0.56f, 0.90f, 0.90f);
        }

        public void Dispose() { }
    }
}
