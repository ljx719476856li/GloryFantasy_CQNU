//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using GamePlay;
//using GameUnit;

//public class SelectAction : MonoBehaviour{

//    /// <summary>
//    /// 在这处理战斗行为；
//    /// </summary>

//    //好吧，还是单例方便
//    private static SelectAction instance = null;
//    public static SelectAction Instance
//    {
//        get
//        {
//            return instance;
//        }
//    }
     
//    // Use this for initialization
//	void Start () {
//        instance = this;
//        BattleMap.BattleMap.Instance().selectAction.SetActive(false);
//    }
	
//	// Update is called once per frame
//	void Update () {
		
//	}

//    public void ShowSeclectActionUI ()
//    {
//        BattleMap.BattleMap.Instance().selectAction.SetActive(true);
//        BattleMap.BattleMap.Instance().selectAction.transform.position = Input.mousePosition;
//    }

//    public void HideSeclectActionUI()
//    {
//        BattleMap.BattleMap.Instance().selectAction.SetActive(false);
//    }
//    //攻击接口
//    public void Attack()
//    {
//        Vector2 tempPosition = BattleMap.BattleMap.Instance().curMapPos;
//        Gameplay.Instance().gamePlayInput.HandleAtkConfirm(tempPosition);
//        BattleMap.BattleMap.Instance().selcetAction_Cancel.SetActive(true);
//        BattleMap.BattleMap.Instance().selcetAction_Cancel.transform.position = Input.mousePosition;
//        HideSeclectActionUI();
//    }

//    //取消当前行动返回选择面板
//    public void Cancel()
//    {
//        BattleMap.BattleMap.Instance().selcetAction_Cancel.SetActive(false);
//        Gameplay.Instance().gamePlayInput.HandleAtkCancel(unitPositon());
//        BattleMap.BattleMap.Instance().selectAction.SetActive(true);
//    }

//    //防御接口
//    public void Defense()
//    {
//        HideSeclectActionUI();
//    }

//    private Vector2 unitPositon()
//    {
//        //？？？？？？
//        return Vector2.zero;
//    }
//}
