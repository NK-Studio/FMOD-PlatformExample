using System.Text.RegularExpressions;
using UnityEditor;

namespace Smart
{
	public partial class Inspector
    {
        //
        // Property Search
        //

        bool SearchFilter(Editor editor)
        {
            if (null == editor) { return false; }

            if (filter == "") { return false; }

            string name = editor.target.GetType().Name;

            if (matchWord)
            {
                return !Regex.IsMatch(filter, string.Format(@"\b{0}\b", name));
            }

            name = name.ToLower();

            return !name.Contains(filter.ToLower());
        }

        bool SearchProperty(string search, string target)
        {
            if (string.IsNullOrWhiteSpace(search)) { return false; }

            target = target.Replace(" ", string.Empty);

            if (matchWord)
            {
                return Regex.IsMatch(search, string.Format(@"\b{0}\b", target));
            }

            target = target.ToLower();

            return target.Contains(search.ToLower());
        }

        bool searching
        {
            get => !string.IsNullOrWhiteSpace(filter);
        }
    }
}
