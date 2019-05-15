using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtility
{
    /// <summary>
    /// 下推状态机的泛型
    /// </summary>
    public class FSMachine<T> where T : FSMState
    {
        public FSMachine()
        {
            StateStack = new Stack<T>();
        }

        /// <summary>
        /// 下推状态栈
        /// </summary>
        protected Stack<T> StateStack;

        /// <summary>
        /// 获取当前状态
        /// </summary>
        public T CurrentState
        {
            get
            {
                if (StateStack.Count == 0)
                {
                    Debug.Log("FSMachine call CurrentState but dont have state.");
                    return null;
                }
                return StateStack.Peek();
            }
        }

        public void Update()
        {
            StateStack.Peek().OnUpdate();
        }

        public void PushState(T pushedState)
        {
            if (StateStack.Count != 0) StateStack.Peek().OnExit();
            pushedState.OnEnter();
            StateStack.Push(pushedState);
        }

        public void PopState()
        {
            StateStack.Peek().OnExit();
            StateStack.Pop();
            if (StateStack.Count != 0) StateStack.Peek().OnEnter();
        }

        public void ClearState()
        {
            StateStack.Clear();
        }
    }
}
