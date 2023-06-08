[English](Description.md) / [日本語](Description.ja.md)

---

# LookAt

このサンプルは、視線追従の機能を示しています。視線追従は手動で設定する必要がありますが、設定はそれほど面倒ではありません。モデルのGameObjectに``CubismLookController``コンポーネントを追加し、視線追従に連動させたいパラメータに ``CubismLookParameter``を追加するだけです。

モデルは、``ICubismLookTarget``に割り当てた``CubismLookContoller.Target``を追いかけます。
