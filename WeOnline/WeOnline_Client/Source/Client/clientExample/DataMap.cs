
namespace clientExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DataMap
    {
        /// <summary>
        /// Serializes a list to a string
        /// </summary>
        /// <param name="lst">The list you wish to serialize</param>
        /// <returns>A serialized string that represents the list</returns>
        public static string Serialize(List<string> lst)
        {
            var output = new StringBuilder();
            foreach (var str in lst)
            {
                string inter = str.Count().ToString().Count() + str.Count().ToString() + str;
                output.Append(inter);
            }
            return output.ToString();
        }

        /// <summary>
        /// Deserializes a list from a string that represents the serialized list
        /// </summary>
        /// <param name="input">The string to deserialize from</param>
        /// <returns>A list that represents the serialized items</returns>
        public static List<string> Deserialize(this string input)
        {
            string str = input;
            var items = new List<string>();
            if (string.IsNullOrEmpty(str))
            {
                return items;
            }
            int index = 0;
            try
            {
                while (index < str.Count())
                {
                    int indexLen = Convert.ToInt32(str[index].ToString());
                    index++;
                    int keyLen = Convert.ToInt32(str.Substring(index, indexLen));
                    index += indexLen;
                    string key = str.Substring(index, keyLen);
                    index += keyLen;
                    items.Add(key);
                }
            }
            catch
            {
                return null;
            }
            return items;
        }
    }
}
