[English](Description.md) / [日本語](Description.ja.md)

---

# MultipleModels

他の2D要素と簡単に混ぜられるように、CubismモデルのパーツはデフォルトでZオフセットを使用して並べ替えられます。これにより、UnityのSortingLayerやSortingOrderのいずれかで複数のモデルを並べ替える必要があります。どちらも、``CubismRenderController.SortingLayer``および``CubismRenderController.SortingOrder``設定を介して簡単に実現できます。

モデルの並べ替えを明確に分離することで、期待される目に見える結果が得られるだけでなく、UnityのDynamic bathingを機能させることもできます（もちろん、有効になっている場合のみ）。
