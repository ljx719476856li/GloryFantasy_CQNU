using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IMessage
{

    /// <summary>
    /// 测试用按钮代码，请勿使用
    /// </summary>
    public class MsgTestButton : MonoBehaviour
    {

        //单例模式
        private static MsgTestButton instance;

        public static MsgTestButton Instance
        {
            get
            {
                return instance;
            }
        }

        //[SerializeField] public IMessage.MsgReceiver targetReceiver;
        public GameObject targetReceiver;

        #region
        private UnityEvent unityEventUpdateResource = new UnityEvent();
        private UnityEvent unityEventBattlePresent = new UnityEvent();
        public UnityEvent UnityEventUpdateResource { get { return unityEventUpdateResource; } }
        #endregion

        #region 
        private bool isUpdateResourceOver = false;
        private bool isBattlePresentOver = false;
        public bool IsUpdateResourceOver
        {
            get
            {
                return isUpdateResourceOver;
            }
            set
            {
                isUpdateResourceOver = value;
            }
        }
        public bool IsBattlePresentOver
        {
            get
            {
                return isBattlePresentOver;
            }
            set
            {
                isBattlePresentOver = value;
            }
        }
        #endregion


        private void Awake()
        {
            instance = this;
            //GetComponent<Button>().onClick.AddListener(() =>
            //{
            //    //IMessage.MsgDispatcher.SendMsg((int)MsgTestType.A, targetReceiver.GetComponent<IMessage.MsgReceiver>());
            //    IMessage.MsgDispatcher.SendMsg((int)MsgTestType.A);
            //});
        }

        private void Start()
        {
            unityEventUpdateResource.AddListener(() =>
            {
                MsgDispatcher.SendMsg((int)MessageType.UpdateSource);
            });
            unityEventBattlePresent.AddListener(() =>
            {
                MsgDispatcher.SendMsg((int)MessageType.BP);
            });

            unityEventUpdateResource.Invoke();
        }

        //响应结束按钮点击事件
        public void EndBtnOnClick()
        {
            if (isUpdateResourceOver == true)
            {
                unityEventBattlePresent.Invoke();
                isUpdateResourceOver = false;
            }
        }

    }
}