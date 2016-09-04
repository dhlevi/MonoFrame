using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace MonoFrame
{
    public enum DiceType { D4, D6, D8, D10, D12, D20, D24, D30 }

    /// <summary>
    /// A Random "Dice" rolling utility class. Dice allows for rolling a specific dice
    /// by method call, or supplying a format string of #D#, for example 2D6. 
    /// </summary>
    public class Dice
    {
        private static Random rand = new Random(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
        /// <summary>
        /// formatted dice string
        /// 1D6, 4D6, for example
        /// comma seperate for groups
        /// ex: 1D6,5D12
        /// Groups will always be summed together.
        /// In place of groups, you can also use basic math operations, + - * / (no backslash!)
        /// In which event, groups will be summed, subtracted, multiplied or divided.
        /// ex: 1D6+4D6-1D2/12D12*7D2
        /// Note that the value returned will always round up to an integer, even if a fraction would result
        /// </summary>
        /// <param name="diceFormatString"></param>
        /// <returns></returns>
        public static int Roll(string diceFormatString)
        {
            int result = 0;

            //remove any whitespace from the string, just in case. May as well force upper case while we're at it
            diceFormatString = new string(diceFormatString.Where(c => !Char.IsWhiteSpace(c)).ToArray()).ToUpper();

            // if we have an emptry string, kick back a zero
            if (string.IsNullOrEmpty(diceFormatString)) return 0;

            try
            {
                // does the input contain any commas?
                bool hasCommas = diceFormatString.Contains(",");
                // does the input contain any operators?
                bool hasOperators = diceFormatString.Contains("+") || diceFormatString.Contains("-") || diceFormatString.Contains("*") || diceFormatString.Contains("/");
                // does the input contain any letters that aren't D?
                bool hasInvalidChars = !Regex.Match(diceFormatString, @"^[dD0-9+\-*/]*$").Success;
                
                // bad string, so kick a zero back
                // error casses are, we have commas and operators, we have invalid characters, or we have no commas and no operators, but multiple Dice indicators
                if ((hasCommas && hasOperators) 
                    || hasInvalidChars 
                    || (!hasCommas && !hasOperators && diceFormatString.Count(c => c == 'D') > 1))
                    return 0;

                if (hasCommas)
                {
                    string[] diceGroups = diceFormatString.ToUpper().Split(',');
                    foreach (string group in diceGroups)
                    {
                        string[] dice = group.ToUpper().Split('D');
                        int totalDice = int.Parse(dice[0]);
                        int sides = int.Parse(dice[1]);

                        result += Roll(totalDice, sides);
                    }
                }
                else if (hasOperators)
                {
                    string formattedMathString = "";
                    // 1D6+4D6-1D2/12D12*7D2
                    
                    // how many dice do we have to roll?
                    int diceRemaining = diceFormatString.Count(c => c == 'D');

                    // go through the string, and roll all of the dice. Create a nice new string
                    // containing just the rolled integers, and the mathematic operators
                    for(int i = 0; i < diceRemaining; i++)
                    {
                        if (i == diceRemaining - 1)
                        {
                            // this is the last dice, so we can make this easier.
                            string[] dice = diceFormatString.Split('D');
                            int totalDice = int.Parse(dice[0]);
                            int sides = int.Parse(dice[1]);

                            formattedMathString += Roll(totalDice, sides);
                            diceFormatString = null;
                        }
                        else
                        {
                            int nextD = diceFormatString.IndexOf('D');

                            string rst = diceFormatString.Substring(0, nextD);

                            int totalDice = int.Parse(diceFormatString.Substring(0, nextD)); // we can determine the total dice to roll by substring of the front of the string to the next "D" character

                            diceFormatString = diceFormatString.Substring(nextD + 1); // remove the characters from the string, we dont' need them anymore, including the D

                            int nextNonNumericCharacter = diceFormatString.ToList().FindIndex(c => !Char.IsNumber(c)); // locate the index of the next non-numeric chatacter. This will be an operator of some kind (+ - / *)

                            int sides = int.Parse(diceFormatString.Substring(0, nextNonNumericCharacter)); // chars from the beginning of the string to the next non-numeric character will give us the "sides"

                            int rolledValue = Roll(totalDice, sides); // now that we have the dice and the sides, we can roll and get a value

                            formattedMathString += rolledValue + diceFormatString.Substring(nextNonNumericCharacter, 1); // lets add the value, plus the next operator to the format string
                            diceFormatString = diceFormatString.Substring(nextNonNumericCharacter + 1); // remove the sides and operator from the string, we don't need them
                            // and we loop back around and do it all again if there are more dice to look at.
                        }
                    }

                    // we've processed the passed in string and created the formatted math string. Time to math!
                    // so... we need an expression parser for this, but I didn't want to include a 3rd party
                    // library (ncalc) and was feeling too lazy to write my own... 
                    // So we'll do this an ugly way. Cross your fingers and hope the magic
                    // of DataTable doesn't change. If it does, this implementation will have to change to 
                    // something a little more robust.

                    DataTable table = new DataTable();
                    table.Columns.Add("formattedMathString", typeof(string), formattedMathString);
                    DataRow row = table.NewRow();
                    table.Rows.Add(row);
                    result =  (int)Math.Round(double.Parse((string)row["formattedMathString"]), 0);
                }
                else
                {
                    // no comma, no operator, just a single dice to roll
                    string[] dice = diceFormatString.Split('D');
                    int totalDice = int.Parse(dice[0]);
                    int sides = int.Parse(dice[1]);

                    result = Roll(totalDice, sides);
                }
            }
            catch (Exception e)
            {
                // If I were to wager a guess, I'd expect this to be a number format exception...
                // or something in the regex failed.
                // either way, just kick back a zero on failures.
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Rolls a list of formatted strings. This counts as a group, and the results will be summed together
        /// </summary>
        /// <param name="formattedStringList"></param>
        /// <returns>The sum of all rolls in the list</returns>
        public static int Roll(List<string> formattedStringList)
        {
            int result = 0;

            foreach(string formattedString in formattedStringList)
            {
                result += Roll(formattedString);
            }

            return result;
        }

        public static int RollD4()
        {
            return Roll(1, 4);
        }

        public static int RollD6()
        {
            return Roll(1, 6);
        }

        public static int RollD8()
        {
            return Roll(1, 8);
        }

        public static int RollD10()
        {
            return Roll(1, 10);
        }

        public static int RollD12()
        {
            return Roll(1, 12);
        }

        public static int RollD20()
        {
            return Roll(1, 20);
        }

        public static int RollD24()
        {
            return Roll(1, 24);
        }

        public static int RollD30()
        {
            return Roll(1, 12);
        }

        public static int Roll(int totalDice, DiceType diceType)
        {
            int result = 0;

            switch(diceType)
            {
                case DiceType.D4:
                    result = Roll(totalDice, 4);
                    break;
                case DiceType.D6:
                    result = Roll(totalDice, 6);
                    break;
                case DiceType.D8:
                    result = Roll(totalDice, 8);
                    break;
                case DiceType.D10:
                    result = Roll(totalDice, 10);
                    break;
                case DiceType.D12:
                    result = Roll(totalDice, 12);
                    break;
                case DiceType.D20:
                    result = Roll(totalDice, 20);
                    break;
                case DiceType.D24:
                    result = Roll(totalDice, 24);
                    break;
                case DiceType.D30:
                    result = Roll(totalDice, 30);
                    break;
                default:
                    result = Roll(totalDice, 6);
                    break;
            }

            return result;
        }

        public static int Roll(int totalDice, int sides)
        {
            int result = 0;

            if (sides > 0)
            {
                for (int i = 0; i < totalDice; i++)
                {
                    result += rand.Next(1, sides);
                }
            }

            return result;
        }

        public static int Roll(Dictionary<DiceType, int> dice)
        {
            int result = 0;
            int totalDice = 1;

            if (dice.Count > 0)
            {
                totalDice = dice.Values.Sum();

                foreach(KeyValuePair<DiceType, int> diceGroup in dice)
                {
                    result += Roll(diceGroup.Value, diceGroup.Key);
                }
            }

            return result;
        }
    }
}
