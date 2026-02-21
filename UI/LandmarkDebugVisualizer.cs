using PicMotion.Core;
using UnityEditor;
using UnityEngine;

namespace PicMotion.UI
{
    /// <summary>
    /// 推定されたランドマークをSceneビューにGizmosとして描画するデバッグユーティリティ。
    /// PicMotionWindow から有効化/無効化を制御する。
    /// </summary>
    [InitializeOnLoad]
    public static class LandmarkDebugVisualizer
    {
        private static PoseLandmark[] _landmarks;
        private static bool _enabled;

        static LandmarkDebugVisualizer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        /// <summary> 可視化するランドマークデータをセットする。 </summary>
        public static void SetLandmarks(PoseLandmark[] landmarks)
        {
            _landmarks = landmarks;
            _enabled = landmarks != null && landmarks.Length > 0;
            SceneView.RepaintAll();
        }

        /// <summary> 可視化をクリアする。 </summary>
        public static void Clear()
        {
            _landmarks = null;
            _enabled = false;
            SceneView.RepaintAll();
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!_enabled || _landmarks == null) return;

            Handles.BeginGUI();
            GUILayout.Label("  PicMotion: ランドマーク表示中", EditorStyles.miniLabel);
            Handles.EndGUI();

            DrawLandmarkSkeleton();
        }

        private static void DrawLandmarkSkeleton()
        {
            const float scale = 2f;
            var offset = new Vector3(-1f, 1.5f, 0f);

            // ランドマーク点の描画
            for (int i = 0; i < Mathf.Min(_landmarks.Length, LandmarkIndex.BodyAndFootCount); i++)
            {
                var lm = _landmarks[i];
                if (lm.Score < 0.3f) continue;

                var pos = lm.ToUnityPosition() * scale + offset;
                float size = 0.03f;

                Handles.color = lm.Score > 0.7f ? Color.green : Color.yellow;
                Handles.SphereHandleCap(0, pos, Quaternion.identity, size, EventType.Repaint);
            }

            // ボーン接続線の描画
            DrawBoneLine(LandmarkIndex.LeftShoulder, LandmarkIndex.RightShoulder, scale, offset);
            DrawBoneLine(LandmarkIndex.LeftShoulder, LandmarkIndex.LeftElbow, scale, offset);
            DrawBoneLine(LandmarkIndex.LeftElbow, LandmarkIndex.LeftWrist, scale, offset);
            DrawBoneLine(LandmarkIndex.RightShoulder, LandmarkIndex.RightElbow, scale, offset);
            DrawBoneLine(LandmarkIndex.RightElbow, LandmarkIndex.RightWrist, scale, offset);
            DrawBoneLine(LandmarkIndex.LeftHip, LandmarkIndex.RightHip, scale, offset);
            DrawBoneLine(LandmarkIndex.LeftHip, LandmarkIndex.LeftKnee, scale, offset);
            DrawBoneLine(LandmarkIndex.LeftKnee, LandmarkIndex.LeftAnkle, scale, offset);
            DrawBoneLine(LandmarkIndex.RightHip, LandmarkIndex.RightKnee, scale, offset);
            DrawBoneLine(LandmarkIndex.RightKnee, LandmarkIndex.RightAnkle, scale, offset);

            // 体幹（肩中間→腰中間）
            var neck = MidPoint(LandmarkIndex.LeftShoulder, LandmarkIndex.RightShoulder, scale, offset);
            var hips = MidPoint(LandmarkIndex.LeftHip, LandmarkIndex.RightHip, scale, offset);
            if (neck.HasValue && hips.HasValue)
            {
                Handles.color = Color.cyan;
                Handles.DrawLine(neck.Value, hips.Value);
            }

            // 頭
            DrawBoneLine(LandmarkIndex.Nose, LandmarkIndex.LeftEye, scale, offset);
            DrawBoneLine(LandmarkIndex.Nose, LandmarkIndex.RightEye, scale, offset);
        }

        private static void DrawBoneLine(int idxA, int idxB, float scale, Vector3 offset)
        {
            if (idxA >= _landmarks.Length || idxB >= _landmarks.Length) return;
            if (_landmarks[idxA].Score < 0.3f || _landmarks[idxB].Score < 0.3f) return;

            var posA = _landmarks[idxA].ToUnityPosition() * scale + offset;
            var posB = _landmarks[idxB].ToUnityPosition() * scale + offset;

            Handles.color = Color.cyan;
            Handles.DrawLine(posA, posB);
        }

        private static Vector3? MidPoint(int idxA, int idxB, float scale, Vector3 offset)
        {
            if (idxA >= _landmarks.Length || idxB >= _landmarks.Length) return null;
            if (_landmarks[idxA].Score < 0.3f || _landmarks[idxB].Score < 0.3f) return null;

            var posA = _landmarks[idxA].ToUnityPosition() * scale + offset;
            var posB = _landmarks[idxB].ToUnityPosition() * scale + offset;
            return (posA + posB) * 0.5f;
        }
    }
}
