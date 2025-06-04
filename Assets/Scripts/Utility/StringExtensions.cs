// -----------------------------------------------------------------------------
//  StringExtensions.cs  – tiny helper to abbreviate long hex / wallet addresses
// -----------------------------------------------------------------------------
namespace BiotonicFrontiers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns something like <c>0x1234…abcd</c> so UI labels stay short.
        /// </summary>
        public static string Short(this string value, int head = 6, int tail = 4)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= head + tail)
                return value;

            return $"{value.Substring(0, head)}…{value[^tail..]}";
        }
    }
}
