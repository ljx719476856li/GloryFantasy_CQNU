using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit = GameUnit.GameUnit;

/// <summary>
/// 处理异常地图块，例如灼烧
/// </summary>
namespace BattleMap
{
    public class DebuffBattleMapBlovk
    {
        //创造灼烧块
        public void SetBattleMapBlockBurning(List<Vector2> vector2s)
        {
            BattleMapBlock bm = null;
            foreach (Vector2 vector2 in vector2s)
            {
                bm = BattleMap.Instance().GetSpecificMapBlock(vector2);
                bm.blockType = EMapBlockType.Burnning;
            }
            
        }

        //创造滞留块
        public void SetBattleMapBlockRetrad(List<Vector2> vector2s)
        {
            BattleMapBlock bm = null;
            foreach (Vector2 vector2 in vector2s)
            {
                bm = BattleMap.Instance().GetSpecificMapBlock(vector2);
                bm.blockType = EMapBlockType.Retire;
            }
        }
        
        //单位进入灼烧块
        public void UnitEnterBurning(Vector2 vector2)
        {
            if (BattleMap.Instance().CheckIfHasUnits(vector2)){
                Unit unit = BattleMap.Instance().GetUnitsOnMapBlock(vector2);
                unit.hp -= 1;//TODO更新HP显示
                Debug.Log(unit.hp);
            }
        }

        //单位进入滞留块
        public void UnitEnterRetire(Unit unit,BattleMapBlock battleMapBlock)
        {
            unit.mapBlockBelow = battleMapBlock;
        }
    }
}

