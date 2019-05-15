//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;

//using GamePlay;
//using IMessage;
//using GameUnit;

//namespace NBearUnit
//{
//    //TODO 处理移动部分
//        //1. 算法设计


//   //TODO 解决方案
//        //1. A*算法

//   //以上只考虑 行动力情况，暂时不考虑攻击范围等问题



//    public class UnitMove : MonoBehaviour, IPointerDownHandler
//    {
//        private IMessage.MsgReceiver targetReceiver;
//        /// <summary>
//        /// 从GamePlayInput获取的对象列表
//        /// </summary>
//        private List<Vector2> targetList;


//        private void Awake()
//        {
//            targetList = Gameplay.Instance().gamePlayInput.TargetList;
//            targetReceiver = GameObject.Find("ReceiverTest").GetComponent<MsgTestReceiver>();
//            m_MyEvent.AddListener(() =>
//            {
//                IMessage.MsgDispatcher.SendMsg((int)MsgTestType.UnitMoving);
//            });
//        }

//        public void OnPointerDown(PointerEventData eventData)
//        {
//            if (eventData.button != PointerEventData.InputButton.Left)
//                return;

//            //Debug.Log("左键击下Unit" + UnitManager.Instance.CurUnit);
//            #region 行动染色
            
            
//            //SelectAction.Instance.ShowSeclectActionUI();//显示行为面板
            
//            #endregion

//            m_MyEvent.Invoke();
//        }


//        UnityEvent m_MyEvent = new UnityEvent();
//    }
//}



