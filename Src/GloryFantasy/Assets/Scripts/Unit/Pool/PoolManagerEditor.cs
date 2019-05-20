//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public class PoolManagerEditor  {

//    [MenuItem("Manager/Create GameObjectPoolConfig")]
//    static void CreateGameObjectPoolList()
//    {
//        GameObjectPoolList poolList = ScriptableObject.CreateInstance<GameObjectPoolList>();
//        string path = GameUnitPool.PoolConfigPath; //初始化
//        AssetDatabase.CreateAsset(poolList, path);
//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();
//    }
//}
