using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameCard;
using LitJson;
using System.IO;
using GameGUI;
using UnityEditor;
namespace MainMap { 
/// <summary>定义六边形坐标的结构体，并处理坐标转换
/// 
/// </summary>
public struct HexVector
{
    /// <summary>
    /// 
    /// </summary>
    public Vector3 Hex_vector;
    /// <summary>世界坐标
    /// 
    /// </summary>
    public Vector3 Normal_vector;
    /// <summary>传入六边形坐标数值转换成世界坐标，返回调用者世界坐标并将结果写入两个Vector；
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public Vector3 ChangeToNormalVect(Vector3 vector)
    {
        Hex_vector = vector;
        Normal_vector.x = 1.5f * vector.x + 1.5f * vector.y;
        Normal_vector.z = vector.z;
        Normal_vector.y = -0.5f* (float)System.Math.Sqrt(3)*vector.x + 0.5f * (float)System.Math.Sqrt(3) * vector.y;
        vector = Normal_vector;
        return vector;

    }
    /// <summary>传入世界坐标数值转换成六边形坐标，返回调用者六边形坐标并将结果写入两个vector
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public Vector3 ChangeToHexVect(Vector3 vector)
    {
        Normal_vector = vector;
        Hex_vector.x = vector.x / 3f - ((float)System.Math.Sqrt(3) / 3f) * vector.y;
        Hex_vector.z = vector.z;
        Hex_vector.y = vector.x / 3f + ((float)System.Math.Sqrt(3)/3f) * vector.y;
        vector = Hex_vector;
        return vector;
    }

    }
/// <summary>在这个类里读取地图文件并生成地图
/// 
/// </summary>
public class MainMapManager : UnitySingleton<MainMapManager>
{
    public Mesh mesh;
    public TextAsset textAsset;
/// <summary>全部地格材质
/// 
/// </summary>
    public Sprite test;
    public Sprite mountainsprite;
    public Sprite planesplite;
    public Sprite deadtreesprite;
    public Sprite marshsprite;
    public Sprite bushsprite;
    public Sprite desertsprite;
    public Sprite greenerysprite;
    public Sprite oasissprite;
    public Sprite grasslandsprite;
    public Sprite wastelandsprite;
    public Sprite bloodstonesprite;
    public Sprite cursestonesprite;
    public Sprite bonepitsprite;
    public Sprite obsidiansprite;
/// <summary>初始化，设定
/// 
/// </summary>
void Awake()
    {
     Screen.SetResolution(960, 540, false);
     ReadMap();     
    }
/// <summary>通过读取文件里的字符串转换成对应的地格生成地图
/// 
/// </summary>
private void ReadMap()
   {
      System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;
      string[] lines = textAsset.text.Split(new char[] { '\r', '\n' }, option);
      for (int i = 0; i < lines.Length; i++)
         {
            string[] element = lines[i].Split(',');
            for (int j = 0; j < element.Length; j++)
                {
                  if (element[j] != "null")//如果字符串不为null,则生成地格挂载脚本。
                    {
                        string[] upper = element[j].Split(new char[] { ':' }, option);
                        GameObject mapunit = new GameObject("test" + i.ToString() + j.ToString());
                        mapunit.transform.parent = GameObject.Find("Map").transform;
                        mapunit.AddComponent<Button>();
                        switch (upper[0])
                        {
                            case "plane":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = planesplite;
                                break;
                            case "mountain":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = mountainsprite;
                                break;
                            case "deadtree":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = deadtreesprite;
                                break;
                            case "marsh":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = marshsprite;
                                break;
                            case "bush":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = bushsprite;
                                break;
                            case "desert":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = desertsprite;
                                break;
                            case "greenery":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = greenerysprite;
                                break;
                            case "oasis":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = oasissprite;
                                break;
                            case "grassland":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = grasslandsprite;
                                break;
                            case "wasteland":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = wastelandsprite;
                                break;
                            case "bloodstone":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = bloodstonesprite;
                                break;
                            case "cursestone":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = cursestonesprite;
                                break;
                            case "bonepit":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = bonepitsprite;
                                break;
                            case "obsidian":
                                mapunit.AddComponent<Plane>();
                                mapunit.transform.position = mapunit.GetComponent<Plane>().hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = obsidiansprite;
                                break;
                            //这里用的是默认的地格素材，
                            case "post":
                                MapUnit post = mapunit.AddComponent<Library>();
                                mapunit.transform.position = post.hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = test;
                                break;
                            case "key":
                                MapUnit key = mapunit.AddComponent<Key>();
                                mapunit.transform.position = key.hexVector.ChangeToNormalVect(new Vector3(i, j, 0));
                                mapunit.AddComponent<SpriteRenderer>();
                                mapunit.GetComponent<SpriteRenderer>().sprite = test;
                                break;
                            default:
                                Debug.Log("你文件写错了，回去看看");
                                break;
                        }

                        MeshCollider collider = mapunit.AddComponent<MeshCollider>();
                        collider.sharedMesh = mesh;
                        mapunit.transform.localScale= new Vector3(0.5f, 0.5f, 0.5f);

                        //如果有地格上层元素，传给MapElementManager处理
                        if (upper.Length == 2)
                        {
                            MapElementManager.Instance().InstalizeElement(upper[1],mapunit);                        
                        }
                        else
                        {
                            
                        }

                    }
                }
            }
        }
        /// <summary>获取角色实例，并初始化AroundList字典
        /// 
        /// </summary>
    private void Start()
    {
           // Charactor.Instance() = GameObject.Find("Charactor").GetComponent<Charactor>();
            Charactor.Instance().aroundlist.Add("0,1", null);
            Charactor.Instance().aroundlist.Add("0,-1", null);
            Charactor.Instance().aroundlist.Add("1,0", null);
            Charactor.Instance().aroundlist.Add("-1,0", null);
            Charactor.Instance().aroundlist.Add("-1,1", null);
            Charactor.Instance().aroundlist.Add("1,-1", null);
            Charactor.Instance().CharactorInitalize();
    }

}
/// <summary>六边形单元格的抽象类，每个地格都会继承这个类
/// 
/// </summary>
public abstract class MapUnit:MonoBehaviour
{
    public HexVector hexVector = new HexVector();
    public Button btn;
    /// <summary>初始化地格，获得所在实例的按钮组件并监听事件
    /// 
    /// </summary>
    public virtual void MapUnitInstalize()
    {
     //   mapManager = gameObject.GetComponentInParent<MainMapManager>();
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }
    /// <summary>点击事件抽象方法，需要每类地格具体实现
    /// 
    /// </summary>
    public abstract void OnClick();
    /// <summary>移动地图角色的方法，改变角色位置并调用charactor里的方法重写字典值，图书馆传送和普通移动都会调用这个方法
    /// 
    /// </summary>
    public virtual void ChangePosition(int step)
    {
            if(Charactor.Instance().charactordata.charactorstate == MoveState.MotionLess)
            {

                Charactor.Instance().Move(GetComponent<Transform>().position, -step);
            }
            else
            {
                Debug.Log("角色非静止");
            }

          }
        /// <summary>charactor中人物移动完成后会调用这个函数，用于响应地格上事件
        /// 
        /// </summary>
    public virtual void ChangePositionOver()
        {
            Debug.Log("移动结束");
            if (GetComponentInChildren<MapElement>() != null)
            {
                GetComponentInChildren<MapElement>().ElementOnClick();
            }
        }
}
/// <summary>普通的地格，没什么特别的
/// 
/// </summary>
public class Plane : MapUnit
{
    /// <summary>初始化，给所在GameObject添加Button组件，并把自己在两个坐标系的坐标写入HexVector里。
    /// 
    /// </summary>  
    public void Awake()
    {
        //普通地格默认监听CheckAround,特殊地格会重新监听各自的新事件
        MapUnitInstalize();
        Debug.Log("普通地面初始化.");

    }

