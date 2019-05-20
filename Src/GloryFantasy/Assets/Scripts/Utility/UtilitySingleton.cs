//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace GameUtility
//{

//    //超简单泛型
//    public class LiteSingleton<T>
//        where T : new()
//    {
//        private static T _instance;
//        public static T Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//                    _instance = new T();
//                }

//                return _instance;
//            }
//        }
//    }

//    //继承MonoBehaviour的泛型单例
//    public class MonoBehaviourSingleton<T> : MonoBehaviour
//        where T : MonoBehaviourSingleton<T>
//    {
//        private static T _instance;
//        public static T Instance
//        {
//            get { return _instance; }
//        }

//        public void Awake()
//        {
//            _instance = this as T;
//        }
//    }

//}