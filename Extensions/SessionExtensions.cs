using Microsoft.AspNetCore.Http;

namespace cars_web.Extensions
{
    public static class SessionExtensions
    {
        public static void SetBoolean(this ISession session, string key, bool value)
        {
            session.SetInt32(key, value ? 1 : 0);
        }

        public static bool? GetBoolean(this ISession session, string key)
        {
            var value = session.GetInt32(key);
            if (value == null)
            {
                return null;
            }
            
            return value == 1;
        }
    }
} 