    /// <summary>检测被点击的地格是否在角色相邻区域，
    /// 
    /// </summary>
    public override void OnClick()
    {
            if  (Charactor.Instance().aroundlist.ContainsValue(this))
        {
                ChangePosition(1);
        }
        else
        {
            Debug.Log("这个格子不在角色相邻区域，无法移动");
        }
    }

}
/// <summary>每个图书馆都会挂这个脚本，负责控制传送逻辑
/// 
/// </summary>
public class Library : MapUnit
{
    public static List<Library> activelibrarylist = new List<Library>();
    /// <summary>图书馆是否激活
    /// 
    /// </summary>
    private bool isActive = false;
    /// <summary>角色踩在图书馆上会把ReadyToTrans设置为true
    /// 
    /// </summary>
    private static bool ReadyToTrans = false;
    public void Awake()
    {
        Debug.Log("图书馆初始化");
        MapUnitInstalize();
    }
    /// <summary>点击图书馆格子后触发的事件
    /// 
    /// </summary>
    public override void OnClick()
    {
        if (Charactor.Instance().aroundlist.ContainsValue(this) && ReadyToTrans == false)
        {
            ChangePosition(1);                    
        }
    #region 弃用的代码
            //else if (ReadyToTrans == true)
            //{

            //    if (isActive == false)
            //    {
            //        Debug.Log("所选图书馆未激活。");
            //    }
            //    else
            //    {
            //        Debug.Log("指令合法，开始传送");
            //        transfer();
            //        ReadyToTrans = false;
            //        Debug.Log("传送完成");
            //    }
            //}
            #endregion
        else
        {
            Debug.Log("你不在这个图书馆");
        }
    }
    public override void ChangePositionOver()
        {
            isActive = true;
            Debug.Log("图书馆已激活");
            Debug.Log("进入图书馆");
            MainMapUI.Instance().ShowlibraryUI(this);
            activelibrarylist.Add(this);
            #region 弃用的代码
            ////如果放弃传送移动到图书馆相邻格子会重新把readytotrans设置为false,这里实现的很蠢，等结合UI就可以通过按钮监听canceltrans了0.0
            //foreach (MapUnit unit in Charactor.Instance().aroundlist.Values)
            //{

            //    if (unit != null)
            //    {
            //        unit.gameObject.GetComponent<Button>().onClick.AddListener(CancelTrans);

            //    }
            //    else
            //    {

            //    }
            //}
            #endregion
        }
        public static void PrepareTrans()
        {
            ReadyToTrans = true;
            Debug.Log("准备传送");
        }
    /// <summary>传送的具体实现
    /// 
    /// </summary>
    public void transfer()
    {
        ChangePosition(2);
        ReadyToTrans = false;
    }
    public void CancelTrans()
    {
        ReadyToTrans = false;
    }
 
    }
/// <summary>钥匙（虚构层为各种可以清理路障的道具,地图文件中对应字符串为key）
/// 
/// 
/// </summary>
public class Key : MapUnit
{
    public string form;//样式
    public void Awake()
    {
        Debug.Log("清障道具初始化");
        MapUnitInstalize();
    }
    public override void OnClick()
    {
        
        if (Charactor.Instance().aroundlist.ContainsValue(this))
        {
            ChangePosition(1);
            Debug.Log("获取合法");
            ///Todo:调用MapManager里的charactor实例的获取道具方法
        }
        else
        {
            Debug.Log("不在相邻地格，无法取得道具");
        }
    }
}
/// <summary>锁（虚构层表现为各种类型的障碍，需要对应的钥匙（清障道具）方可清除并到达此地格）
/// 
/// </summary>
public class Barrier : MapUnit
{
    public string form;
    public  void Awake()
    {
        Debug.Log("路障初始化");
    }
    public override void OnClick()
    {
        //Todo:检测角色背包里是否有相匹配的钥匙，因为我还没写背包，所以就先晾在这了。    
    }
}
}

    