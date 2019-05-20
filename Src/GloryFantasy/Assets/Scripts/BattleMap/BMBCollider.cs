using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleMap;
using Unit = GameUnit.GameUnit;

public class BMBCollider
{
    //地图块儿的检测范围
    public List<Vector2> colliderRange = new List<Vector2>();
    //进入单位
    public List<Unit> enterUnits = new List<Unit>();
    //离开单位
    public List<Unit> exitUnits = new List<Unit>();
    //驻足单位
    public List<Unit> disposeUnits = new List<Unit>();


    //-1 离开 / 0 进入 / 1 驻足
    public int state = -1; //-2(默认值)
    /// <summary>
    /// 初始化检测范围
    /// </summary>
    /// <param name="battleMapBlock"></param>
    public void init(BattleMapBlock battleMapBlock)
    {
        for(int i = 0; i < 2; i++)
        {
            float x = battleMapBlock.position.x + i;
            float y = battleMapBlock.position.y + i;
            colliderRange.Add(new Vector2(x, y));
        }
    }

    /// <summary>
    /// 单位进入，并更新enterUnits，state
    /// </summary>
    /// <param name="unit">当前操作单位</param>
    public void OnUnitEnter(List<Unit> units)
    {

        enterUnits = units; //进入记录
        state = 0;
        Debug.Log("坐标：" + colliderRange[0] + " 地图块儿检测到单位进入");
        if (units[0].nextPos == units[0].CurPos)
            OnUnitDispose();

    }
    /// <summary>
    /// 单位退出，并更新enterUnits/disposeUnits，state
    /// </summary>
    /// <param name="unit">当前操作单位</param>
    public void OnUnitExit()
    {
        exitUnits = enterUnits; //退出记录
        enterUnits = new List<Unit>(); //覆盖

        if (state == 1)
            disposeUnits = new List<Unit>();//覆盖
        state = -1;
        Debug.Log("坐标：" + colliderRange[0] + " 地图块儿检测到单位离开");

    }

    /// <summary>
    /// 单位驻足，并更新disposeUnits，state
    /// </summary>
    /// <param name="unit">当前操作单位</param>
    public void OnUnitDispose()
    {
        state = 1;
        disposeUnits = enterUnits; //驻足记录
        Debug.Log("坐标：" + colliderRange[0] + " 地图块儿检测到单位驻足");
        if (GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.CurrentState is GamePlay.FSM.InputFSMSummonState)
            return;
        GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.HandleAtkConfirm(colliderRange[0]);
        //GamePlay.Gameplay.Instance().gamePlayInput.HandleAtkConfirm(colliderRange[0]);
    }
}
