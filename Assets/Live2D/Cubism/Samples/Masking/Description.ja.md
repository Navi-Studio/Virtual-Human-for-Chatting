[English](Description.md) / [日本語](Description.ja.md)

---

# Masking

インポート時にクリッピングマスクを使用するCubismモデルに対して（および手動で``CubismModel3Json.ToModel()``を呼び出すたびに）クリッピングマスクが自動的に設定されます。

ただし、デフォルトでは、すべてのモデルが内部のマスクデータ用に同じグローバルGPUバッファー（``CubismMaskTexture.GlobalMaskTexture``）を共有します。モデルがそのバッファーに収まるよりも多くのクリッピングマスクを使用している場合は、必ずバッファーを調整するか、``CubismMaskController.MaskTexture``を介してモデルにカスタムバッファーを割り当ててください。

シーン内のQuadは、GPUバッファー内のクリッピングマスクのレイアウトを示しています。
