[English](Description.md) / [日本語](Description.ja.md)

---

# PerspectiveCamera

CubismモデルパーツのデフォルトのZオフセットは、特定のPerspectiveカメラ設定ではうまく機能しません。Perspectiveカメラを使用したいが、デフォルトのZオフセットを使用しても期待どおりの結果が得られない場合は、``CubismRenderController.SortingMode``を使用して別のソーティングモードに切り替えてください。

このサンプルは、``CubismRenderController.CameraToFace``を上書きしてモデルをカメラに向ける方法を示しています。
