using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GamePlay;

namespace IMessage
{

    enum MessageType
    {
        UpdateSource, //？
        BP, //开始阶段
        MPBegin, //主要阶段开始
        MPEnd, //主要阶段结束
        EP, //结束阶段
        CastCard, //发动卡片
        Summon, //召唤
        DrawCard, //抽牌
        Prepare,//准备阶段
        Discard,//弃牌阶段
        AI,     //敌人行动阶段
        
        WIN,    // 胜利消息
        LOSE,    // 失败消息
        
        HandcardChange,        // 手牌变动消息
        CardsetChange,         // 卡牌堆变动消息
        CooldownlistChange,    // 冷却列表变动消息
        
        AddInHand, //加入手牌
        AnnounceAttack, //攻击宣言
        ActiveAbility, //异能发动
        
        
        #region ATK 时点部分
        BeAttacked, //被攻击
        Damage, //造成伤害
        BeDamaged, //被伤害
        Kill, //杀死了什么
        BeKilled, //被杀死
        Dead, //死亡
        ToBeKilled, //即将被杀死
        #endregion

        #region 状态
        SIdleToSPerpareMove,//idle -> PerpareMove
        SPerpareMoveToSIdle,//PerpareMove -> Idle
        SIdleToSStartMove,//Idle -> StartMove
        SStartMoveToSIdle,
        SIdleToAtk,
        SAtkToSIdle,
        #endregion

        Move, //开始移动
        UnitMoving, //正在移动
        Aftermove, //移动结束
        
        RoundsEnd,  //回合结束
        
        Encounter // 遭遇战
    };

    public interface MsgReceiver
    {
        /// <summary>
        /// 返回接收者接口所依附的基类,注意一定要保证请求的基类是正确的
        /// </summary>
        /// <returns></returns>
        T GetUnit<T>() where T : MonoBehaviour;
    }

    public delegate bool Condition();
    public delegate void Action();

    public static class MsgDispatcher
    {
        /// <summary>
        /// Trigger句柄，包含receiver, eventType, Condition和Action
        /// </summary>
        class MsgHandler
        {
            public MsgReceiver receiver;
            public int msgName;
            public Condition condition;
            public Action action;

            public MsgHandler(MsgReceiver receiver, int msgName, Condition condition, Action action)
            {
                this.receiver = receiver;
                this.msgName = msgName;
                this.condition = condition;
                this.action = action;
            }

            /// <summary>
            /// 触发Trigger
            /// </summary>
            public void strike()
            {
                if (condition())
                {
                    action();
                }
            }
        }

        static Dictionary<int, List<MsgHandler>> MsgHandlerDict = new Dictionary<int, List<MsgHandler>>();

        /// <summary>
        /// 给msgReciver增加注册MSG的函数
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="TriggerName"></param>
        public static void RegisterMsg(Trigger trigger, string TriggerName = "NoDefine")
        {
            RegisterMsg(trigger.register, trigger.msgName, trigger.condition, trigger.action, TriggerName);
        }
        /// <summary>
        /// 给msgReciver增加注册MSG的函数
        /// </summary>
        /// <param name="self"></param>
        /// <param name="msgName"></param>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <param name="TriggerName"></param>
        public static void RegisterMsg(this MsgReceiver self, int msgName, Condition condition, Action action, string TriggerName = "NoDefine")
        {
            if (msgName < 0)
            {
                Debug.Log("RegisterMsg: " + TriggerName + "'s "+ msgName + "is not define");
            }
            if (null == condition)
            {
                Debug.Log("RegisterMsg: " + TriggerName + "'s condition" + "is null");
            }
            if (null == action)
            {
                Debug.Log("RegisterMsg: " + TriggerName + "'s action" + "is null");
            }
            
            if (!MsgHandlerDict.ContainsKey(msgName))
            {
                MsgHandlerDict[msgName] = new List<MsgHandler>();
            }

            var handlers = MsgHandlerDict[msgName];

            handlers.Add(new MsgHandler(self, msgName, condition, action));

            Debug.Log("RegisterMsg: " + TriggerName + "successfully register");

        }
        /// <summary>
        /// 返回MsgReceiver
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static MsgReceiver GetMsgReceiver(this MsgReceiver self)
        {
            return self;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="targetReceiver">目标接收者，不填或null为广域广播</param>
        public static void SendMsg(int msgName, MsgReceiver targetReceiver = null)
        {
            if (msgName < 0)
            {
                //Debug.Log("SendMsg: " + msgName + "is not define");
            }
            if (!MsgHandlerDict.ContainsKey(msgName))
            {
                //Debug.Log("SendMsg: " + msgName + "had not been regeisted");
                return;
            }

            var handlers = MsgHandlerDict[msgName];
            var handlerCount = handlers.Count;

            for (int index = handlerCount - 1; index >= 0; index --)
            {
                var handler = handlers[index];
                if (handler.receiver != null && !handler.receiver.Equals(null))
                //if ((MonoBehaviour)handler.receiver != null)
                {

                    //单播
                    if (targetReceiver != null)
                    {
                        if (targetReceiver == handler.receiver)
                        {
                            handler.strike();
                        }
                    }
                    else
                    {
                        //广域广播
                        handler.strike();
                    }
                }
                else
                {
                    //接收者已经不存在则从广播列表里删除
                    handlers.Remove(handler);
                    Debug.Log("SendMsg: One " + msgName + "'s receiver had been destory");
                }
            }
        }
    }

    public class Trigger : GameplayTool
    {
        /// <summary>
        /// 注册这个Trigger的游戏物体
        /// </summary>
        public MsgReceiver register;
        /// <summary>
        /// Trigger会被触发的消息
        /// </summary>
        public int msgName;
        /// <summary>
        /// Trigger的成立限定条件函数
        /// </summary>
        public Condition condition;
        /// <summary>
        /// Trigger的执行函数
        /// </summary>
        public Action action;
    }
}