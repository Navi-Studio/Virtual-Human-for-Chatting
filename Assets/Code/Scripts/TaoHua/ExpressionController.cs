using System.Collections;
using System.Collections.Generic;
using Live2D.Cubism.Framework.Expression;
using UnityEngine;

public class ExpressionController : MonoBehaviour
{
    private CubismExpressionController expressionController;
    // Start is called before the first frame update
    void Start()
    {
        expressionController = GameObject.FindGameObjectWithTag("Model").GetComponent<CubismExpressionController>();
    }

    public void RestoreDefaultExpression(){
        expressionController.CurrentExpressionIndex = -1;
    }

    public void setCurrentExpression(string style){
        switch (style)
        {
            case "customerservice": expressionController.CurrentExpressionIndex = -1; break;
            case "cheerful": expressionController.CurrentExpressionIndex = 10; break;
            case "sad": expressionController.CurrentExpressionIndex = 8; break;
            case "angry": expressionController.CurrentExpressionIndex = 7; break;
            case "fearful": expressionController.CurrentExpressionIndex = 0; break;
            case "disgruntled": expressionController.CurrentExpressionIndex = 7; break;
            case "serious": expressionController.CurrentExpressionIndex = 0; break;
            default: expressionController.CurrentExpressionIndex = -1; break;
        }
    }
}
