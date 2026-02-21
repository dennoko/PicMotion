using PicMotion.Core;
using UnityEngine;

namespace PicMotion.PreProcessing
{
    /// <summary>
    /// 画像をAI推論モデルの入力形式に変換するプリプロセッサのインターフェース。
    /// </summary>
    public interface IImagePreprocessor
    {
        /// <summary>
        /// Texture2Dを推論エンジン入力用のデータに変換する。
        /// </summary>
        PreprocessedImage Process(Texture2D sourceImage);
    }
}
