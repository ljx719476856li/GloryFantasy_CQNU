using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GamePlay;

namespace Ability.Buff
{
    public class BuffManager
    {
        List<Buff> BuffList = new List<Buff>();

        /// <summary>
        /// 将传入的Buff加入到管理器中管理声明周期
        /// </summary>
        /// <param name="buff"></param>
        public void AddBuff(Buff buff)
        {
            BuffList.Add(buff);
        }

        /// <summary>
        /// 将所有Buff的生命周期减去0.5，并删除生命周期为0的Buff
        /// </summary>
        public void SubtractBuffLife()
        {
            foreach (Buff buff in BuffList)
            {
                buff.SetLife(buff.Life - 0.5f);
                if (buff.Life <= 0)
                {
                    BuffList.Remove(buff);
                    GameObject.Destroy(buff);
                }
            }
        }
    }

    public class Buff : MonoBehaviour, GameplayTool
    {
        /// <summary>
        /// Buff的声明周期，0.5等于半个回合，1等于自己+PC完整的一个回合
        /// </summary>
        public float Life { get; private set; }
        /// <summary>
        /// 仅用作增加错误信息，无实际用途
        /// </summary>
        public string BuffName = "Buff:NoDefine";

        private void Start()
        {
            //将自己加入到BuffManager
            Gameplay.Instance().buffManager.AddBuff(this);
            //初始化Buff要做的事情
            InitialBuff();
        }

        private void OnDestroy()
        {
            //调用被删除
            this.OnDisappear();
        }

        /// <summary>
        /// 设定Buff生命周期，0.5等于半个回合，1等于自己+PC完整的一个回合
        /// </summary>
        /// <param name="Life"></param>
        public void SetLife(float life)
        {
            this.Life = life;
        }

        /// <summary>
        /// 设定Buff被赋予时要做的事情
        /// </summary>
        virtual protected void InitialBuff() { }

        /// <summary>
        /// 设定Buff消失时要做的事情（暂时不区分被净化和达到时限的区别）
        /// </summary>
        virtual protected void OnDisappear() { }
    }
}
