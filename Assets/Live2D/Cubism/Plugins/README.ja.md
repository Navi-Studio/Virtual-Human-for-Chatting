[English](README.md) / [日本語](README.ja.md)

---

# Live2D Cubism Core

このフォルダには、プラットフォーム固有のライブラリファイルが含まれています。

**最初のインポート時に例外が発生した場合は、Unityを再起動してください。**

## ライブラリリスト

| プラットフォーム | アーキテクチャ | パス | 注記 |
| --- | --- | --- | --- |
| Android | ARM64 | Android/arm64-v8a |   |
| Android | ARMv7 | Android/armeabi-v7a |   |
| Android | x86 | Android/x86 |   |
| Android | x86_64 | Android/x86_64 |   |
| Emscripten |  | Experimental/Emscripten/latest | bitcode（upstream LLVM wasmバックエンド） |
| Emscripten |  | Experimental/Emscripten/1_38_48 | bitcode（fastcompバックエンド） |
| iOS | ARM64 | iOS/xxx-iphoneos | iOSデバイス |
| iOS | x86_64 | iOS/xxx-iphonesimulator | iOS Simulator |
| Linux | x86_64 | Linux/x86_64 |   |
| macOS | x86_64 | macOS |   |
| macOS | ARM64 | macOS |   |
| UWP | ARM | Experimental/UWP/arm |   |
| UWP | ARM64 | Experimental/UWP/arm64 |   |
| UWP | x64 | Experimental/UWP/x64 |   |
| UWP | x86 | Experimental/UWP/x86 |   |
| Windows | x86 | Windows/x86 |   |
| Windows | x86_64 | Windows/x86_64 |   |

### 実験的ライブラリ

`Emscripten`と`UWP`は実験的なライブラリです。
