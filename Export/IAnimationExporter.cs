using PicMotion.Core;
using UnityEngine;

namespace PicMotion.Export
{
    /// <summary>
    /// HumanoidBoneRotation配列からAnimationClipを生成し保存するエクスポータのインターフェース。
    /// </summary>
    public interface IAnimationExporter
    {
        /// <summary>
        /// ボーン回転情報をAnimationClipとしてエクスポートする。
        /// </summary>
        /// <param name="boneRotations">各ボーンの回転情報</param>
        /// <param name="referenceAvatar">ボーン階層パスの解決に使用するアバター</param>
        /// <param name="savePath">保存先アセットパス (.anim)</param>
        /// <returns>保存されたアセットのパス</returns>
        string Export(HumanoidBoneRotation[] boneRotations, Avatar referenceAvatar, string savePath);
    }
}
