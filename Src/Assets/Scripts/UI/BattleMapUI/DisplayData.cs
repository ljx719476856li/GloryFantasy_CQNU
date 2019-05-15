using UnityEngine;
using System.Collections;
using Unit = GameUnit.GameUnit;

namespace GameGUI
{
    public class DisplayData : MonoBehaviour
    {

        //主摄像机对象
        private Camera selfCamera;

        private float unitHeight;
        private string selfName = "0.0";

        public Unit unit;

        private void Awake()
        {
            unit = gameObject.GetComponent<Unit>();
        }

        void Start()
        {
            //得到摄像机对象
            selfCamera = Camera.main;

            //float size_y = GetComponent<Collider2D>().bounds.size.y;
            //得到模型缩放比例
            //float scal_y = transform.localScale.y;
            //它们的乘积就是高度
            //unitHeight = (size_y *scal_y)/ 2.0f ;
        }

        void Update()
        {
            //name = string.Format("{0}  {1}", unit.hp, unit.atk);
        }

        void OnGUI()
        {
            Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + unitHeight, transform.position.z);
            Vector2 position = selfCamera.WorldToScreenPoint(worldPosition);
            position = new Vector2(position.x, Screen.height - position.y);

            Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(selfName));
            GUI.color = Color.red;
            GUI.skin.label.fontSize = 22;
            GUI.Label(new Rect(position.x - (nameSize.x / 2), position.y - nameSize.y, nameSize.x, nameSize.y), selfName);
        }

    }
}