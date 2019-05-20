using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GameUtility
{
    /// <summary>
    /// 序列化工具
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// 对象序列化为内存流
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static MemoryStream InstanceDataToMemory(object instance)
        {
            //创建一个新的流来容纳经过序列化的对象
            MemoryStream memoStream = new MemoryStream();
            //创建一个序列化格式化器来执行具体的序列化操作
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            //将传入的对象instance序列化到流memoStream中
            binaryFormatter.Serialize(memoStream, instance);
            //返回序列化好的流
            return memoStream;
        }

        /// <summary>
        /// 内存流序列化为对象
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <returns></returns>
        public static object MemoryToInstanceData(Stream memoryStream)
        {
            //创建一个序列化格式化器来执行具体的反序列化操作
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            //返回从流memoryStream中反序列化得到的对象
            return binaryFormatter.Deserialize(memoryStream);
        }
    }
}
