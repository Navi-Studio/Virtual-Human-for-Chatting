[English](NOTICE.md) / [日本語](NOTICE.ja.md)

---

# お知らせ

## [注意事項] Unity 2022 での動作について (2022-09-08)

本 Cubism SDK におきまして、`2022.1.14f1` にて Unity Editor 上と成果物の動作を確認しております。
ただし、試験的な動作確認であり、Unity Editor からの書き出し機能等含む全ての機能が正しく動作することを保証するものではございません。
アプリケーションに組み込み製品として利用する際は、Unity 2021 等 LTSバージョンのご利用をご検討ください。

対応する Unity のバージョンについては、`README.ja.md` の[開発環境](README.ja.md#開発環境)をご参照ください。


## [制限事項] Apple製品の対応状況について (2023-01-26 更新)

Apple Silicon製のMacにつきまして、Cubism 4 SDK for Unity R4 (4-r.4) にて対応いたしました。
※Cubism Editorは現在Apple Silicon製のMacに対応しておりません、ご了承ください。

また、macOS Ventura v13.0以降につきましては動作を保証しておりません、ご了承ください。


## [注意事項] Apple Silicon版 Unity Editor での動作について (2023-01-26)

Apple Silicon版Unity Editorでの動作につきまして、macOS向けのCubism Coreを利用するには `Assets/Live2D/Cubism/Plugins/macOS` 以下にある `Live2DCubismCore.bundle` をインスペクタから操作する必要があります。
手順は以下の通りとなります。

1. `Live2DCubismCore.bundle` を選択状態にし、インスペクタを表示する。
1. `Platform settings` の `Editor` を選択し、`Apple Silicon` または `Any CPU` を選択する。
1. Unity Editorを再起動する。


## [注意事項] Windows 11の対応状況について (2021-12-09)

Windows 11対応につきまして、Windows 11上にて成果物の動作を確認しております。
ただし、Windows 11を利用したビルドにつきましては動作を保証しておりません、ご了承ください。
対応バージョンや時期につきましては今後のリリースをもってお知らせいたします。


## [注意事項] macOS Catalina 以降での `.bundle` と `.dylib` の利用について

macOS Catalina 以降で `.bundle` と `.dylib` を利用する際、公証の確認のためオンライン環境に接続している必要があります。

詳しくは、Apple社 公式ドキュメントをご確認ください。

* [Apple社 公式ドキュメント](https://developer.apple.com/documentation/security/notarizing_your_app_before_distribution)
---

©Live2D
