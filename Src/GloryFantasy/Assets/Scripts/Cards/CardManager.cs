using System;
using System.IO;
using IMessage;
using UnityEngine;
using System.Collections.Generic;
using GameGUI;
using GamePlay;
using GamePlay.FSM;
using GameUnit;
using LitJson;
using Random = UnityEngine.Random;

namespace GameCard
{
    public class CardManager : UnitySingleton<CardManager>, MsgReceiver, GameplayTool
    {
        #region 变量
        // 空物体的引用
        public GameObject emptyObject;
        public int cardsUpperLimit;                   // 手牌数量上限
        public int extractCardsUpperLimit;            // 抽牌数量上限
        public bool cancelCheck;                      // 是否取消抽卡检查，在本行注释存在的情况下请不要修改值

        private List<string> _handcards;               // 存储手牌的ID
        private List<string> _cardsSets;               // 牌组堆，待抽取的卡牌组
        private List<string> _cardOnMap;                // 场上卡牌记录列表
        private List<cdObject> _cooldownCards;           // 临时存储冷却状态中卡牌
        private List<string> _garbageCards;            //弃牌堆
        private GameUnit.GameUnit _latestDeadUnit;      // 最近死掉的单位

        // 存储json格式卡牌数据的字典
        private Dictionary<string, JsonData> _cardsData;
        
        // 存放手牌实体的list
        private List<GameObject> _handcardsInstance;

        private string _currentSelectingCard;        // 当前进入选中状态的手牌的ID
        private int _currentSelectingPos;            // 当前进入选中状态的手牌的位置

        #endregion
        
        #region 变量可见性定义
        public List<string> cardsInHand { get { return _handcards; } }
        public List<string> cardsSets { get { return _cardsSets; } }
        public List<cdObject> cooldownCards { get { return _cooldownCards; } }
        public List<GameObject> handcardsInstance { get { return _handcardsInstance; } }

        /// <summary>
        /// 当前手牌中选中的卡牌的实例，若没有选中卡牌则返回null
        /// </summary>
        public GameObject currentSelectingCardInstance
        {
            get
            {
                if (_currentSelectingCard == null || _currentSelectingPos < 0)
                    return null;
                return _handcardsInstance[_currentSelectingPos];
            }
        }
        
        /// <summary>
        /// 获取当前选中卡牌的id,若没有选中卡牌则返回null
        /// </summary>
        public string currentSelectingCardId { get { return _currentSelectingCard; } }

        #endregion
        
        private void Awake()
        {
            Init();
            LoadCardsIntoSets();
            cancelCheck = false;
        }

        private void Start()
        {
            // 注册函数响应抽牌信息
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.DrawCard,
                canDoExtractAction,
                ()=> { ExtractCards(); },
                "Extract cards Trigger"
            );
            
