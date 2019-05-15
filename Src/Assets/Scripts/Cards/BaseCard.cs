using System;
using System.Collections.Generic;
using IMessage;
using LitJson;
using UnityEngine;

namespace GameCard
{
    public class BaseCard : MonoBehaviour, IMessage.MsgReceiver
    {
        #region 变量定义
        
        private List<string> _abilityId;
        private List<string> _tag;
        
        private int _cd;
        private int _cost;
        
        private string _slot;
        private string _color;
        private string _effect;
        private string _flavourText;
        private string _name;
        private string _relatedToken;
        private string _tokenId;
        private string _type;
        
        private string _id;
        
        // 记录卡牌冷却回合数
        public int cooldownRounds;
        #endregion
        
        #region 所有变量可访问性定义
        /// <summary>
        /// 若没有id，则得到list为空，而不是Null！
        /// </summary>
        public List<string> ability_id
        {
            get { return _abilityId; }
        }

        public List<string> tag
        {
            get { return _tag; }
        }

        public int cd
        {
            get { return _cd; }
        }

        public string slot
        {
            get { return _slot; }
        }

        public int cost
        {
            get { return _cost; }
        }

        public string color
        {
            get { return _color; }
        }

        public string effect
        {
            get { return _effect; }
        }

        public string flavor_text
        {
            get { return _flavourText; }
        }

        public string name
        {
            get { return _name; }
        }

        public string related_token
        {
            get { return _relatedToken; }
        }

        public string tokenID
        {
            get { return _tokenId; }
        }

        public string type
        {
            get { return _type; }
        }
        
        public string id
        {
            get { return _id; }
        }
        
        #endregion
        
        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }

        /// <summary>
        /// 调用时从卡牌数据库请求数据并进行初始化
        /// </summary>
        /// <param name="cardId">卡牌初始化使用的CardID.必须存在</param>
        /// <param name="cardData">卡牌初始化时需要用到的json数据</param>
        /// <exception cref="NotImplementedException">无法正常初始化</exception>
        public void Init(string cardId, JsonData cardData)
        {
            // 若id为空，则抛出异常，一般在预制件没有做好，或者程序内某个地方挂上了id为空的BaseCard脚本就会抛出错误
            if (cardId.Length == 0 )
            {
                throw new NotImplementedException();
            }

            _id = cardId;
            
            // 如果不存在此ID，则抛出异常
            if(cardData == null)
                throw new NotImplementedException();

            _cd = int.Parse(cardData["cd"].ToString());
            _cost = int.Parse(cardData["cost"].ToString());
            _slot = cardData["slot"].ToString();
            _color = cardData["color"].ToString();
            _effect = cardData["effect"].ToString();
            _flavourText = cardData["flavor_text"].ToString();
            _name = cardData["name"].ToString();
            _relatedToken = cardData["related_token"].ToString();
            _tokenId = cardData["tokenID"].ToString();
            _type = cardData["type"].ToString();

            _abilityId = new List<string>();
            _tag = new List<string>();

            for (int i = 0; i < cardData["ability_id"].Count; i++)
            {
                _abilityId.Add(string.Copy(cardData["ability_id"][i].ToString()));
            }

            for (int i = 0; i < cardData["tag"].Count; i++)
            {
                _tag.Add((string.Copy(cardData["tag"][i].ToString())));
            }

            foreach (string abilityName in _abilityId)
            {
                gameObject.AddComponent(System.Type.GetType("Ability." +abilityName));
            }
            
        }

        /// <summary>
        /// 用于使用卡牌效果
        /// </summary>
        /// <returns>返回使用结果，成功返回true</returns>
        public virtual bool Use()
        {
            return true;
        }

    }
    
}