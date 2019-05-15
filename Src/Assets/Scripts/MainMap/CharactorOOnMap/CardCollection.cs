using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using GameCard;

/// <summary>
/// 管理角色的卡牌收藏
/// 
/// 
/// </summary>
public class CardCollection : UnitySingleton<CardCollection> {
    /// <summary>
    /// 角色的卡牌收藏
    /// </summary>
    public Dictionary<string, JsonData> mycollection = new Dictionary<string, JsonData>();
    /// <summary>
    /// 卡牌数据库的json文件
    /// </summary>
    JsonData cardsJsonData =  JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/Cards/cardSample.1.json"));
    /// <summary>
    /// 通过卡牌ID向收藏中添加卡牌时调用，添加成功返回true
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool AddCard(string ID)
    {
        mycollection.Add(name, CardManager.Instance().GetCardJsonData(ID));
        return true;
    }
    public bool BuyCard(string ID)
    {

        return true;
    }
}
