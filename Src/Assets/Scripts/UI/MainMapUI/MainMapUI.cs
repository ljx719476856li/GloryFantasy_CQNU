using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainMap;
using GameCard;
using UnityEngine.UI;
using LitJson;
using System.IO;
using FairyGUI;

namespace GameGUI
{/// <summary>以下所有代码都是为了测试和演示，所以会略草率= = 
/// 
/// </summary>
    public class MainMapUI : UnitySingleton<MainMapUI>
    {
       // public Button CardCollection;
        public GameObject TestUI;
        public GComponent mainmapUI;
        public Window cardcollect_UI;
        public GComponent cardcollectUI;
        public GameObject postUI;
        public GButton ccbtn;
        public GButton closebtn;
        public GameObject Map;


        private void Awake()
        {
            GRoot.inst.SetContentScaleFactor(960, 540);
            UIPackage.AddPackage("MainMapFairyGUIPackage/MainMapUI");
            mainmapUI = UIPackage.CreateObject("MainMapUI", "Component2").asCom;
            cardcollectUI = UIPackage.CreateObject("MainMapUI", "Component4").asCom;
            GRoot.inst.AddChild(mainmapUI);
            ccbtn = mainmapUI.GetChild("CardCollectionBtn").asButton;
            ccbtn.onClick.Add(() => ShowCardCollect());
            Map = GameObject.FindGameObjectWithTag("Map");
            cardcollect_UI = new Window();
            cardcollect_UI.contentPane=cardcollectUI;
            cardcollect_UI.CenterOn(GRoot.inst, true);
            Debug.Log("ui初始化");
        }  
        /// <summary>点击驿站地格后调用此方法展示驿站UI;
        /// 
        /// </summary>
        public  void ShowPostUI(Post post)
        {
            postUI.SetActive(true);
        }
        public  void DealOnClick()
        {
            GetCards();
        }
        /// <summary>获取三张卡牌信息
        /// 
        /// </summary>
        public  void GetCards()
        {
            JsonData cardsJsonData =
            JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/Cards/cardSample.1.json"));
            //因为赶时间写的超烂，以后会改0.0
            int dataAmount = cardsJsonData.Count;
            int num1 = Random.Range(0, dataAmount);
            int num2 = Random.Range(0, dataAmount);
            int num3 = Random.Range(0, dataAmount);
          //  JsonData Card1 = CardManager.Instance().GetCardJsonData(cardsJsonData[num1]["id"].ToString());
          //  JsonData Card2 = CardManager.Instance().GetCardJsonData(cardsJsonData[num2]["id"].ToString());
          //  JsonData Card3 = CardManager.Instance().GetCardJsonData(cardsJsonData[num3]["id"].ToString());
            ShowCard(cardsJsonData[num1]["id"].ToString(), cardsJsonData[num2]["id"].ToString(), cardsJsonData[num3]["id"].ToString());
        }
        /// <summary>点击传送时，调用此方法
        /// 
        /// </summary>
        public  void TransOnClick()
        {
            Post.PrepareTrans();
        }
        /// <summary>点击离开时，调用此方法
        /// 
        /// </summary>
        public  void LeaveOnClick()
        {
            postUI.SetActive(false);
        }
        /// <summary>展示三张卡牌，
        /// 
        /// </summary>
        /// <param name="card1"></param>
        /// <param name="card2"></param>
        /// <param name="card3"></param>
        public  void ShowCard(string card1,string card2,string card3)
        {
            //TODO:在ui上显示三张卡牌的信息
            Debug.Log("ThreeCards:" + card1 + "," + card2 + "," + card3);


        }
        /// <summary>展示卡牌收藏
        /// 
        /// </summary>
        public void ShowCardCollect()
        {
            Debug.Log("展示卡牌收藏");
            //TODO：ui调用setactive会导致性能问题，后期要改其他实现方式！！2019.5.8
            Map.SetActive(false);
            cardcollect_UI.Show();
            closebtn = cardcollect_UI.contentPane.GetChild("Close").asButton;
            closebtn.onClick.Add(() => CloseCardCollect());

        }
        /// <summary>关闭卡牌收藏
        /// 
        /// </summary>
        public void CloseCardCollect()
        {
            cardcollect_UI.Hide();
            Map.SetActive(true);
            //TODO：ui调用setactive会导致性能问题，后期要改其他实现方式！！2019.5.8
            Debug.Log("卡牌收藏关闭");
        }
    }
}

