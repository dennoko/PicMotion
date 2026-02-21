using PicMotion.PreProcessing;
using PicMotion.Inference;
using PicMotion.Kinematics;
using PicMotion.Export;

namespace PicMotion.Pipeline
{
    /// <summary>
    /// パイプラインの生成を一元化するファクトリ。
    /// UIから具象クラスへの直接依存を排除し、構成の変更を局所化する。
    /// </summary>
    public static class PipelineFactory
    {
        /// <summary>
        /// デフォルト構成（モック + ヒューリスティック深度）のパイプラインを生成する。
        /// Phase 3以降で実装クラスに順次差し替える。
        /// </summary>
        public static PoseGenerationPipeline CreateDefault()
        {
            return new PoseGenerationPipeline(
                preprocessor:  new MockImagePreprocessor(),
                estimator:     new MockPoseEstimator(),
                depthEstimator: new HeuristicDepthEstimator(),
                solver:        new MockKinematicsSolver(),
                exporter:      new AnimationClipExporter()
            );
        }
    }
}
