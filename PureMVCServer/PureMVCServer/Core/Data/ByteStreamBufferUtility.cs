using System;
using System.Collections;
using Newtonsoft.Json;

namespace PureNet.Data
{
    public static class ByteStreamBufferUtility
    {
        public static void WriteObject(this ByteStreamBuffer steamBuffer, object _object)
        {
            try
            {
                string jsonObject = JsonConvert.SerializeObject(_object);
                steamBuffer.Write_UTF8String(jsonObject);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static T ReadObject<T>(this ByteStreamBuffer steamBuffer) where T : class
        {
            string jsonObject = steamBuffer.Read_UTF8String();
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonObject);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
