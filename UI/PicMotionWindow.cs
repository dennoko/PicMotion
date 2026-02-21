using UnityEditor;
using UnityEngine;
using Unity.Barracuda;
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
        private NNModel _modelAsset;
        private string _savePath = "Assets/PicMotion_Output.anim";
        private string _statusMessage;
        private MessageType _statusType = MessageType.None;
        private bool _useMock;
        private bool _showDebugLandmarks;

        [MenuItem("Tools/PicMotion")]
        public static void ShowWindow()
        {
            var window = GetWindow<PicMotionWindow>("PicMotion");
            window.minSize = new Vector2(350, 320);
        }

        private void OnGUI()
        {
            DrawHeader();
            EditorGUILayout.Space(8);
            DrawModelSettings();
            EditorGUILayout.Space(4);
            DrawInputFields();
            EditorGUILayout.Space(8);
            DrawSavePath();
            EditorGUILayout.Space(12);
            DrawGenerateButton();
            DrawStatus();
        }

        // ── UI描画 ──

        private void DrawHeader()
        {
            EditorGUILayout.LabelField(
                "PicMotion - 画像からポーズ生成", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "画像から人物のポーズを推定し、Humanoidアニメーションを生成します。",
                EditorStyles.wordWrappedMiniLabel);
        }

        private void DrawModelSettings()
        {
            EditorGUILayout.LabelField("推論設定", EditorStyles.miniBoldLabel);
            _useMock = EditorGUILayout.Toggle("モックモード", _useMock);

            EditorGUI.BeginDisabledGroup(_useMock);
            _modelAsset = (NNModel)EditorGUILayout.ObjectField(
                "ONNXモデル", _modelAsset, typeof(NNModel), false);
            EditorGUI.EndDisabledGroup();

            _showDebugLandmarks = EditorGUILayout.Toggle(
                "Sceneビュー デバッグ表示", _showDebugLandmarks);
        }

        private void DrawInputFields()
        {
            EditorGUILayout.LabelField("入力", EditorStyles.miniBoldLabel);
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
            bool canGenerate = ValidateInputs();

            EditorGUI.BeginDisabledGroup(!canGenerate);
            if (GUILayout.Button("ポーズを生成", GUILayout.Height(32)))
                ExecutePipeline();
            EditorGUI.EndDisabledGroup();

            if (!canGenerate)
                EditorGUILayout.HelpBox(GetValidationMessage(), MessageType.Info);
        }

        private void DrawStatus()
        {
            if (!string.IsNullOrEmpty(_statusMessage))
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.HelpBox(_statusMessage, _statusType);
            }
        }

        // ── バリデーション ──

        private bool ValidateInputs()
        {
            if (_sourceImage == null || _avatarPrefab == null) return false;
            if (!_useMock && _modelAsset == null) return false;

            var animator = _avatarPrefab.GetComponent<Animator>();
            return animator != null && animator.avatar != null && animator.avatar.isHuman;
        }

        private string GetValidationMessage()
        {
            if (_sourceImage == null) return "入力画像を設定してください。";
            if (_avatarPrefab == null) return "アバターPrefabを設定してください。";

            var animator = _avatarPrefab.GetComponent<Animator>();
            if (animator == null || animator.avatar == null || !animator.avatar.isHuman)
                return "PrefabにHumanoid Animatorが必要です。";

            if (!_useMock && _modelAsset == null)
                return "ONNXモデルを設定するか、モックモードを有効にしてください。";
            return "";
        }

        // ── パイプライン実行 ──

        private void ExecutePipeline()
        {
            var pipeline = _useMock
                ? PipelineFactory.CreateMock()
                : PipelineFactory.CreateWithBarracuda(_modelAsset);

            try
            {
                var result = pipeline.Execute(_sourceImage, _avatarPrefab, _savePath);

                if (result.IsValid)
                {
                    _statusMessage = $"✓ 生成完了: {result.ExportedPath}";
                    _statusType = MessageType.Info;
                    AssetDatabase.Refresh();

                    // デバッグ可視化
                    if (_showDebugLandmarks && result.Landmarks != null)
                        LandmarkDebugVisualizer.SetLandmarks(result.Landmarks);
                    else
                        LandmarkDebugVisualizer.Clear();

                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
                        result.ExportedPath);
                    if (clip != null) Selection.activeObject = clip;
                }
                else
                {
                    _statusMessage = $"✗ エラー: {result.ErrorMessage}";
                    _statusType = MessageType.Error;
                }
            }
            catch (System.Exception e)
            {
                _statusMessage = $"✗ 例外: {e.Message}";
                _statusType = MessageType.Error;
                Debug.LogException(e);
            }
        }
    }
}
