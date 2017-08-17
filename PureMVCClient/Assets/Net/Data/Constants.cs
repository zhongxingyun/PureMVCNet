using UnityEngine;
using System.Collections;

/// <summary>
/// 常量数据
/// </summary>
public class Constants
{
    //消息：数据总长度(4byte) + 数据类型(4byte) + 数据(N byte)
    public static int HEAD_DATA_LEN = 4;
    public static int HEAD_TYPE_LEN = 4;
    public static int HEAD_LEN//8byte
    {
        get { return HEAD_DATA_LEN + HEAD_TYPE_LEN; }
    }
}
