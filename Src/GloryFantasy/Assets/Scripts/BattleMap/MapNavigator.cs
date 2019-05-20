using GameUnit;
using IMessage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unit = GameUnit.GameUnit;

namespace BattleMap
{
    /// <summary>
    /// 寻路结点
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 该点的坐标
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 该点上一个点的坐标
        /// </summary>
        public Vector2 parentPosition;
        /// <summary>
        /// 从起点走到该点的代价
        /// </summary>
        public int G;
        /// <summary>
        /// 从该点走到终点的理想预测代价
        /// </summary>
        public int H;

        public Node(Vector2 _position, Vector2 _destination , int _G = int.MaxValue / 2)
        {
            position = _position;
            parentPosition = _position;
            this.H = (int)Vector2.Distance(_position, _destination);
            this.G = _G;
        }

        public Node(Vector2 _position, Vector2 _fatherPosition, Vector2 _destination, int _G = int.MaxValue / 2)
        {
            position = _position;
            parentPosition = _fatherPosition;
            this.H = (int)Vector2.Distance(_position, _destination);
            this.G = _G;
        }
    }

    public class MapNavigator : MsgReceiver
    {
        T IMessage.MsgReceiver.GetUnit<T>(){return this as T;}

        public Dictionary<Vector2, Node> openList;
        public Dictionary<Vector2, Node> closeList;
        List<Node> paths; //最优路径

        List<Vector2> vectorPaths;
        public List<Vector2> Paths
        {
            get
            {
                if(paths != null)
                {
                    vectorPaths = new List<Vector2>();
                    foreach (Node path in paths)
                    {
                        vectorPaths.Add(path.position);
                    }
                    return vectorPaths;
                }
                return null;
            }
        }

        //寻路入口
        public bool PathSearchForAI(Vector2 startPos, Vector2 endPos, bool isAtk)
        {
            if (BattleMap.Instance().GetSpecificMapBlock((int)startPos.x, (int)startPos.y) == null || BattleMap.Instance().GetSpecificMapBlock((int)endPos.x, (int)endPos.y) == null)
            {
                Debug.Log("In MapNavigator: invalid Pos");
                return false;
            }

            //初始化openList和closeList
            openList = new Dictionary<Vector2, Node>();
            closeList = new Dictionary<Vector2, Node>();
            //加入起点
            openList.Add(startPos, new Node(startPos, endPos, 0));
            do
            {
                //遍历OpenList，寻找F值最小的节点，设为A
                Node A = new Node(startPos, endPos, int.MaxValue / 2);
                //遍历OpenList，寻找F值最小的节点，设为A
                foreach (Node node in openList.Values)
                {
                    if (node.H + node.G < A.H + A.G)
                        A = node;
                }

                AStarSearch(A, startPos, endPos);
                openList.Remove(A.position);
                closeList.Add(A.position, A);

                //如果找到了endPos
                if (A.H < Mathf.Epsilon)
                {
                    paths = new List<Node>();
                    paths.Add(closeList[endPos]);
                    //Debug.Log("找到路径");
                    for (int i = 0; i < closeList.Count; i++)
                    {
                        paths.Add(closeList[paths[i].parentPosition]);
                        if (paths[i + 1].G == 0)
                            break;
                    }
                    //如果是攻击ai的话
                    if (isAtk)
                        return true;
                    //判断移动力与路径长度
                    if (IsExcessUnitMove())
                        return false;
                    return true;
                }

                //OpenList是否还有节点
            } while (openList.Count > 0);

            return false;
        }

        //寻路入口
        public bool PathSearch(Vector2 startPos, Vector2 endPos)
        {
            if (BattleMap.Instance().GetSpecificMapBlock((int)startPos.x, (int)startPos.y) == null || BattleMap.Instance().GetSpecificMapBlock((int)endPos.x, (int)endPos.y) == null)
            {
                Debug.Log("In MapNavigator: invalid Pos");
                return false;
            }

            //初始化openList和closeList
            openList = new Dictionary<Vector2, Node>();
            closeList = new Dictionary<Vector2, Node>();
            //加入起点
            openList.Add(startPos, new Node(startPos, endPos, 0));
            do
            {
                //遍历OpenList，寻找F值最小的节点，设为A
                Node A = new Node(startPos, endPos, int.MaxValue / 2);
                //遍历OpenList，寻找F值最小的节点，设为A
                foreach (Node node in openList.Values)
                {
                    if (node.H + node.G < A.H + A.G)
                        A = node;
                }

                AStarSearch(A, startPos, endPos);
                openList.Remove(A.position);
                closeList.Add(A.position, A);

                //如果找到了endPos
                if (A.H < Mathf.Epsilon)
                {
                    paths = new List<Node>();
                    paths.Add(closeList[endPos]);
                    //Debug.Log("找到路径");
                    for (int i = 0; i < closeList.Count; i++)
                    {
                        paths.Add(closeList[paths[i].parentPosition]);
                        if (paths[i + 1].G == 0)
                            break;
                    }

                    //判断移动力与路径长度
                    if (IsExcessUnitMove())
                        return false;
                    return true;
                }

                //OpenList是否还有节点
            } while (openList.Count > 0);

            return false;
        }

        //查找周边方块
        private void AStarSearch(Node A, Vector2 startPos, Vector2 endPos)
        {
            //获得A的周边MapBlock
            List<BattleMapBlock> neighbourBlock = BattleMap.Instance().GetNeighbourBlock(A);
            //将MapBlock转为Node格式
            List<Node> neighourNode = new List<Node>();
            foreach (BattleMapBlock mapBlock in neighbourBlock)
            {
                neighourNode.Add(new Node(mapBlock.position, A.position, endPos, A.G + 1));
            }

            //遍历A的周边Node
            for (int i = 0; i < neighourNode.Count; i++)
            {
                Node B = neighourNode[i];
                //如果在关闭列表中直接忽略跳到下一个
                if (closeList.ContainsKey(B.position))
                    continue;
                //如果在开放列表中
                if (openList.ContainsKey(B.position))
                {
                    if (B.G > A.G + 1)
                    {
                        B.parentPosition = A.position;
                        B.G = A.G + 1;
                    }
                    continue;
                }
                //如果不在两个列表里
                else if (!openList.ContainsKey(B.position))
                {
                    openList.Add(B.position, B);
                    continue;
                }
            }
        }

        /// <summary>
        /// 判断A星算法给出的路径是否超过单位行动力
        /// </summary>
        /// <param name="unit">当前控制单位</param>
        /// <returns></returns>
        private bool IsExcessUnitMove()
        {
            Vector2 startPos = paths[paths.Count - 1].position;
            Unit gameUnit = BattleMap.Instance().GetUnitsOnMapBlock(startPos);

            Debug.Log(gameUnit.name + "行动力为 " + gameUnit.mov);
            if (paths.Count - 1 > gameUnit.mov)
            {
                Debug.Log("超过移动力，无法移动到指定目标");
                return true;
            }
            return false;
        }

        /// <summary>
        /// AI移动
        /// 一格一格移动处理函数
        /// </summary>
        /// <returns></returns>
        public IEnumerator moveStepByStepAI(Unit unit, List<Vector2> paths, System.Action callback)
        {
            #region 测试一哈 先固定（0.0）为灼烧块，（0.1）为粘滞块
            List<Vector2> vector2s = new List<Vector2>();
            vector2s.Add(new Vector2(0, 0));
            BattleMap.Instance().debuffBM.SetBattleMapBlockBurning(vector2s);
            List<Vector2> vector22s = new List<Vector2>();
            vector22s.Add(new Vector2(1, 1));
            BattleMap.Instance().debuffBM.SetBattleMapBlockRetrad(vector22s);
            #endregion

            Vector2 tempVector;
            BattleMapBlock battleMap;
            bool isRetire = false;
            for (int i = paths.Count - 2; i >= 0; i--)
            {
                //移除上一步的地图块儿下面的units_on_me
                tempVector = new Vector2((int)paths[i + 1].x, (int)paths[i + 1].y);
                battleMap = BattleMap.Instance().GetSpecificMapBlock(tempVector);
                battleMap.RemoveUnit(unit);
                //添加当前unit到地图块儿下面的units_on_me内
                tempVector = new Vector2((int)paths[i].x, (int)paths[i].y);
                battleMap = BattleMap.Instance().GetSpecificMapBlock(tempVector);
                if(i != 0)
                    battleMap.AddUnit(unit);
                else
                    battleMap.AddUnit(unit, false);
                unit.transform.localPosition = Vector3.zero;

                if (battleMap.blockType == EMapBlockType.Burnning)//如果经过灼烧块
                {
                    BattleMap.Instance().debuffBM.UnitEnterBurning(tempVector);
                } 
                else if (battleMap.blockType == EMapBlockType.Retire)//如果经过滞留块
                {
                    
                    BattleMap.Instance().debuffBM.UnitEnterRetire(unit,battleMap);
                    unit.nextPos = paths[i];
                    MsgDispatcher.SendMsg((int)MessageType.Aftermove);
                    isRetire = true;
                    break;
                }
                unit.nextPos = paths[i];
                MsgDispatcher.SendMsg((int)MessageType.Move);
                yield return new WaitForSeconds(0.4f); 
            }
            if (isRetire == false)
            {
                MsgDispatcher.SendMsg((int)MessageType.Aftermove);
            }
            if (callback != null)
                callback();
        }
        //一格一格移动
        public IEnumerator moveStepByStep(Unit unit)
        {
            #region 测试一哈 先固定（0.0）为灼烧块，（0.1）为粘滞块
            List<Vector2> vector2s = new List<Vector2>();
            vector2s.Add(new Vector2(0, 0));
            BattleMap.Instance().debuffBM.SetBattleMapBlockBurning(vector2s);
            List<Vector2> vector22s = new List<Vector2>();
            vector22s.Add(new Vector2(1, 1));
            BattleMap.Instance().debuffBM.SetBattleMapBlockRetrad(vector22s);
            #endregion

            Vector2 tempVector;
            BattleMapBlock battleMap;
            for (int i = paths.Count - 2; i >= 0; i--)
            {
                //移除上一步的地图块儿下面的units_on_me
                tempVector = new Vector2((int)paths[i + 1].position.x, (int)paths[i + 1].position.y);
                battleMap = BattleMap.Instance().GetSpecificMapBlock(tempVector);
                battleMap.RemoveUnit(unit);
                //添加当前unit到地图块儿下面的units_on_me内
                tempVector = new Vector2((int)paths[i].position.x, (int)paths[i].position.y);
                battleMap = BattleMap.Instance().GetSpecificMapBlock(tempVector);
                battleMap.AddUnit(unit);
                unit.transform.localPosition = Vector3.zero;

                if (battleMap.blockType == EMapBlockType.Burnning)//如果经过灼烧块
                {
                    BattleMap.Instance().debuffBM.UnitEnterBurning(tempVector);
                }
                else if (battleMap.blockType == EMapBlockType.Retire)//如果经过滞留块
                {
                    BattleMap.Instance().debuffBM.UnitEnterRetire(unit,battleMap);
                    unit.nextPos = paths[i].position;
                    GamePlay.Gameplay.Instance().bmbColliderManager.Fresh(unit);
                    break;
                }
                unit.nextPos = paths[i].position;
                GamePlay.Gameplay.Instance().bmbColliderManager.Fresh(unit);
                MsgDispatcher.SendMsg((int)MessageType.Move);
                yield return new WaitForSeconds(0.2f);
            }
            unit.restrain = true;
            MsgDispatcher.SendMsg((int)MessageType.Aftermove);
        }
    }
}



