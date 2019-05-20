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
{/// <summary>
/// 
/// </summary>
    public class MainMapUI : UnitySingleton<MainMapUI>
    {
        /// <summary>测试用
        /// 
        /// </summary>
        public GameObject mapcamera;
        GameObject TestUI;
        #region 大地图FairyGUI素材包
        private string MainMapUIPackage = "MainMapFairyGUIPackage/MainMapUI";
        private string CardCollectionPackage = "MainMapFairyGUIPackage/CardCollection";
        private string CardIconPackage = "MainMapFairyGUIPackage/CardIcon";
        private string LibraryPackage = "MainMapFairyGUIPackage/Library";
        #endregion
        #region 大地图的GCompoment 和window
        private GComponent mainmapUI;
        private GComponent libraryUI;
        private GComponent cardcollectUI;
        private GComponent verifyUI;
        private Window cardcollect_UI;
        private Window library_UI;
        private Window verify_UI;
        private GComponent _cardDisplayer;
        #endregion
        #region 按钮，文本,装载器等
        private GButton ccbtn;
        private GButton buybtn;
        private GButton cancelbtn;
        //private GButton closebtn;
        private GLoader _cardloader;
        private GList cardcollectionlist;
        private GList onsalelist;
        private GTextField _abstractText;
        private GTextField _storyText;
        private GLoader _iconLoader;
        private GLoader _picLoader;
        #endregion
        /// <summary>对CardCollection的引用
        /// //Todo:为啥这么写呢。。？
        /// 
        /// </summary>
        private List<string> playercardlist;
        private List<string> library_list;
        //URL
        private const string cardicons = "CardIcon";
        /// <summary>初始化
        /// 
        /// </summary>
        private void Awake()
        {
            GRoot.inst.SetContentScaleFactor(960, 540);
            UIPackage.AddPackage(MainMapUIPackage);
            UIPackage.AddPackage(CardCollectionPackage);
            UIPackage.AddPackage(CardIconPackage);
            UIPackage.AddPackage(LibraryPackage);
            mainmapUI = UIPackage.CreateObject("MainMapUI", "Component2").asCom;
            cardcollectUI = UIPackage.CreateObject("CardCollection", "CardBookMain").asCom;
            cardcollect_UI = new Window();
            cardcollect_UI.contentPane = cardcollectUI;
            cardcollect_UI.CenterOn(GRoot.inst, true);
            libraryUI = UIPackage.CreateObject("Library", "LibraryMain").asCom;
            library_UI = new Window();
            library_UI.contentPane = libraryUI;
            library_UI.CenterOn(GRoot.inst, true);
            verifyUI = UIPackage.CreateObject("Library", "verifyUI").asCom;
            verify_UI = new Window();
            verify_UI.contentPane = verifyUI;
            verify_UI.CenterOn(GRoot.inst, true);
            GRoot.inst.AddChild(mainmapUI);
            #region 初始化按钮装载器文本等
            ccbtn = mainmapUI.GetChild("CardCollectionBtn").asButton;
            ccbtn.onClick.Add(() => ShowCardCollect());
            cardcollectionlist = cardcollectUI.GetChild("cardList").asList;
            onsalelist = libraryUI.GetChild("LibraryCardList").asList;
            _cardDisplayer = cardcollectUI.GetChild("cardDisplayer").asCom;
            _abstractText = _cardDisplayer.GetChild("abstractText").asTextField;
            _storyText = _cardDisplayer.GetChild("storyText").asTextField;
            _iconLoader = _cardDisplayer.GetChild("iconLoader").asLoader;
            _picLoader = _cardDisplayer.GetChild("cardPicLoader").asLoader;
            #endregion
            Debug.Log("ui初始化");
            
        }
        private void Start()
        {
            playercardlist = CardCollection.Instance().mycollection;
            library_list = CardCollection.Instance().librarylist;
            cardcollectionlist.onClickItem.Add(OnClickCardInCardCollection);
            onsalelist.onClickItem.Add(OnClickCardInLibrary);
            CardCollection.Instance().GetCards();
        }
        #region 图书馆相关代码
        /// <summary>点击图书馆地格后调用此方法展示图书馆UI并作初始化工作;
        /// 
        /// </summary>
        public void ShowlibraryUI(Library library)
        {
            mapcamera.SetActive(false);
            library_UI.Show();
            ShowCard();
            GButton closebtn = library_UI.contentPane.GetChild("Close").asButton;
            closebtn.onClick.Add(() => LeaveOnClick());
        }    
        /// <summary>点击传送时，调用此方法
        /// 
        /// </summary>
        public  void TransOnClick()
        {
            Library.PrepareTrans();
        }
        /// <summary>点击离开时，调用此方法
        /// 
        /// </summary>
        public  void LeaveOnClick()
        {
            library_UI.Hide();
            verify_UI.Hide();
            mapcamera.SetActive(true);
        }
        /// <summary>展示三张卡牌，
        /// 
        /// </summary>
        public  void ShowCard()
        {
            onsalelist.RemoveChildren();
            foreach (string cardID in library_list)
            {
                GObject item = UIPackage.CreateObject("Library", "CardItem");
                item.icon = UIPackage.GetItemURL(cardicons, cardID);
                onsalelist.AddChild(item);
            }
        }
        /// <summary>图书馆内卡牌的点击事件
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnClickCardInLibrary(EventContext context)
        {
            CardCollection.Instance().choosecardindex = onsalelist.GetChildIndex(context.data as GObject);
            CardCollection.Instance().choosecardID = library_list[CardCollection.Instance().choosecardindex];
            verify_UI.Show();
            buybtn = verify_UI.contentPane.GetChild("buybtn").asButton;
            cancelbtn = verify_UI.contentPane.GetChild("cancelbtn").asButton;
            buybtn.onClick.Add(BuyOnclick);
            cancelbtn.onClick.Add(CancelOnclick);

        }
        /// <summary>购买按钮点击事件
        /// 
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="index"></param>
        public void BuyOnclick()
        {
            CardCollection.Instance().BuyCard();
            onsalelist.RemoveChildAt(CardCollection.Instance().choosecardindex, true);
            verify_UI.Hide();
            
        }
        /// <summary>取消按钮点击事件
        /// 
        /// </summary>
        public void CancelOnclick()
        {
            Debug.Log("取消购买");
            buybtn.onClick.Remove(BuyOnclick);
            verify_UI.Hide();
        }
        #endregion
        #region 卡牌书相关代码
        /// <summary>展示卡牌收藏
        /// 
        /// </summary>
        public void ShowCardCollect()
        {
            cardcollect_UI.Show();
            Debug.Log("展示卡牌收藏");
            //TODO：隐藏地格渲染
            mapcamera.SetActive(false);
            cardcollectionlist.RemoveChildren(0, -1, true);

            foreach (string cardId in playercardlist)
            {
                GObject item = UIPackage.CreateObject("CardCollection", "cardsSetsItem");
                item.icon = UIPackage.GetItemURL(cardicons, cardId);
                cardcollectionlist.AddChild(item);
            }
           GButton closebtn = cardcollect_UI.contentPane.GetChild("Close").asButton;
           closebtn.onClick.Add(() => CloseCardCollect());

        }
        /// <summary>关闭卡牌收藏
        /// 
        /// </summary>
        public void CloseCardCollect()
        {
            cardcollect_UI.Hide();
            mapcamera.SetActive(true);
            //TODO：显示地格渲染
            Debug.Log("卡牌收藏关闭");
        }
        /// <summary>
        /// 响应卡牌书内卡牌点击事件
        /// </summary>
        /// <param name="context"></param>
        public void OnClickCardInCardCollection(EventContext context)
        {
            // 先获取到点击的下标
            int index = cardcollectionlist.GetChildIndex(context.data as GObject);

            // 通过下标获取到id
            string cardId = playercardlist[index];
            // 向数据库查询展示数据
            JsonData data = CardManager.Instance().GetCardJsonData(cardId);

            _abstractText.text = "姓名：" + data["name"] + "\n" + "类型：" + data["type"];

            _storyText.text = "这里本来该有卡牌故事但是现在没有数据\n" + data["effect"];

            // TODO: 根据策划案加载icon

            _picLoader.url = UIPackage.GetItemURL(cardicons, cardId);

        }
        #endregion
    }
}

