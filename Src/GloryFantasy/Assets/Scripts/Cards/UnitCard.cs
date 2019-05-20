using LitJson;
using UnityEngine;

namespace GameCard
{
    /// <summary>
    /// 单位卡牌类
    /// </summary>
    public class UnitCard : BaseCard
    {
        
        public override void Init(string cardId, JsonData cardData)
        {
            base.Init(cardId, cardData);
            foreach (string abilityName in ability_id)
            {
                gameObject.AddComponent(System.Type.GetType("Ability." +abilityName));
                Debug.Log(abilityName);
            }
        }
        
        /// <summary>
        /// 点击使用卡牌时调用的函数
        /// </summary>
        /// <returns>若成功使用则返回true，中途取消或其他情况返回false</returns>
        public bool Use()
        {
            return true;
        }
    }
}