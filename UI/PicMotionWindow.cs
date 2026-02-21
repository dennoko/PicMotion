using UnityEditor;
using UnityEngine;
using PicMotion.Pipeline;

namespace PicMotion.UI
{
    /// <summary>
    /// PicMotion のメインエディタウィンドウ。
    /// ユーザー入力を受け取り、PipelineFactoryで生成したパイプラインを実行する。
    /// </summary>
    public class PicMotionWindow : EditorWindow
    {
        private Texture2D _sourceImage;
        private GameObject _avatarPrefab;
        private string _savePath = "Assets/PicMotion_Output.anim";
        private string _statusMessage;
        private MessageType _statusType = MessageType.None;

        [MenuItem("Tools/PicMotion")]
        public static void ShowWindow()
        {
            var window = GetWindow<PicMotionWindow>("PicMotion");
            window.minSize = new Vector2(350, 280);
        }

        private void OnGUI()
        {
            DrawHeader();
            EditorGUILayout.Space(8);
            DrawInputFields();
            EditorGUILayout.Space(8);
            DrawSavePath();
            EditorGUILayout.Space(12);
            DrawGenerateButton();
            DrawStatus();
        }

        // ── UI描画メソッド群 ──

        private void DrawHeader()
        {
            EditorGUILayout.LabelField(
                "PicMotion - 画像からポーズ生成", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "画像から人物のポーズを推定し、Humanoidアニメーションを生成します。",
                EditorStyles.wordWrappedMiniLabel);
        }

        private void DrawInputFields()
        {
            _sourceImage = (Texture2D)EditorGUILayout.ObjectField(
                "入力画像", _sourceImage, typeof(Texture2D), false);

            _avatarPrefab = (GameObject)EditorGUILayout.ObjectField(
                "アバターPrefab", _avatarPrefab, typeof(GameObject), false);
        }

        private void DrawSavePath()
        {
            EditorGUILayout.BeginHorizontal();
            _savePath = EditorGUILayout.TextField("保存先", _savePath);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                var path = EditorUtility.SaveFilePanelInProject(
                    "アニメーション保存先", "NewPose", "anim",
                    "保存先を選択してください");
                if (!string.IsNullOrEmpty(path))
                    _savePath = path;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGenerateButton()
        {
            bool canGenerate = _sourceImage != null && _avatarPrefab != null;

            EditorGUI.BeginDisabledGroup(!canGenerate);
            if (GUILayout.Button("ポーズを生成", GUILayout.Height(32)))
            {
                ExecutePipeline();
            }
            EditorGUI.EndDisabledGroup();

            if (!canGenerate)
            {
                EditorGUILayout.HelpBox(
                    "入力画像とアバターPrefabを設定してください。",
                    MessageType.Info);
            }
        }

        private void DrawStatus()
        {
            if (!string.IsNullOrEmpty(_statusMessage))
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.HelpBox(_statusMessage, _statusType);
            }
        }

        // ── パイプライン実行 ──

        private void ExecutePipeline()
        {
            var avatar = ValidateAvatar();
            if (avatar == null) return;

            var pipeline = PipelineFactory.CreateDefault();
            var result = pipeline.Execute(_sourceImage, avatar, _savePath);

            if (result.IsValid)
            {
                _statusMessage = $"✓ 生成完了: {result.ExportedPath}";
                _statusType = MessageType.Info;
                AssetDatabase.Refresh();

                // 生成されたアセットを選択状態にする
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(result.ExportedPath);
                if (clip != null) Selection.activeObject = clip;
            }
            else
            {
                _statusMessage = $"✗ エラー: {result.ErrorMessage}";
                _statusType = MessageType.Error;
            }
        }

        private Avatar ValidateAvatar()
        {
            var animator = _avatarPrefab.GetComponent<Animator>();
            if (animator == null || animator.avatar == null)
            {
                _statusMessage = "エラー: PrefabにAnimatorコンポーネントがありません。";
                _statusType = MessageType.Error;
                return null;
            }

            if (!animator.avatar.isHuman)
            {
                _statusMessage = "エラー: Humanoid Avatarが設定されていません。";
                _statusType = MessageType.Error;
                return null;
            }

            return animator.avatar;
        }
    }
}
