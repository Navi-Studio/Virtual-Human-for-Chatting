using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

public class SAService
{
    private string APISpace_Token;
    public SAService()
    {
        APISpace_Token = GameSettingsEntity.Instance.APISpaceAPI;
        // 注册 OnGameSettingChanged 事件
        GameSettingsEvent.OnGameSettingChanged += SetSASettings; 
    }

    public void SetSASettings(){
        APISpace_Token = GameSettingsEntity.Instance.APISpaceAPI;
    }
    

    public SAEntity sentimentAnalysis(string msg)
    {
        try
        {
            string content = msg.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
            string utf = HttpUtility.UrlEncode(content, Encoding.UTF8);
            string serviceAddress = "https://eolink.o.apispace.com/wbqgfx/api/v1/forward/sentiment_anls?text=" + utf;
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

            int start_positive = retString.IndexOf("positive_prob") + 16;
            int start_negative = retString.IndexOf("negative_prob") + 16;
            int start_sentiments = retString.IndexOf("sentiments") + 13;
            int start_Sentences = retString.IndexOf("sentences") + 12;
            int start_Style = start_Sentences + 3;
            string positive_prob = retString.Substring(start_positive, retString.IndexOf(",", start_positive) - start_positive);
            string negative_prob = retString.Substring(start_negative, retString.IndexOf(",", start_negative) - start_negative);
            string sentiments = retString.Substring(start_sentiments, retString.IndexOf(",", start_sentiments) - start_sentiments);
            string sentences = retString.Substring(start_Sentences, 1);
            string style_hao = retString.Substring(start_Style + 5, 1);
            string style_le = retString.Substring(start_Style + 13, 1);
            string style_ai = retString.Substring(start_Style + 21, 1);
            string style_nu = retString.Substring(start_Style + 29, 1);
            string style_ju = retString.Substring(start_Style + 37, 1);
            string style_e = retString.Substring(start_Style + 45, 1);
            string style_jing = retString.Substring(start_Style + 53, 1);
            SAEntity sAEntity = new SAEntity();
            sAEntity.Positive_prob = Convert.ToDouble(positive_prob);
            sAEntity.Negative_prob = Convert.ToDouble(negative_prob);
            sAEntity.Sentiments = Convert.ToDouble(sentiments);
            sAEntity.Sentences = Convert.ToInt32(sentences);
            if (style_hao == "1" || style_le == "1"/* || sAEntity.Sentences == 2*/)
            {
                sAEntity.Style = "cheerful";
            }
            else if (style_ai == "1")
            {
                sAEntity.Style = "sad";
            }
            else if (style_nu == "1")
            {
                sAEntity.Style = "angry";
            }
            else if (style_ju == "1")
            {
                sAEntity.Style = "fearful";
            }
            else if (style_e == "1")
            {
                sAEntity.Style = "disgruntled";
            }
            else if (style_jing == "1")
            {
                sAEntity.Style = "serious";
            }
            else
            {
                sAEntity.Style = "customerservice";
            }
            return sAEntity;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception();
        }
        
    }
}