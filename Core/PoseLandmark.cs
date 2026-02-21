using UnityEngine;

namespace PicMotion.Core
{
    /// <summary>
    /// AIモデルから推定された1つのランドマーク（関節点）の情報。
    /// </summary>
    [System.Serializable]
    public struct PoseLandmark
    {
        /// <summary> 正規化された2D座標 (0〜1)。AIモデルの出力形式。 </summary>
        public Vector2 Position;

        /// <summary> ヒューリスティックまたは深度モデルにより推定されたZ値。未推定時は0。 </summary>
        public float Depth;

        /// <summary> 推定の信頼度スコア (0〜1)。 </summary>
        public float Score;

        public PoseLandmark(Vector2 position, float depth, float score)
        {
            Position = position;
            Depth = depth;
            Score = score;
        }

        public PoseLandmark(float x, float y, float score)
        {
            Position = new Vector2(x, y);
            Depth = 0f;
            Score = score;
        }

        /// <summary>
        /// AI出力座標系(左上原点, Y↓) → Unity座標系(Y↑, Z奥行) に変換した3D位置を返す。
        /// </summary>
        public Vector3 ToUnityPosition()
        {
            return new Vector3(Position.x, -Position.y, -Depth);
        }
    }
}
