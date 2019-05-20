using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

namespace Encounter
{
    public class EncounterData
    {
        #region 变量
        //初始化遭遇
        [SerializeField] private string EncounterPath = "/Scripts/BattleMap/encounter.json";//遭遇事件文件路径
        private Dictionary<string, JsonData> encounterData = new Dictionary<string, JsonData>();//遭遇id和对用的Jsondata对象
        private Dictionary<int, Dictionary<string, int>> eventDic = new Dictionary<int, Dictionary<string, int>>();//战区id和对应的战区事件；
        #endregion


        ///<summary>
        /// 初始并存储遭遇
        ///</summary>
        public void InitEncounter()
        {
            encounterData = new Dictionary<string, JsonData>();
            JsonData data = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + EncounterPath));
            int encouterCount = data.Count;
            for (int i = 0; i < encouterCount; i++)
            {
                string encounterID = data[i]["EncounterID"].ToString();
                this.encounterData.Add(encounterID, data[i]);
            }
        }

        /// <summary>
        /// 初始战区事件
        /// </summary>
        /// <param name="encounterID"></param>
        public void InitBattlefield(string encounterID)
        {
            JsonData jsonData = null;//整个遭遇Json对象
            encounterData.TryGetValue(encounterID, out jsonData);

            JsonData battleFieldData = jsonData["BattlefieldMessage"];//战区事件的data
            for (int i = 0; i < battleFieldData.Count; i++)
            {
                int regionID = (int)battleFieldData[i]["RegionID"]; //该战区在地图文件中的“战区区分编号”
                eventDic.Add(regionID, JsonToDictionary(battleFieldData[i]["EventList"]));
            }
        }
        /// <summary>
        /// Json转字典，只支持{"a":1,"b":1,"c":1}格式,其中a为key，1为value
        /// </summary>
        /// <param name="a"></param>
        private Dictionary<string,int> JsonToDictionary(JsonData a)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            string b = a.ToJson(); //转成字符串
            string c = b.Replace("{", "").Replace("}", "").Replace("\"", "");//"去掉",{等字符
            string[] d = c.Split(',');//分割
            string[][] e = new string[d.Length][];
            for (int i = 0; i < e.Length; i++)//再次分割
            {
                e[i] = d[i].Split(':');
            }
            for (int i = 0; i < e.Length; i++)
            {
                dic.Add(e[i][0],int.Parse(e[i][1]));
            }
            return dic;
        }

        /// <summary>
        /// 获取遭遇id，给大地图的接口
        /// </summary>
        /// <param name="encounterID">遭遇id</param>
        public void InitEncounter(string encounterID)
        {
            JsonData jsonData = null;
            encounterData.TryGetValue(encounterID, out jsonData);
            BattleMap.BattleMap.Instance().InitBattleMapPath(jsonData["MapID"].ToString());
        }

        /// <summary>
        /// 通过遭遇ID获取对应的遭遇data
        /// </summary>
        /// <param name="encounterID">遭遇id</param>
        public JsonData GetEncounterDataByID(string encounterID)
        {
            if (encounterID == null)
                return null;

            return encounterData[encounterID];
        }

        /// <summary>
        /// 根据战区id，获取该战区上的事件data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Dictionary<string,int> GetBattleFieldDataByID(int id)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            eventDic.TryGetValue(id, out dict);
            return dict;
        }

    }
}