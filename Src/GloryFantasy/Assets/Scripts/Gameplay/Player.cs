using System.Collections;
using System.Collections.Generic;
using IMessage;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay
{
    public enum PlayerEnum
    {
        Player,
        AI
    };

    public class Player: UnitySingleton<Player>, IMessage.MsgReceiver
    {
        
        #region 变量
        /// <summary>
        /// 专注值的上限
        /// </summary>
        private int _apUpLimit;
        
        /// <summary>
        /// 专注值，计算方式为：默认专注值 + 增益专注值
        /// </summary>
        private int _ap;
        
        /// <summary>
        /// 默认专注值
        /// </summary>
        private int _defaultAp;
        
        /// <summary>
        /// 增益专注值，用于存储因为buff或者其他因素对于已有专注值的影响
        /// </summary>
        private int _addOnAp;
        #endregion

        #region 变量可见性定义

        public int ap
        {
            get{ return _ap; }
        }

        public int apUpLimit
        {
            get { return _apUpLimit; }
        }

        public int defaultAp
        {
            get { return _defaultAp; }
        }

        public int addOnAp
        {
            get { return _addOnAp; }
        }
        #endregion


        private void Start()
        {
            InitAp();
            
            // 注册恢复AP值的函数
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(), 
                (int)MessageType.UpdateSource, 
                canDoAction, 
                ReCalculateAp,
                "RestoreAP Trigger"
                );
            
        }

        /// <summary>
        /// 用于注册的condition函数，现在没有具体限制所以返回值永远为true
        /// </summary>
        /// <returns>根据情况返回是否可以进行Action</returns>
        public bool canDoAction()
        {
            return true;
        }

        /// <summary>
        /// 初始化Ap值，根据当前策划，默认上限为3
        /// </summary>
        public void InitAp()
        {
            //_apUpLimit = 30;
            _apUpLimit = 3;
            _defaultAp = _apUpLimit;
            _addOnAp = 0;
            ReCalculateAp();
        }
        
        /// <summary>
        /// 增加ap值的接口
        /// </summary>
        /// <param name="Ap">要增加的ap值</param>
        public void AddAp(int Ap)
        {
            _addOnAp += ap;
            ReCalculateAp();
        }


        /// <summary>
        /// 清除附加ap值效果，请于回合开始时进行调用
        /// </summary>
        public void ClearAddOnAp()
        {
            _addOnAp = 0;
            ReCalculateAp();
        }

        /// <summary>
        /// 重新计算ap值的接口,用于更新ap值时调用，也是Action函数
        /// </summary>
        public void ReCalculateAp()
        {
            _ap = _addOnAp + _defaultAp;
            
            // 超出专注值上限时进行求余操作，等于上限时不做变化
            if (_ap > _apUpLimit)
                _ap %= _apUpLimit;
        }

        
        /// <summary>
        /// 消耗ap值接口，返回值确定是否成功消耗
        /// </summary>
        /// <param name="Ap">消耗的ap值</param>
        /// <returns>若成功消耗返回true，否则为false并不改变玩家当前ap值</returns>
        public bool CanConsumeAp(int Ap)
        {
            if (_ap < Ap)
                return false;
            
            return true;
        }

        /// <summary>
        /// 消耗AP值的接口，请确定能消耗之后再进行消耗
        /// </summary>
        /// <param name="Ap">要消耗掉的ap值</param>
        public void ConsumeAp(int Ap)
        {
            _ap -= Ap;
        }

        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
    }

    public class Computer
    {

    }
}