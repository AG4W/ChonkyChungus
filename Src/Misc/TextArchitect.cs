using System.Text.RegularExpressions;
using System;

public static class TextArchitect
{
    public static string Process(string input)
    {
        foreach (string word in input.Split(' ', '.', ',', '!', '?', '\n'))
        {
            if (word.Contains("vital:"))
                input = Regex.Replace(input, word, Vital.ToStringFormat((VitalType)Enum.Parse(typeof(VitalType), word.Split(':')[1], true)));
            else if (word.Contains("dice:"))
                input = Regex.Replace(input, word, DiceUtils.ToStringFormatted(int.Parse(word.Split(':')[1])));
        }

        return input;
    }
}
