using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SAEntity
{
    // 表示积极类别的概率
    public double Positive_prob { get; set; }

    // 表示消极类别的概率
    public double Negative_prob { get; set; }

    // 表示情感极性分类结果的概率
    public double Sentiments { get; set; }

    // 表示情感极性分类结果，0:负向，1:中性，2:正向
    public int Sentences { get; set; }

    // 表示情感极性分类结果
    public string Style { get; set; }

}
