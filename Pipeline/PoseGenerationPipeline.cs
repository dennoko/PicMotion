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
        /// </summary>
        /// <param name="sourceImage">入力画像</param>
        /// <param name="referenceAvatar">基準姿勢のアバター</param>
        /// <param name="savePath">アニメーション保存先パス</param>
        /// <returns>パイプライン実行結果</returns>
        public PoseEstimationResult Execute(
            Texture2D sourceImage, Avatar referenceAvatar, string savePath)
        {
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
            result.BoneRotations = _solver.Solve(landmarks, referenceAvatar);

            // Step 5: アニメーション書き出し
            result.ExportedPath = _exporter.Export(
                result.BoneRotations, referenceAvatar, savePath);

            return result;
        }
    }
}
