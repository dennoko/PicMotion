using PicMotion.Core;
using UnityEngine;
using UnityEditor;

namespace PicMotion.Export
{
    /// <summary>
    /// HumanoidBoneRotation配列からHumanPoseHandler経由でマッスル値を取得し、
    /// Humanoid互換AnimationClipとして保存するエクスポータ。
    /// </summary>
    public class AnimationClipExporter : IAnimationExporter
    {
        public string Export(
            HumanoidBoneRotation[] boneRotations, Animator animator, string savePath)
        {
            if (!savePath.EndsWith(".anim"))
                savePath += ".anim";

            var muscles = ComputeMuscleValues(boneRotations, animator);
            var clip = CreateClipFromMuscles(muscles);
            SaveClip(clip, savePath);

            return savePath;
        }

        /// <summary>
        /// ボーン回転をスケルトンに適用し、HumanPoseHandlerでマッスル値を読み取る。
        /// </summary>
        private float[] ComputeMuscleValues(
            HumanoidBoneRotation[] boneRotations, Animator animator)
        {
            var handler = new HumanPoseHandler(
                animator.avatar, animator.transform);

            // ボーン回転をTransformに適用
            foreach (var rotation in boneRotations)
            {
                var bone = animator.GetBoneTransform(rotation.Bone);
                if (bone == null) continue;

                // ワールド空間のデルタ回転を適用
                bone.rotation = rotation.LocalRotation * bone.rotation;
            }

            // 適用後の姿勢からマッスル値を読み取り
            var humanPose = new HumanPose();
            handler.GetHumanPose(ref humanPose);
            handler.Dispose();

            return humanPose.muscles;
        }

        private AnimationClip CreateClipFromMuscles(float[] muscles)
        {
            var clip = new AnimationClip { legacy = false };

            for (int i = 0; i < muscles.Length && i < HumanTrait.MuscleCount; i++)
            {
                var binding = new EditorCurveBinding
                {
                    path = "",
                    type = typeof(Animator),
                    propertyName = HumanTrait.MuscleName[i]
                };

                var curve = new AnimationCurve(new Keyframe(0f, muscles[i]));
                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }

            return clip;
        }

        private void SaveClip(AnimationClip clip, string savePath)
        {
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
