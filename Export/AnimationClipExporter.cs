using PicMotion.Core;
using UnityEngine;
using UnityEditor;

namespace PicMotion.Export
{
    /// <summary>
    /// HumanoidBoneRotation配列からAnimationClipを生成しアセットとして保存する。
    /// Phase 2ではHumanoidマッスル値を0（デフォルト姿勢）で書き出す。
    /// Phase 4以降で実際のボーン回転→マッスル値変換を実装予定。
    /// </summary>
    public class AnimationClipExporter : IAnimationExporter
    {
        public string Export(
            HumanoidBoneRotation[] boneRotations, Avatar referenceAvatar, string savePath)
        {
            if (!savePath.EndsWith(".anim"))
                savePath += ".anim";

            var clip = CreateClip(boneRotations);
            SaveClip(clip, savePath);

            return savePath;
        }

        private AnimationClip CreateClip(HumanoidBoneRotation[] boneRotations)
        {
            var clip = new AnimationClip { legacy = false };

            // Phase 2: 全マッスル値をデフォルト (0) で書き出し
            // Phase 4以降: boneRotations から HumanPoseHandler 経由でマッスル値を計算
            for (int i = 0; i < HumanTrait.MuscleCount; i++)
            {
                var binding = new EditorCurveBinding
                {
                    path = "",
                    type = typeof(Animator),
                    propertyName = HumanTrait.MuscleName[i]
                };

                var curve = new AnimationCurve(new Keyframe(0f, 0f));
                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }

            return clip;
        }

        private void SaveClip(AnimationClip clip, string savePath)
        {
            // 既存アセットがあれば上書き、なければ新規作成
            var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(savePath);
            if (existing != null)
            {
                EditorUtility.CopySerialized(clip, existing);
                AssetDatabase.SaveAssets();
            }
            else
            {
                AssetDatabase.CreateAsset(clip, savePath);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
