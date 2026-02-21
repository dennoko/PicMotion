using UnityEngine;

namespace PicMotion.Core
{
    /// <summary>
    /// 1つのHumanoidボーンに対する計算済みローカル回転情報。
    /// </summary>
    [System.Serializable]
    public struct HumanoidBoneRotation
    {
        /// <summary> 対象のHumanoidボーン種別。 </summary>
        public HumanBodyBones Bone;

        /// <summary> 計算されたローカル回転。 </summary>
        public Quaternion LocalRotation;

        public HumanoidBoneRotation(HumanBodyBones bone, Quaternion localRotation)
        {
            Bone = bone;
            LocalRotation = localRotation;
        }
    }
}
