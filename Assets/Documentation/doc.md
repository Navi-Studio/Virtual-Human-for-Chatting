# 技术文档

## 1 项目简介

### 项目目录

```
Assets
├─ Animator						// 动画
├─ Code							// 代码
│  ├─ AI							// AI 模块
│  │  ├─ LM								// 语言模型模块
│  │  │  └─ ChatGPT	
│  │  │     ├─ ChatScript.cs			// ChatGPT服务
│  │  │     └─ GptTurboScript.cs		// API调用
│  │  ├─ SA							// 情绪识别模块
│  │  │  └─ APISpace
│  │  │     ├─ SAEntity.cs				// SA实体
│  │  │     └─ SAService.cs				// SA服务
│  │  └─ TTS						// 智能语音模块
│  │     └─ Azure
│  │        ├─ AzureSpeech.cs			// Azure服务
│  │        └─ Entity					// 实体封装以及建造者
│  │           ├─ AdvancedTTSBuilder.cs
│  │           ├─ BasicTTSBuilder.cs
│  │           ├─ BlendShapeEntity.cs
│  │           ├─ TTSBuilder.cs
│  │           └─ TTSEntity.cs
│  └─ Scripts					// 脚本
│     ├─ AnimationEvents			// 动画事件
│     │  └─ TouchAnimationEvent.cs		// 触摸动画事件
│     ├─ Common						// 通用脚本
│     │  ├─ SetFPS.cs					// 设置全局FPS
│     │  └─ SingletonT.cs				// 单例模板
│     ├─ DataBase					// 持久化层
│     │  └─ GameSettingsEntity.cs		// 游戏设置实体
│     ├─ FunctionSystem				// 功能系统
│     │  └─ ReadSystem					// 朗读系统
│     │     ├─ InputSettingsEntity.cs		// 输入设置实体
│     │     └─ ReadService.cs				// 朗读服务
│     ├─ TaoHua						// Live2D模块
│     │  ├─ Audio2Face.cs				// 口型识别
│     │  ├─ ExpressionController.cs		// 表情控制
│     │  ├─ FreeChat.cs					// 自由对话模块
│     │  ├─ LookTargetController.cs		// 目光跟随
│     │  ├─ PlayerController.cs			// 玩家控制
│     │  ├─ TouchController.cs			// 触摸控制
│     │  └─ haarcascade_frontalface_default.xml
│     └─ UI							// 用户界面
│        ├─ Chat						// 聊天界面脚本
│        │  ├─ LocalDialog.cs				
│        │  ├─ OpenFileName.cs				
│        │  ├─ ReadEvents.cs				// 朗读事件
│        │  └─ SubmitChat.cs				// 提交事件
│        ├─ Common
│        │  └─ CommonUIEvents.cs			// 通用事件
│        ├─ LeftUI
│        │  └─ LeftEvents.cs				// 左侧UI事件
│        └─ Middle
│           └─ GameSettingsEvent.cs			// 游戏设置事件
├─ Documentation
│  └─ doc.md			// 技术文档
├─ Font					// 字体
├─ Live2D Model			// Live2D模型
├─ Live2D				// Live2D SDK
├─ Prefab				// 预制体
├─ Resources
├─ Scenes
│  └─ MainScene.unity	// 主场景
├─ Spirits				// 图片资源
├─ StreamingAssets
│  └─ Config
│     └─ haarcascade_frontalface_default.xml	// 已弃用
└─ Utils				// 外部资产
```

## 2 功能模块

### 2.1 Live2D 模块

