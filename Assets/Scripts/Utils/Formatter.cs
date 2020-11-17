using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class Formatter {

    private static readonly string[] shortles = { "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai",
                        "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az",
                        "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi",
                        "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz" };

    // public static string formatGameValue(long value) {
    //     string valueString = value.ToString();
    //     if (value < 1000000000) {
    //         valueString = insertStringEveryLetters(" ", valueString, 3, true);
    //     } else {

    //     }
    //     return valueString;
    // }

    // private static string insertStringEveryLetters(string insert, string dest, int letterCount, bool reverse) {
    //     int insertCount = dest.Length / letterCount;
    //     if (!reverse) {
    //         for (int i = 1; i <= insertCount; i++) {
    //             dest = dest.Insert(i * letterCount + (i - 1), insert);
    //         }
    //     } else {
    //         for (int i = 1; i <= insertCount; i++) {
    //             dest = dest.Insert(dest.Length - i * letterCount - (i - 1), insert);
    //         }
    //     }
    //     return dest.Trim();
    // }

    // public static string formatLevel(int lvl) {
    //     return "Level " + lvl.ToString();
    // }

    public static string formatNumber(BigDecimal number) {
        string numberStr = bigDecToString(number);
        int moveCount = 0;
        while (fullNumberLength(numberStr) - moveCount * 3 > 3) {
            moveCount++;
        }
        string res = numberStr;
        if (moveCount > 0) {
            int backEnd;
            if (numberStr.Contains(",")) {
                backEnd = numberStr.IndexOf(",");
            } else {
                backEnd = numberStr.Length;
            }

            res = numberStr.Insert(backEnd - moveCount * 3, ",");
        }
        string ress = res.Substring(0, Mathf.Min(4, res.Length));
        return cleanCommaZeros(ress) + shortles[moveCount];
    }

    private static int fullNumberLength(string num) {
        if (num.Contains(",")) {
            return num.IndexOf(",");
        } else {
            return num.Length;
        }
    }

    private static string cleanCommaZeros(string input) {
        if (!input.Contains(",")) return input;
        int cleanUpLength = input.Length;
        for (int i = input.Length - 1; i >= 0; i--) {
            if (input[i].Equals('0')) {
                cleanUpLength--;
                continue;
            } else if (input[i].Equals(',')) {
                cleanUpLength--;
            }
            break;
        }
        return input.Substring(0, cleanUpLength);
    }

    public static string bigDecToString(BigDecimal number) {
        if (number.Exponent >= 0) {
            string result = number.Mantissa.ToString();
            for (int i = 0; i < number.Exponent; i++) {
                result = result.Insert(result.Length, "0");
            }
            return result;
        } else {
            string num = number.Mantissa.ToString();
            if (num.Length + number.Exponent < 1) {
                string zeros = "";
                int count = Mathf.Abs(number.Exponent) - 1 - num.Length;
                for (int i = 0; i < count; i++) {
                    zeros = zeros.Insert(0, "0");
                }
                return num.Insert(0, "0," + zeros);
            }
            return num.Insert(num.Length + number.Exponent, ",");
        }
    }

    public static string formatTime(long timeSeconds, bool inShort) {
        long days = timeSeconds / (24 * 60 * 60);
        timeSeconds -= days * (24 * 60 * 60);
        long hours = timeSeconds / (60 * 60);
        timeSeconds -= hours * (60 * 60);
        long minutes = timeSeconds / 60;
        timeSeconds -= minutes * 60;
        long seconds = timeSeconds;
        int used = 0;
        string output = "";
        if (days > 0) {
            output = $"{days}d ";
            used++;
        }
        if (hours > 0) {
            output = output + $"{hours}h ";
            used++;
        }
        if (minutes > 0 && (inShort && used < 2)) {
            output = output + $"{minutes}m ";
            used++;
        }
        if (seconds > 0 && (inShort && used < 2)) output = output + $"{seconds}s";

        return output;
    }

}
