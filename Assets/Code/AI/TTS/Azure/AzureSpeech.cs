using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class AzureSpeech : MonoBehaviour
{
   public AudioSource audioSource;

    // 服务所在区域
    private string Region = "eastus";
    // 设置声音的角色码
    // private string m_SoundSetting; // zh-CN-XiaoxiaoNeural(温柔) zh-CN-XiaoyiNeural(可爱)

    private const int SampleRate = 24000;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    private string message;

    private SpeechConfig speechConfig;
    private AudioConfig audioConfig;
    private SpeechSynthesizer synthesizer;
    private Microsoft.CognitiveServices.Speech.SpeechRecognizer recognizer;

    // 关键词识别
    private string kwsModelDir = "/";
    private string kwsModelFile = "TaoHua.table";
    private const string keyword = "桃花";

    // 主线程
    private SynchronizationContext mainThreadSynContext;
    private Animator animator;
    [HideInInspector]public AudioClip m_AudioClip;
    [SerializeField] private ExpressionController expressionController;

    // Audio2Face
    private BlendShapeEntity blendShapeEntity = new BlendShapeEntity();
    [HideInInspector] public Queue<float[]> blendShapeQueue = new Queue<float[]>();


    // 事件
    [HideInInspector] public static event Action OnSpeechStart;
    [HideInInspector] public static event Action OnSpeechEnd;

    void Start(){
        // 注册 OnGameSettingChanged 事件
        GameSettingsEvent.OnGameSettingChanged += SetAzureSettings;

        m_AudioClip = null;

        // 记录主线程
        mainThreadSynContext = SynchronizationContext.Current;
        // 初始化动画控制器组件
        animator = GameObject.FindGameObjectWithTag("Model").GetComponent<Animator>();

        // Creates an instance of a speech config with specified subscription key and service region.
        SetSpeechConfig();

        // STT配置
        var audioProcessingOptions = AudioProcessingOptions.Create(AudioProcessingConstants.AUDIO_INPUT_PROCESSING_DISABLE_DEREVERBERATION);
        audioConfig = AudioConfig.FromDefaultMicrophoneInput(audioProcessingOptions);

        
    }
    
    public void SetAzureSettings(){
        SetSpeechConfig();
    }

    public void SetSpeechConfig(){
        speechConfig = SpeechConfig.FromSubscription(GameSettingsEntity.Instance.AzureAPI, Region);
        speechConfig.SpeechRecognitionLanguage = "zh-CN";
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);
        speechConfig.SpeechSynthesisVoiceName = GameSettingsEntity.Instance.Speaker;
        speechConfig.EnableDictation();
        speechConfig.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs, "2000");
        speechConfig.SetProperty(PropertyId.SpeechServiceConnection_InitialSilenceTimeoutMs, "6000");
    }


    public void SsmlGeneration(TTSEntity _input)
    {
        // #if UNITY_EDITOR    // 调试时
        //     var ssmlFile = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Audios/" + _input.SsmlFile;
        // #else
            var ssmlFile = Application.persistentDataPath + "/" + _input.SsmlFile;
        // #endif
        
        if (System.IO.File.Exists(ssmlFile))
        {
            System.IO.File.Delete("ssmlFile");
        }

        var lang = "zh-CN";
        // var voice = _input.Speaker;
        var voice = GameSettingsEntity.Instance.Speaker;
        try
        {
            var ssmlContent = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\"";
            ssmlContent += " xmlns:mstts=\"https://www.w3.org/2001/mstts\"";
            ssmlContent += ($" xml:lang=\"{lang}\"");
            ssmlContent += ">";
            ssmlContent += $"<voice name=\"{voice}\"><mstts:viseme type=\"FacialExpression\"/>\n"; ;

            for (int i = 0; i < _input.Content.Count; i++)
            {
                // 情绪、咬字
                ssmlContent += $"<mstts:express-as style=\"{_input.Style[i]}\" styledegree=\"{Math.Round(_input.StyleDegree[i], 1)}\">";
                // 音量
                ssmlContent += $"<prosody volume=\"{Math.Round(_input.ProsodyVolume[i], 1)}\"";
                // 语速
                ssmlContent += $" rate=\"{Math.Round(_input.ProsodyRate[i], 1)}\">";
                // 内容
                ssmlContent += _input.Content[i];
                // 停顿
                ssmlContent += $"<break strength=\"{_input.Break[i]}\" />";
                ssmlContent += "</prosody></mstts:express-as>";
            }
            ssmlContent += "</voice></speak>";
            // print(ssmlContent);
            // 

            System.IO.File.WriteAllText(ssmlFile, ssmlContent);
        }
        catch (IOException ee) { }
    }

    // 合成语音并播放
    public void TurnTextToSpeech(TTSEntity _input)
    {
        if(audioSource.isPlaying){
            audioSource.Stop();
        }

        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        // 生成ssml文件并读取
        SsmlGeneration(_input);
        // #if UNITY_EDITOR    // 调试时
        //     var ssmlFile = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Audios/" + _input.SsmlFile;
        // #else
            var ssmlFile = Application.persistentDataPath + "/" + _input.SsmlFile;
        // #endif
        string ssml = System.IO.File.ReadAllText(ssmlFile, Encoding.UTF8);

        // 控制表情
        expressionController.setCurrentExpression(_input.Style[0]);

        // 生成语音
        TurnTextToSpeechFromSSML(ssml);
    }

    // 从SSML合成语音并播放
    public async Task TurnTextToSpeechFromSSML(string ssml)
    {
        string newMessage = null;
        var startTime = DateTime.Now;
        synthesizer = new SpeechSynthesizer(speechConfig, null);

        synthesizer.SynthesisStarted += (s, e) =>
        {
            blendShapeQueue.Clear();
            // Debug.Log("SynthesisStarted");
        };
        
        // Audio2face
        synthesizer.VisemeReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Animation))
            {
                // Debug.Log(e.Animation);
                blendShapeEntity = JsonConvert.DeserializeObject<BlendShapeEntity>(e.Animation);
                blendShapeEntity.FrameIndex = blendShapeEntity.BlendShapes.Length;
                for (int i = 0; i < blendShapeEntity.FrameIndex; i++)
                {
                    blendShapeQueue.Enqueue(blendShapeEntity.BlendShapes[i]);
                }
            }
        };

        var result = await synthesizer.SpeakSsmlAsync(ssml);

        // Starts speech synthesis, and returns once the synthesis is started.
        {
            int audioLength = result.AudioDuration.Seconds + 1;
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            bool isFirstFinish = true;
            m_AudioClip = null;
            var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * audioLength, // 设置播放时长
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                        isFirstAudioChunk = false;
                    }

                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }

                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); // Leave some time for the audioSource to finish playback

                        if (isFirstFinish)
                        {
                            // Finish
                            mainThreadSynContext.Post(new SendOrPostCallback(OnPlayEnd), null);//通知主线程
                            isFirstFinish = false;
                        }

                    }
                });

            m_AudioClip = audioClip;
            // 开始播放
            StartCoroutine(OnPlayStartIE());
        }

        lock (threadLocker)
        {
            if (newMessage != null)
            {
                message = newMessage;
            }

            waitingForSpeak = false;
        }
    }

    IEnumerator OnPlayStartIE()
    {
        // yield return new WaitForSeconds(0.1f);
        yield return null;
        Debug.Log("Speech start!");
        OnSpeechStart?.Invoke();  // 调用通知事件
        Audio2Face.f_IsAudioPlaying = true;
        animator.SetBool("isSpeaking", true);
        animator.SetInteger("idleState", -1);
        audioSource.clip = m_AudioClip;
        audioSource.Play();
    }
    private void OnPlayEnd(object state){
        StartCoroutine(OnPlayEndIE());
    }
    IEnumerator OnPlayEndIE()
    {
        // yield return new WaitForSeconds(0.1f);
        yield return null;
        OnSpeechEnd?.Invoke();  // 调用通知事件
        Audio2Face.f_IsAudioPlaying = false;
        Debug.Log("Speech end!");
        blendShapeQueue.Clear();
        animator.SetBool("isSpeaking", false);
        animator.SetInteger("idleState", 0);
        expressionController.RestoreDefaultExpression();
    }

    [HideInInspector] public bool m_KeyWordResult = false;
    public async Task KeyWordRecognition(){
        string kwsModelDir = Application.dataPath + $"/Resources/Audios/KwrModel/{kwsModelFile}";
        var model = KeywordRecognitionModel.FromFile(kwsModelDir);
        var stopRecognition = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var resultStr = "";

        recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(speechConfig, audioConfig);
        // 事件
        recognizer.Recognized += (s, e) =>  // recognizer.Recognized 识别结果
        {
            if (e.Result.Reason == ResultReason.RecognizedKeyword)
            {
                m_KeyWordResult = true;
                resultStr = $"RECOGNIZED KEYWORD: '{e.Result.Text}'";
            }
            else if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                resultStr = $"RECOGNIZED: '{e.Result.Text}'";
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                resultStr = "NOMATCH: Speech could not be recognized.";
            }
            Debug.Log(resultStr);
        };
        recognizer.Canceled += (s, e) => // 识别出错并取消
        {
            var cancellation = CancellationDetails.FromResult(e.Result);
            resultStr = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
            stopRecognition.TrySetResult(0);
        };

        recognizer.SessionStopped += (s, e) =>
        {
            // Debug.Log("\nKWR Session stopped event.");
            stopRecognition.TrySetResult(0);
        };

        // 关键词识别
        await recognizer.StartKeywordRecognitionAsync(model).ConfigureAwait(false);  

        // Waits for a single successful keyword-triggered speech recognition (or error).
        // Use Task.WaitAny to keep the task rooted.
        Task.WaitAny(new[] { stopRecognition.Task });

        await recognizer.StopKeywordRecognitionAsync().ConfigureAwait(false);
    }

    [HideInInspector] public string m_STTResult = "";
    public async Task TurnSpeechToText()
    {
        bool hasText = false;
        var stopRecognition = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(speechConfig, audioConfig);
        // 事件
        recognizer.Recognized += (s, e) =>
        {
            Debug.Log("Recognize Text = " + e.Result.Text);
            if (!hasText) m_STTResult = e.Result.Text;
            hasText = true;
            stopRecognition.TrySetResult(0);
        };
        recognizer.Canceled += (s, e) =>
        {
            var cancellation = CancellationDetails.FromResult(e.Result);
            var resultStr = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
            stopRecognition.TrySetResult(0);
        };

        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

        Task.WaitAny(new[] { stopRecognition.Task });

        await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
    }
}