该模块主要基于 Live2D 所提供的 Cubism SDK for Unity 进行开发，其主要功能的详细说明可参考 [官方文档](https://docs.live2d.com/zh-CHS/cubism-sdk-manual/cubismcomponents/) 。

#### 2.1.1 EyeBlink

参照 [文档](https://docs.live2d.com/zh-CHS/cubism-sdk-manual/eyeblink-unity/) 即可，组件的混合模式推荐设置为乘法。

#### 2.1.2 LookAt

>   LookAt是操作任意参数追随特定座标的值的功能。
>   通过自定义用户侧要追随的座标，可以使Cubism模型追随特定的GameObject等。

人物目光的追踪需要跟随一个 `LookTarget` 的 GameObject。而对于 `LookTarget` 的位置坐标的定位，我们采取了以下两种方式：

##### 2.1.2.1 基于鼠标指针

基于鼠标指针的跟随方式采用以下语句，获取鼠标指针的实时位置坐标：

```c#
Vector3 pos = Input.mousePosition;
```

##### 2.1.2.2 基于人脸位置

基于人脸位置的跟随方式采用OpenCVPlusUnity+haarcascade分类器的技术，需要安装OpenCVPlusUnity包，并使用用于人脸检测的haarcascade_frontalface_default.xml文件。其核心代码如下：

```c#
public Vector3 findNewFace(Mat frame)
{
    var faces = cascade.DetectMultiScale(frame);
    Vector3 pos = Input.mousePosition;
    if (faces.Length >= 1)
    {
        posX = faces[0].X - faces[0].Width / 2;
        posY = faces[0].Y - faces[0].Height / 2;
        //update sight target cordinates by proportion
        pos.x = centerX - (posX - centerRX) * proportion;
        pos.y = centerY - (posY - centerRY) * proportion * 0.6f;
    }
    return pos;
}
```

#### 2.1.3 MouthMovement

详细见 [2.5.1 Live2D.MouthMovement](#2.5.1 Live2D.MouthMovement)

#### 2.1.4 Raycasting

根据 [文档](https://docs.live2d.com/zh-CHS/cubism-sdk-manual/raycasting-unity/) 描述，我们首先需要创建一个用于接收击中图层的数组。接着我们需要从鼠标位置发射一条射线。检测击中目标中是否包含我们所期望的部位，以便做出反应。

>当多个网格在同一座标上重叠时，CubismRaycaster.Raycast()会获得CubismRaycastHit[]类型副本的元素数量。
>如果超过元素数量的网格重叠，将不会获得超出部分网格的结果。

```c#
cubismRaycastHits = new CubismRaycastHit[2];    // 射线可能击中多个图层
Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
int hitCount = cubismRaycaster.Raycast(ray,cubismRaycastHits);  // 发射射线
for(int i=0;i<hitCount;i++){
    String name = cubismRaycastHits[i].Drawable.name;
    switch(name){
        case "ArtMesh218" : // 手部
            TouchResponse(i);
            break;
        default : 
            break;
    }
}
```

#### 2.1.5 Expression

表情切换的功能只需修改 `Cubism Expression Controller` 的 `index` 值即可。

#### 2.1.6 Animator

动画控制可以通过创建 `Animation` 动画文件，将 `BlendShape` 也就是 Live2D 中的

### 2.2 大语言模型模块



### 2.3 AI 语音模块

#### 2.3.1 KWR 模块

Azure平台支持在线训练关键词模型(.table)，无需上传训练数据，只需要指定关键词，详见[官方文档](https://learn.microsoft.com/zh-cn/azure/cognitive-services/speech-service/custom-keyword-basics?pivots=programming-language-csharp)。

KWR由`Microsoft.CognitiveServices.Speech.SpeechRecognizer`类实现，详见[官方文档](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.cognitiveservices.speech.speechrecognizer?view=azure-dotnet)，主要使用其中的`StartKeywordRecognitionAsync(KeywordRecognitionModel)`函数进行关键词识别，该函数需要指定本地的关键词模型，在工作中会调用麦克风设备获取语音输入。

调用 `recognizer.Recognized += (s, e) =>{}` 事件接口，可以进行关键词判定和其他需要的后续操作。

更多信息见[官方关键词识别文档](https://learn.microsoft.com/zh-cn/azure/cognitive-services/speech-service/keyword-recognition-overview)。



#### 2.3.2 STT 模块

STT模块也由`Microsoft.CognitiveServices.Speech.SpeechRecognizer`类实现，详见[官方文档](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.cognitiveservices.speech.speechrecognizer?view=azure-dotnet)，主要使用其中的`StartContinuousRecognitionAsync()`函数进行语音转文字操作，该函数在工作中会调用麦克风设备获取语音输入。

调用 `recognizer.Recognized += (s, e) =>{}` 事件接口，可以在其中用`e.Result.Text`获取字符串格式的STT结果。

更多信息见[官方语音转文本文档](https://learn.microsoft.com/zh-cn/azure/cognitive-services/speech-service/index-speech-to-text)。



#### 2.3.3 TTS 模块

TTS模块也由`Microsoft.CognitiveServices.Speech.SpeechSynthesizer`类实现，详见[官方文档](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.cognitiveservices.speech.speechsynthesizer?view=azure-dotnet)，主要使用其中的`StartSpeakingSsmlAsync(String)`函数进行文本转语音操作，参数为本地SSML文件路径，之后用该函数的返回值创建[AudioDataStream](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.cognitiveservices.speech.audiodatastream?view=azure-dotnet)对象，可以用其`ReadData(Byte[])`函数读取语音数据。

如果需要播放生成的语音，可以用读取的语音数据创建Unity的[AudioClip](https://docs.unity3d.com/ScriptReference/AudioClip.html)对象，该对象可以被加载到Unity的[AudioSource](https://docs.unity3d.com/ScriptReference/AudioSource.html)对象的`clip`属性中，并用其`Play()`函数播放。



#### 2.3.4 Viseme 模块

详细见 [2.5.2 Azure.Viseme](#2.5.2 Azure.Viseme)



### 2.4 情感识别模块

对于文本情感倾向分析功能，采用当今为数不多可以统计多种情绪的API——[APISpace](https://www.apispace.com/eolink/api/wbqgfx/introduction)。该文本情感倾向分析 API可自动判断文本的情感极性类别，给出相应的置信度，情感极性分为积极、消极、中性。重要的是，其支持七种情绪统计，包含：好、乐、哀、怒、惧、恶、惊。其输出的JSON结构示例如下：

```c#
{
    "code": 200,
    "message": "success",
    "data": {
        "positive_prob": 0, //积极类别的概率
        "negative_prob": 0, //消极类别的概率
        "part_of_speech": [ //词性标注、分析
            ["这", "r"],
            ["真是", "d"],
            ["太", "d"],
            ["棒", "a"],
            ["了", "y"]
        ],
        "sentiments": 0.9005911035821151, //表示情感极性分类结果的概率
        "words": 3,
        "sentences": 2,	//表示情感极性分类结果，0:负向，1:中性，2:正向
        "好": 0,
        "乐": 0,
        "哀": 0,
        "怒": 0,
        "惧": 0,
        "恶": 0,
        "惊": 0
    }
}
```

情感识别模块调用API实现的核心代码如下：

```c#
string utf = HttpUtility.UrlEncode(content, Encoding.UTF8);
string serviceAddress = 
	"https://eolink.o.apispace.com/wbqgfx/api/v1/forward/sentiment_anls?text=" 
	+ utf;
HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
request.Method = "GET";
request.ContentType = "text/html;charset=UTF-8";
request.Headers.Add("X-APISpace-Token", APISpace_Token);
request.Headers.Add("Authorization-Type", "apikey");
HttpWebResponse response = (HttpWebResponse)request.GetResponse();
Stream myResponseStream = response.GetResponseStream();
StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
string retString = myStreamReader.ReadToEnd();
myStreamReader.Close();
myResponseStream.Close();
```

### 2.5 Audio2Face 模块

#### 2.5.1 Live2D.MouthMovement

MouthMovement 是 Cubism SDK for Unity 所提供的通过音频驱动人物模型说话口型的方法。但是根据 [文档](https://docs.live2d.com/zh-CHS/cubism-sdk-manual/mouthmovement-unity/) 描述，这只能驱动嘴巴的张开与闭合。

>   MouthMovement中唯一设置的是嘴巴的开合状态。
>   无法执行将嘴形与元音匹配等操作。

据我们观测，`Live2D.Cubism.Framework.MouthMovement` 所采用的口型驱动方式是基于音量的（有待考证），因为当我们播放一段纯音乐的时候人物也会张口。同时，该方法无法与元音进行匹配，无法表现说话时的口型。因此我们后来没有采取该方法作为Audio2Face模块的解决方案。

但 `Live2D.Cubism.Framework.MouthMovement` 是依旧是我们所知在 Live2D 模型上最简单的实现朴素口型同步方案。即使它的表现差强人意。

#### 2.5.2 Azure.Viseme

这是Microsoft Azure 提供的基于音素的口型同步方案，使用视位获取面部位置 。这里是它们的官方 [文档](https://learn.microsoft.com/zh-cn/azure/cognitive-services/speech-service/how-to-speech-synthesis-viseme?tabs=3dblendshapes) 。

##### 2.5.2.1 接口调用

调用 `synthesizer.VisemeReceived += (s, e) =>{}` 事件接口，以获得 `e.Animation` 帧对齐BlendShape权重数组。

##### 2.5.2.2 封装

如文档所示 `e.Animation` 返回的结果为 Json 格式

```json
{
    "FrameIndex":0,
    "BlendShapes":[
        [0.021,0.321,...,0.258],
        [0.045,0.234,...,0.288],
        ...
    ]
}
```

因此我们需要封装一个相同格式的实体类来接收 Json 反序列化的结果

```c#
public class BlendShapeEntity{
    public int FrameIndex { get; set; }
    public float[][] BlendShapes { get; set; }
}

blendShapeEntity = JsonConvert.DeserializeObject<BlendShapeEntity>(e.Animation);
```

由于异步操作，不会一次返回所有数据，因此我们需要使用一个队列来缓存数据。

```c#
for (int i = 0; i < blendShapeEntity.FrameIndex; i++){
    blendShapeQueue.Enqueue(blendShapeEntity.BlendShapes[i]);
}
```

##### 2.5.2.3 每帧读取数据

由于 Cubism 的特性，只能在 `LateUpdate()` 中对模型进行操作。

>在Live2D Cubism SDK for Unity中，动画播放使用Unity的内置功能Animator、Playable API，这些功能在MonoBehaviour.Update()和MonoBehaviour.LateUpdate()之间应用参数值。
>因此，如果使用MonoBehaviour.Update()设置参数值，根据执行顺序，该值可能会被动画覆盖。

在 TTS 合成好音频准备播放时，我们每帧从 ` blendShapeQueue` 队列中拿出一组值使用。

```c#
private void LateUpdate() {
    if(f_IsAudioPlaying){
        if(azureSpeech.blendShapeQueue.Count>0){
            var blendShapeList = azureSpeech.blendShapeQueue.Dequeue();
            // ...
        }
    }
}
```

##### 2.5.2.4 对齐绑定

`Azure.Viseme` 提供了每帧55个BlendShapes权重。

>`BlendShapes` 中的每个帧都包含 55 个面部位置的数组，这些位置表示为 0 到 1 之间的十进制值。 十进制值的顺序与下面的面部位置表中所述的顺序相同。

>| Order | BlendShapes参数 |
>| :---- | :-------------- |
>| 1     | eyeBlinkLeft    |
>| 2     | eyeLookDownLeft |
>| 3     | eyeLookInLeft   |
>| ...   | ...             |
>| 55    | rightEyeRoll    |

但很遗憾，项目中使用的 Live2D 模型所支持的 BlendShape 中与口型相关的只有5个值。而且经过测试发现其中可以将 `Azure BlendShapes` 有效映射到本项目模型  `CubismParameter` 的 BlendShape 参数仅有3个。由此可以见，剩下的52个 `Azure BlendShapes` 均被丢弃。这很可惜，因为越多有效的 `BlendShapes` 参数被绑定，就意味着模型的面部口型效果表现将越好。所以如果你的模型支持更多的面部 `BlendShapes` 参数，那么你会获得优于本项目的口型表现效果。即便如此，在仅有三个参数的情况下，该解决方案的表现也更优于 `Live2D.MouthMovement` 的方案。本项目能明显感受到在发音包含韵母 `o`、`u`、`ü` 等可以触发 `mouthPucker` 权重的O型嘴型时，有较为显著的效果。

在上一步中我们获得了每一帧的55个 `Azure BlendShapes` 权重，但我们最终留下了3个有效值。但这三个值的索引与 `CubismParameter` 对应的索引并非一定是对齐的。因此在这一步我们需要手动将他们对齐绑定。

```c#
// 绑定 CubismParameter 索引
mouthOpenYParam = cubismModel.Parameters[38];
cheekPuffParam = cubismModel.Parameters[18]; 
mouthFormParam = cubismModel.Parameters[37];

// 倍率
float rate = 1.5f;
// 参数映射处理
float jawOpenAzure = blendShapeList[17] * rate;
float mouthPuckerAzure = blendShapeList[20] * rate;
float mouthFunnelAzure = -(blendShapeList[19] * 2f - 1) * rate; // 0~1 映射到 1~-1

// BlendShape Azure -> Live2D 对齐
if(jawOpenAzure > 0.2f){ // 阈值
    mouthOpenYParam.Value = jawOpenAzure;
}else{
    mouthOpenYParam.Value = 0;
}
cheekPuffParam.Value = mouthPuckerAzure;
mouthFormParam.Value = mouthFunnelAzure;
```

为了让其效果更加明显，我们将BlendShape权重乘以1.5倍。该系数需要根据不同的模型进行调整。同时，为了避免噪声。我们将嘴巴张开闭合的参数设置了启动阈值，以此来过滤掉极小但没有被判定为0的值。

##### 2.5.2.5 同步帧率

由于 `Azure.Viseme` 输出的 BlendShape 数组是按60帧计算的。因此我们需要将游戏设置为60帧运行才能保证音画同步。

## 3 非功能模块

### 3.1 持久化

我们封装了用于持久化的 `GameSettingsEntity` 类，他包括了需要长期存储的必要游戏设置，并且我们将它做成了线程安全的单例。在它的构造函数中，我们在可读可写的 `Application.persistentDataPath` 路径下创建或读取 Json 格式的本地持久化文件，并在其他模块初始化时读取该类。
