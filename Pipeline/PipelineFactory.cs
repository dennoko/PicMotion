using PicMotion.PreProcessing;
using PicMotion.Inference;
using PicMotion.Kinematics;
using PicMotion.Export;
using Unity.Barracuda;

namespace PicMotion.Pipeline
{
    /// <summary>
    /// パイプラインの生成を一元化するファクトリ。
    /// UIから具象クラスへの直接依存を排除し、構成の変更を局所化する。
    /// </summary>
    public static class PipelineFactory
    {
        /// <summary>
        /// モック構成のパイプラインを生成する（デバッグ用）。
        /// </summary>
        public static PoseGenerationPipeline CreateMock()
        {
            return new PoseGenerationPipeline(
                preprocessor:   new MockImagePreprocessor(),
                estimator:      new MockPoseEstimator(),
                depthEstimator: new HeuristicDepthEstimator(),
                solver:         new MockKinematicsSolver(),
                exporter:       new AnimationClipExporter()
            );
        }

        /// <summary>
        /// Barracuda + RTMPose + ForwardKinematics 構成のパイプラインを生成する。
        /// </summary>
        public static PoseGenerationPipeline CreateWithBarracuda(NNModel modelAsset)
        {
            return new PoseGenerationPipeline(
                preprocessor:   new RtmPoseImagePreprocessor(),
                estimator:      new BarracudaRtmPoseEstimator(modelAsset),
                depthEstimator: new HeuristicDepthEstimator(),
                solver:         new ForwardKinematicsSolver(),
                exporter:       new AnimationClipExporter()
            );
        }
    }
}
