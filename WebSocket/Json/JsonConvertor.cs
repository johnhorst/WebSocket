using System.Web.Script.Serialization;

namespace WebSocket.Json
{
    public static class JsonConvertor 
    {       
        public static string ToJSON(object obj)
        {
            if (obj is IJson)
                return ((IJson)obj).Stringify();
            return new JavaScriptSerializer().Serialize(obj);
        }      

        public static T FromJSON<T>(string obj)
        {            
            return new JavaScriptSerializer().Deserialize<T>(obj);
        }      
    }
}
