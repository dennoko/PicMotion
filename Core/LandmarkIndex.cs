namespace PicMotion.Core
{
    /// <summary>
    /// RTMPose-Wholebody の主要ランドマークインデックス定義。
    /// Body(COCO-17): 0-16, Foot: 17-22, Face: 23-90, LeftHand: 91-111, RightHand: 112-132
    /// </summary>
    public static class LandmarkIndex
    {
        // ── Body (COCO-17) ──
        public const int Nose = 0;
        public const int LeftEye = 1;
        public const int RightEye = 2;
        public const int LeftEar = 3;
        public const int RightEar = 4;
        public const int LeftShoulder = 5;
        public const int RightShoulder = 6;
        public const int LeftElbow = 7;
        public const int RightElbow = 8;
        public const int LeftWrist = 9;
        public const int RightWrist = 10;
        public const int LeftHip = 11;
        public const int RightHip = 12;
        public const int LeftKnee = 13;
        public const int RightKnee = 14;
        public const int LeftAnkle = 15;
        public const int RightAnkle = 16;

        // ── Foot ──
        public const int LeftBigToe = 17;
        public const int LeftSmallToe = 18;
        public const int LeftHeel = 19;
        public const int RightBigToe = 20;
        public const int RightSmallToe = 21;
        public const int RightHeel = 22;

        // ── ランドマーク数 ──
        public const int TotalCount = 133;
        public const int BodyCount = 17;
        public const int BodyAndFootCount = 23;
    }
}
