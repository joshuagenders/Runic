using System.Collections.Generic;

namespace Runic.ExampleTest
{
    public class Ref
    {
        public enum Keys
        {
            CustomerId = 0,
            AuthCookie = 1,
            Area = 2
        }

        public static Dictionary<int, string> Indexes = new Dictionary<int, string>
        {
            {(int) Keys.AuthCookie, "AuthCookie"},
            {(int) Keys.CustomerId, "CustomerId"},
            {(int) Keys.Area, "Area"}
        };
    }
}