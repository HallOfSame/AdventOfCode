namespace Helpers.Extensions;

public static class StringExtensions
{
    public static string ReplaceAt(this string input, int index, char newChar)
    {
        var arr = input.ToCharArray();
        arr[index] = newChar;
        return new string(arr);
    }
}