            // 注册函数响应回合结束信息
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.EP,
                canDoCoolDownAction,
                HandleCooldownEvent,
                "Cooldown cards Trigger"
            );
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.Dead,
                canSendToCoolDownList,
                HandleCardOnMapToCooldown,
                "Map Unit cooldown Trigger"
             );
            
            ExtractCards(3);
        }

        /// <summary>
        /// 类内变量初始化工作
        /// </summary>
        private void Init()
        {
            _handcards = new List<string>();
            _cardsSets = new List<string>();
            _cooldownCards = new List<cdObject>();
            _garbageCards = new List<string>();
            _cardOnMap = new List<string>();
            
            _handcardsInstance = new List<GameObject>();
            
            _cardsData = new Dictionary<string, JsonData>();

            cardsUpperLimit = 7;
            extractCardsUpperLimit = 1;

            _currentSelectingCard = null;
            _currentSelectingPos = -1;
            
            InitCardsData();
            
        }

        /// <summary>
        /// 用于设置当前选中的卡牌
        /// </summary>
        /// <param name="cardPos"></param>
        public void SetSelectingCard(int cardPos)
        {
            // 当手牌数量大于cardPos时，可以正常设置卡牌位置，因为此时Count是上界，永远不等于手牌最大位置下标
            if (cardPos >= 0 && _handcards.Count > cardPos)
            {
                _currentSelectingPos = cardPos;
                _currentSelectingCard = _handcards[cardPos];
            }
            // 若手牌数量小于cardPos则越界，若手牌数量与cardPos相等
            // 则要么有手牌，此时cardPos越上界，要么无手牌，此时0下标也越界
            else
            {
                _currentSelectingPos = -1;
                _currentSelectingCard = null;
            }
        }

        
        /// <summary>
        /// 外界接口，使用前需要设置当前选中的卡牌，用于使用当前选中的卡牌
        /// </summary>
        public void OnUseCurrentCard()
        {
            BaseCard baseCardReference = _handcardsInstance[_currentSelectingPos].GetComponent<BaseCard>();
            if (!Player.Instance().CanConsumeAp(baseCardReference.cost))
            {
                // TODO : 并实现AP值震动效果
                Debug.Log("Ran out of AP, cant use this one");
                return;
            }

            if (baseCardReference.type.Equals("Order"))
            {
                Gameplay.Instance().gamePlayInput.OnUseOrderCard(
                    _handcardsInstance[_currentSelectingPos].GetComponent<Ability.Ability>());
                return;
            }

            if (Gameplay.Instance().gamePlayInput.IsSelectingCard == false)
            {
                Gameplay.Instance().gamePlayInput.OnPointerDownUnitCard(_handcardsInstance[_currentSelectingPos]);
                BattleMap.BattleMap.Instance().IsColor = true;
            }
            
        }

        /// <summary>
        /// 放弃使用当前卡牌，会重置当前选择的卡牌
        /// </summary>
        public void CancleUseCurrentCard()
        {
            // TODO: 由UI方面调用
            if (Gameplay.Instance().gamePlayInput.IsSelectingCard)
                Gameplay.Instance().gamePlayInput.InputFSM.PushState(new InputFSMIdleState(
                    Gameplay.Instance().gamePlayInput.InputFSM));

            _currentSelectingPos = -1;
            _currentSelectingCard = null;
        }

        /// <summary>
        /// 用于触发当前选择的效果牌的函数，会重置当前选择的卡牌
        /// </summary>
        public void OnTriggerCurrentCard()
        {
            Player.Instance().ConsumeAp(_handcardsInstance[_currentSelectingPos].GetComponent<OrderCard>().cost);
            MsgDispatcher.SendMsg((int)MessageType.CastCard);
            // 从卡牌中移除当前手牌
            RemoveCardToCd(_currentSelectingPos);
            _currentSelectingPos = -1;
            _currentSelectingCard = null;
        }

        /// <summary>
        /// 初始化存储所有卡牌的数据字典
        /// id -> json
        /// </summary>
        private void InitCardsData()
        {
            _cardsData = new Dictionary<string, JsonData>();

            JsonData cardsJsonData =
                JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/Cards/cardSample.1.json"));

            int dataAmount = cardsJsonData.Count;
            for (int i = 0; i < dataAmount; i++)
            {
                _cardsData.Add(cardsJsonData[i]["id"].ToString(), cardsJsonData[i]);
            }
        }


        /// <summary>
        /// 返回给定的ID对应的Json数据
        /// </summary>
        /// <param name="cardID">卡牌预制件保存的UnitCard内存储的ID</param>
        /// <returns>若存在此ID则返回对应的Json数据，若不存在则返回null</returns>
        public JsonData GetCardJsonData(string cardID)
        {
            if (_cardsData.ContainsKey(cardID))
            {
                return _cardsData[cardID];
            }

            return null;
        }

        /// <summary>
        /// 返回给定卡牌ID对应的卡牌ability id
        /// </summary>
        /// <param name="cardID">卡牌预制件内BaseCard存的ID</param>
        /// <returns>若存在此ID则返回对用的List，若不存在则返回null，若abilityid总数为0，则返回list为空</returns>
        public List<string> GetCardAbilityIDs(string cardID)
        {
            if (_cardsData.ContainsKey(cardID))
            {
                JsonData abilitys = _cardsData[cardID]["ability_id"];
                List<string> abilityIDs = new List<string>();
                for (int i = 0; i < abilitys.Count; i++)
                {
                    abilityIDs.Add(abilitys[i].ToString());
                }

                return abilityIDs;
            }

            return null;
        }

        /// <summary>
        /// 用于设置卡牌堆内卡牌情况
        /// </summary>
        private void LoadCardsIntoSets()
        {
            // TODO: 根据策划案修改此函数，以下仅用于demo
            // 将所有卡牌加入牌堆中，一样只有一张
            foreach (string cardID in _cardsData.Keys)
            {
                _cardsSets.Add(string.Copy(cardID));
            }
            
            // 随机洗牌            
            Shuffle();
            
            // Shuffle函数会发送卡牌堆变动消息，这里不再重复发送
            // MsgDispatcher.SendMsg((int)MessageType.CardsetChange);
        }

        /// <summary>
        /// 从牌组中抽取卡牌到手牌中
        /// </summary>
        /// <param name="cardAmount">抽取卡牌数量，默认为一</param>
        public void ExtractCards(int cardAmount = 1)
        {
            // 若手牌数量大于或等于手牌上限，直接返回（取消检查的话则此判定永false）
            if (_handcards.Count >= cardsUpperLimit && !cancelCheck)
            {
                return;
            }

            // 根据参数确定抽取卡牌的数量,若和抽牌上限一致则使用抽牌上限，否则使用给定参数
            int extractAmount = (cardAmount == extractCardsUpperLimit) ? extractCardsUpperLimit : cardAmount;

            // 计算应该抽取的卡牌数，计算规则：不检查=抽cardAmount张， 检查=不超出手牌数量上限的，最多cardAmount张牌
            extractAmount = cancelCheck ? extractAmount : 
                (cardsUpperLimit - _handcards.Count > extractAmount ? 
                    extractAmount : cardsUpperLimit - _handcards.Count);
            
            // 如果剩余牌量不足，有多少抽多少（几乎不可能）
            if (extractAmount > _cardsSets.Count)
                extractAmount = _cardsSets.Count;
            
            
            // 按照序列抽取卡牌
            for (int i = 0; i < extractAmount; i++)
            {
                // 获得对应卡牌的id
                string cardId= _cardsSets[i];
                
                // 将其从卡牌堆中移除
                _cardsSets.RemoveAt(i);
                
                // 调用接口完成向手牌中插入卡牌操作，设置不发送消息，由后续发生消息
                InsertIntoHandCard(cardId, false);
            }
            
            // 发送手牌变动消息
            MsgDispatcher.SendMsg((int)MessageType.HandcardChange);
            // 发送牌堆变化消息
            MsgDispatcher.SendMsg((int)MessageType.CardsetChange);
        }

        /// <summary>
        /// 用于冷却死亡单位卡牌action的condition函数，确定收到的死亡消息来自于玩家单位
        /// </summary>
        /// <returns>若符合条件则返回true，否则返回false</returns>
        public bool canSendToCoolDownList()
        {
            _latestDeadUnit = this.GetDead();
            return _latestDeadUnit.owner == OwnerEnum.Player;
        }

        /// <summary>
        /// 用于冷却死亡单位卡牌action的action函数，将需要冷却的卡牌放入冷却池
        /// </summary>
        public void HandleCardOnMapToCooldown()
        {
            // 获取刚死亡的卡牌的id
            string cardId = _latestDeadUnit.id;

            
            if (_cardOnMap.Contains(cardId))
                _cardOnMap.Remove(cardId);
            else
            {    
                // 与现有记录不同步，抛出异常
                throw new NotImplementedException();
            }

            // 调用接口进行冷却, 并调用方法计算冷却回合数
            CooldownCard(cardId, CalculateCardCoolDown(_latestDeadUnit));
        }

        /// <summary>
        /// 计算unit的冷却回合数，因为有可能有异能影响所以拿到一边进行计算
        /// </summary>
        /// <param name="unit">要进行计算的卡牌单位的unit引用</param>
        /// <returns>若为-1则表示按照原有冷却进行计算，否则按照计算结果进行冷却</returns>
        private int CalculateCardCoolDown(GameUnit.GameUnit unit)
        {
            // TODO:获取脚本，根据实际情况计算其冷却回合数
            return -1;
        }

        /// <summary>
        /// 将手牌中的单位牌使用时调用接口，处理CardManager内事务并发送卡牌变动信息，不直接冷却卡牌
        /// </summary>
        /// <param name="cardInstance">要移除的手牌的下标</param>
        public void RemoveCardToMapList(GameObject cardInstance)
        {
            // 调用接口完成所有从手牌中移除需要做的操作
            string cardId = RemoveFromHandCard(cardInstance);
            
            // 添加到地图上单位卡牌列表
            _cardOnMap.Add(cardId);

            // 发送消息通知手牌发生变动
            MsgDispatcher.SendMsg((int)MessageType.HandcardChange);
            
        }

        /// <summary>
        /// 从手牌中移除卡牌（默认进行冷却），处理CardManager内事务后发出手牌变动消息，用于外界通知CardManager手牌发生变动
        /// </summary>
        /// <param name="cardIndex">手牌中要移除的卡牌的下标，函数内会进行检测</param>
        /// <param name="controlCd">控制cd值为-1表示按卡牌本身cd值进行冷却，否则按设定值进行冷却</param>
        /// <exception cref="NotImplementedException">若卡牌下标异常则抛出此异常</exception>
        public void RemoveCardToCd(int cardIndex, int controlCd = -1)
        {
            // 调用接口完成所有从手牌中移除需要做的操作
            string cardId = RemoveFromHandCard(cardIndex);
            
            // 将其加入冷却列表进行冷却，传递控制冷却信息
            CooldownCard(cardId, controlCd);
            
            // 发送手牌变动消息
            MsgDispatcher.SendMsg((int)MessageType.HandcardChange);
        }

        /// <summary>
        /// 从手牌中移除卡牌（默认进行冷却），处理CardManager内事务后发出手牌变动消息，用于外界通知CardManager手牌发生变动
        /// </summary>
        /// <param name="cardInstance">要一处的卡牌的实例，函数内会进行检验</param>
        /// <param name="controlCd">控制cd值为-1表示按卡牌本身cd值进行冷却，否则按设定值进行冷却</param>
        public void RemoveCardToCd(GameObject cardInstance, int controlCd = -1)
        {
            // 调用接口完成所有从手牌中移除需要做的操作
            string cardId = RemoveFromHandCard(cardInstance);
            
            // 将其加入冷却列表进行冷却，传递控制冷却信息
            CooldownCard(cardId, controlCd);
            
            // 发送手牌变动消息
            MsgDispatcher.SendMsg((int)MessageType.HandcardChange);
        }

        /// <summary>
        /// 用于将卡牌从手牌中移除，仅会处理cardManager内部事务，会检查下标，若错误则抛出异常
        /// 外界调用前请确认清楚自己的行为可能导致cardManger与UI中卡牌信息不一致
        /// </summary>
        /// <returns>返回移除的卡牌的ID</returns>
        /// <param name="cardIndex">要移除的卡牌在手牌中的下标</param>
        /// <exception cref="NotImplementedException">若下标不存在则抛出此异常</exception>
        public string RemoveFromHandCard(int cardIndex)
        {
            // 检测传入卡牌下标真实性
            if (cardIndex < 0 || cardIndex >= _handcards.Count)
            {
                Debug.Log("Invalid index sent!");
                throw new NotImplementedException();
            }
            
            string cardId = _handcards[cardIndex];
            
            // 销毁该实例
            Destroy(_handcardsInstance[cardIndex]);
            
            // 从手牌列表中移除对应的卡牌
            _handcards.RemoveAt(cardIndex);
            
            // 从手牌实例中移除该object对应的object
            _handcardsInstance.RemoveAt(cardIndex);

            return cardId;
        }

        /// <summary>
        /// 用于移除给定手牌，仅会处理cardManager内部事务，会检查是否存在，若错误则抛出异常
        /// 外界调用前请确认清楚自己的行为可能导致cardManger与UI中卡牌信息不一致
        /// </summary>
        /// <param name="cardInstance">要移出的卡牌的实例</param>
        /// <returns>返回移除的卡牌的ID</returns>
        /// <exception cref="NotImplementedException">若实例不存在则抛出此异常</exception>
        public string RemoveFromHandCard(GameObject cardInstance)
        {
            // 检查是否存在该卡牌实例
            if (!_handcardsInstance.Contains(cardInstance))
            {
                Debug.Log("Card instance does not exist!");
                throw new NotImplementedException();
            }

            string cardId = cardInstance.GetComponent<BaseCard>().id;
            
            // 从手牌列表中移除对应的卡牌
            _handcards.Remove(cardId);

            // 从手牌实例中移除该object对应的object
            _handcardsInstance.Remove(cardInstance);
            
            // 销毁该实例
            Destroy(cardInstance);

            return cardId;
        }

        /// <summary>
        /// 用于将卡牌插入手牌中，会根据参数产生手牌变动消息
        /// </summary>
        /// <param name="cardId">要插入的卡牌的ID</param>
        /// <param name="sendMsg">是否发送手牌变动消息，默认发送，若连续调用请设置false并自己发送消息</param>
        public void InsertIntoHandCard(string cardId, bool sendMsg = true)
        {
            // 实例化卡牌到不可见区域，并绑定脚本再初始化
            GameObject cardInstance = Instantiate(emptyObject);
            JsonData cardData = GetCardJsonData(cardId);
            
            //根据json数据中卡牌分类挂载不同脚本
            if(cardData["type"].ToString().Equals("Order"))
                cardInstance.AddComponent<OrderCard>().Init(cardId, cardData);
            else if(cardData["type"].ToString().Equals("Unit"))
                cardInstance.AddComponent<UnitCard>().Init(cardId, cardData);
            else
                cardInstance.AddComponent<BaseCard>().Init(cardId, cardData);
            
            // 将实例放入对应位置的list中
            _handcardsInstance.Add(cardInstance);
            
            // 更新手牌list
            _handcards.Add(cardId);

            if (sendMsg)
            {
                // 发送手牌变动消息
                MsgDispatcher.SendMsg((int)MessageType.HandcardChange);
            }
        }

        /// <summary>
        /// 用于将手牌中的卡牌移回抽牌牌库，产生牌堆变动、手牌变动消息
        /// </summary>
        /// <param name="cardIndex">要移除的卡牌在手牌中的位置</param>
        /// <exception cref="NotImplementedException">若下标位置异常则抛出异常</exception>
        public void MoveBackToCardSets(int cardIndex)
        {
            // 调用接口完成所有从手牌中移除需要做的操作
            string cardId = RemoveFromHandCard(cardIndex);
            
            // 将其加入cardSets
            _cardsSets.Add(cardId);
            
            // 发送牌堆变动消息
            MsgDispatcher.SendMsg((int)MessageType.CardsetChange);
            
            // 发送手牌变动消息
            MsgDispatcher.SendMsg((int)MessageType.HandcardChange);
        }
            
        /// <summary>
        /// 将预存的预制件放入冷却列表中，仅CardManager内部使用，产生冷却池变动消息
        /// </summary>
        /// <param name="cardId">要冷却的卡牌的预制件引用</param>
        /// <param name="controlCd">默认值为-1，表示按卡牌cd值进行冷却，设定其他值则按设定值冷却</param>
        private void CooldownCard(string cardId, int controlCd = -1)
        {
            // 如果控制cd的参数值小于等于0，表示按卡牌cd值进行冷却
            if (controlCd <= 0)
            {
                // 读取数据库得到冷却回合数
                int roundAmount = (int)_cardsData[cardId]["cd"];

                // 若卡牌自身的cd值为负数，则直接销毁，并进入弃牌堆
                if (roundAmount < 0)
                {
                    _garbageCards.Add(cardId);
                    return;
                }

                // 初始化BaseCard脚本内剩余回合数的counter
                _cooldownCards.Add(new cdObject(roundAmount, cardId));
                
            }
            else // 按controlCd值进行冷却
                _cooldownCards.Add(new cdObject(controlCd, cardId));
            
            // 发送冷却池变动消息
            MsgDispatcher.SendMsg((int)MessageType.CooldownlistChange);
        }

        /// <summary>
        /// 冷却处理函数，响应EP消息，处理卡牌冷却，根据情况产生冷却池变动消息，卡牌堆变动消息
        /// </summary>
        public void HandleCooldownEvent()
        {
            List<cdObject> toRemove = new List<cdObject>();
            for (int i = 0; i < _cooldownCards.Count; i++)
            {
                int leftRounds = _cooldownCards[i].ReduceCd();
                if (leftRounds == 0)
                {
                    toRemove.Add(_cooldownCards[i]);
                }
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                // 将冷却完毕的id重新放回牌堆
                _cardsSets.Add(toRemove[i].objectId);
                
                // 将其从冷却列表中移除
                _cooldownCards.Remove(toRemove[i]);
            }

            // 发送冷却池变动消息
            MsgDispatcher.SendMsg((int)MessageType.CooldownlistChange);
            
            if (toRemove.Count > 0)
                // 发送卡牌堆变动消息
                MsgDispatcher.SendMsg((int)MessageType.CardsetChange);
        }
        
        /// <summary>
        /// 随机洗牌函数，产生卡牌堆变动消息
        /// </summary>
        public void Shuffle()
        {
            int size = _cardsSets.Count;
            
            for (int i = 0; i < size; i++)
            {
                int pos = Random.Range(0, size);
                string temp = _cardsSets[i];
                _cardsSets[i] = _cardsSets[pos];
                _cardsSets[pos] = temp;
            }
            // 发送卡牌堆变动消息
            MsgDispatcher.SendMsg((int)MessageType.CardsetChange);
        }

        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }

        /// <summary>
        /// 检测是否能进行抽卡操作，是抽卡的condition函数
        /// </summary>
        /// <returns>根据实际情况确定是否能抽卡</returns>
        public bool canDoExtractAction()
        {
            return _cardsSets.Count != 0;
        }

        /// <summary>
        /// 检测是否需要进行冷却操作，现在暂时设定为永true，是冷却事件的condition函数
        /// </summary>
        /// <returns>若冷却堆不为0，返回true，即需要处理冷却，否则返回false，即不需要</returns>
        public bool canDoCoolDownAction()
        {
            return _cooldownCards.Count != 0;
        }
    }

    public class cdObject
    {
        private int _leftCd;
        private string _objectId;
        
        public int leftCd
        {
            get { return _leftCd; }
        }
        public string objectId
        {
            get { return _objectId; }
        }

        public cdObject(int cd, string id)
        {
            _leftCd = cd;
            _objectId = id;
        }

        public bool IfCdIsOver()
        {
            return _leftCd == 0;
        }

        public int ReduceCd()
        {
            return --_leftCd;
        }
    }
}