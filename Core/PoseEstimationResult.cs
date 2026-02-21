namespace PicMotion.Core
{
    /// <summary>
    /// ポーズ生成パイプライン全体の結果を保持するコンテナ。
    /// ファクトリメソッドで生成し、パイプラインの各段階でプロパティを蓄積する。
    /// </summary>
    public class PoseEstimationResult
    {
        /// <summary> 推定された全ランドマーク座標の配列。 </summary>
        public PoseLandmark[] Landmarks { get; }

        /// <summary> キネマティクス計算後のHumanoidボーン回転の配列。 </summary>
        public HumanoidBoneRotation[] BoneRotations { get; set; }

        /// <summary> エクスポートされたファイルのパス。 </summary>
        public string ExportedPath { get; set; }

        /// <summary> 処理が成功したかどうか。 </summary>
        public bool IsValid { get; }

        /// <summary> エラーメッセージ（失敗時）。 </summary>
        public string ErrorMessage { get; }

        private PoseEstimationResult(PoseLandmark[] landmarks, string errorMessage)
        {
            Landmarks = landmarks;
            ErrorMessage = errorMessage;
            IsValid = string.IsNullOrEmpty(errorMessage)
                      && landmarks != null
                      && landmarks.Length > 0;
        }

        public static PoseEstimationResult Success(PoseLandmark[] landmarks)
        {
            return new PoseEstimationResult(landmarks, null);
        }

        public static PoseEstimationResult Failure(string errorMessage)
        {
            return new PoseEstimationResult(null, errorMessage);
        }
    }
}
