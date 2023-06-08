# Live2D Virtual Human for Chatting based on Unity

[<img alt="NPM" src="https://img.shields.io/badge/license-MIT-green">](https://opensource.org/license/mit) 

## 项目介绍

视频演示：https://www.bilibili.com/video/BV1CW4y1X7PK

![MainScene](https://cdn.dggyu.top/image/MainScene.png?imageslim)

## 项目部署

### API申请

-   本项目不提供公用API Key，请自行申请。
    1.   [Azure](https://azure.microsoft.com/zh-cn/products/cognitive-services/)：[API文档](https://learn.microsoft.com/zh-cn/azure/cognitive-services/speech-service/)
    2.   [OpenAI](https://platform.openai.com/account/api-keys)：[API文档](https://platform.openai.com/docs/api-reference)
    3.   [APISpace](https://www.apispace.com/eolink/api/wbqgfx/introduction)：[API文档](https://www.apispace.com/eolink/api/wbqgfx/apiDocument#scroll=0)


-   申请好后请将 API Key 填入设置中，项目会持久存储这些数据。

![SpeakSetting](https://cdn.dggyu.top/image/SpeakSetting.png?imageslim)

### 环境

- Unity 2021.3.0

### Unity包导入

下载最新版本的Unity包，导入依赖后导入你的Unity工程中：

- [OpenCVPlusUnity](https://assetstore.unity.com/packages/tools/integration/opencv-plus-unity-85928)：处理图像相关操作，用于人脸检测等模块
- Newtonsoft.Json：序列化、反序列化等JSON相关操作
    - Add Package from git URL : `com.unity.nuget.newtonsoft-json`

## 基础架构

![image-20230608200934228](https://cdn.dggyu.top/image/image-20230608200934228.png?imageslim)

## 技术文档

如果你对本项目的实现感兴趣，我们记录下了一些开发、学习时的过程。供参考：[技术文档](./Assets/Documentation/doc.md)

## 许可

-   本项目代码根据 [MIT](https://opensource.org/license/mit) 许可证获得许可。

-   [模型](#致谢)以及[美术资源](#致谢)均由其作者各自拥有，必须在其各自作者的许可下使用。

## 引用

如果您认为本项目对您的工作有用，请考虑引用：

```tex
@misc{VHC,
  author = {Tianyu and Si, Haoyu and Gao, Lingjie and Wang},
  title = {Live2D Virtual Human for Chatting based on Unity},
  year = {2023},
  publisher = {GitHub},
  journal = {https://github.com/Navi-Studio/Virtual-Human-for-Chatting},
  school = {Nanjing University}
}
```

## 6 致谢

我们非常感谢画师 [草莓奶兔](https://www.bilibili.com/video/BV1hB4y1Q7vn) 提供的“桃花酪元子” Live2D 模型，以及 [是柚子呀owo](https://www.bilibili.com/video/BV1RW4y14715) 提供的精美背景原画。非常感谢以上作者的优秀作品。

感谢 [wangph.2020@gmail.com](mailto:wangph.2020@gmail.com) 提供的[OpenAI API代理](https://www.openai-proxy.com/)，使得本项目可以在国内网络环境下直接调用 OpenAI 服务。
