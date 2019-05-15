using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.Events;
using UnityEngine.UI;

namespace GameGUI
{
    public class CardUI : MonoBehaviour, IPointerDownHandler
    {
        #region UI Component
        private Image m_itemImage;
        private Image ItemImage
        {
            get
            {
                if (m_itemImage == null)
                {
                    m_itemImage = GetComponent<Image>();
                }
                return m_itemImage;
            }
        }
        #endregion


        /// <summary>
        /// 设置卡图不可见
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// 设置卡图可见
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 设置卡图的当前位置
        /// </summary>
        /// <param name="position">目标位置</param>
        public void SetLocalPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        /// <summary>
        /// 设置动作牌的卡图
        /// </summary>
        public void SetOrderCard()
        {
            //TODO 获取当前Unit信息与image
            //当前为测试代码。。我这边根据resources函数处理的
            ItemImage.sprite = Resources.Load<Sprite>("test");
        }
        /// <summary>
        /// 设置单位牌的卡图
        /// </summary>
        public void SetUnit()
        {
            //TODO 获取当前Unit信息与image
            //当前为测试代码。。我这边根据resources函数处理的
            ItemImage.sprite = Resources.Load<Sprite>("test");
        }

        /// <summary>
        /// 卡牌点击的回调
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
        
        }
    }
}


