using PicMotion.Core;
using PicMotion.PreProcessing;
using PicMotion.Inference;
using PicMotion.Kinematics;
using PicMotion.Export;
using UnityEngine;

namespace PicMotion.Pipeline
{
    /// <summary>
    /// ポーズ生成の全パイプラインを統括するオーケストレータ。
    /// 各モジュールをインターフェース経由で連携させる（DIP準拠）。
    /// アバターPrefabの一時インスタンス化もここで管理する。
    /// </summary>
    public class PoseGenerationPipeline
    {
        private readonly IImagePreprocessor _preprocessor;
        private readonly IPoseEstimator _estimator;
        private readonly IDepthEstimator _depthEstimator;
        private readonly IKinematicsSolver _solver;
        private readonly IAnimationExporter _exporter;

        public PoseGenerationPipeline(
            IImagePreprocessor preprocessor,
            IPoseEstimator estimator,
            IDepthEstimator depthEstimator,
            IKinematicsSolver solver,
            IAnimationExporter exporter)
        {
            _preprocessor = preprocessor;
            _estimator = estimator;
            _depthEstimator = depthEstimator;
            _solver = solver;
            _exporter = exporter;
        }

        /// <summary>
        /// パイプライン全体を実行する。
        /// アバターPrefabを一時的にインスタンス化し、完了後に破棄する。
        /// </summary>
        public PoseEstimationResult Execute(
            Texture2D sourceImage, GameObject avatarPrefab, string savePath)
        {
            // アバターの一時インスタンス生成
            var instance = Object.Instantiate(avatarPrefab);
            instance.hideFlags = HideFlags.HideAndDontSave;
            var animator = instance.GetComponent<Animator>();

            try
            {
                if (animator == null || !animator.avatar.isHuman)
                {
                    return PoseEstimationResult.Failure(
                        "指定されたPrefabにHumanoid Animatorが含まれていません。");
                }

                // Step 1: 画像前処理
                var preprocessed = _preprocessor.Process(sourceImage);

                // Step 2: ポーズ推定推論
                var landmarks = _estimator.Estimate(preprocessed);
                if (landmarks == null || landmarks.Length == 0)
                {
                    return PoseEstimationResult.Failure(
                        "ポーズが検出されませんでした。画像に人物が写っているか確認してください。");
                }

                // Step 3: 深度推定
                _depthEstimator.EstimateDepth(landmarks);

                // Step 4: キネマティクス計算
                var result = PoseEstimationResult.Success(landmarks);
                result.BoneRotations = _solver.Solve(landmarks, animator);

                // Step 5: アニメーション書き出し
                result.ExportedPath = _exporter.Export(
                    result.BoneRotations, animator, savePath);

                return result;
            }
            finally
            {
                Object.DestroyImmediate(instance);
            }
        }
    }
}
