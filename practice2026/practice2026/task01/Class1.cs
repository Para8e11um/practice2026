using System;

namespace task01
{
    public static class StringExtension
    {
        public static bool IsPalindrome(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            int left = 0;
            int right = str.Length - 1;
            bool hasValidChars = false;
            while (left <= right)
            {
                if (char.IsPunctuation(str[left]) || char.IsWhiteSpace(str[left]))
                {
                    left++;
                    continue;
                }
                if (char.IsPunctuation(str[right]) || char.IsWhiteSpace(str[right]))
                {
                    right--;
                    continue;
                }
                hasValidChars = true;
                if (char.ToLower(str[left]) != char.ToLower(str[right]))
                {
                    return false;
                }

                left++;
                right--;
            }
            return hasValidChars;
        }
    }
}