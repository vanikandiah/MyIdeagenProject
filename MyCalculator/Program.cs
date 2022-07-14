using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            string val;
            double computedValue;
            Console.WriteLine("Enter value: ");
            val = Console.ReadLine();
            Console.WriteLine("Your input is: {0}", val);
            computedValue = Calculate(val);
            Console.WriteLine("The answer is: {0}", computedValue.ToString());
            Console.Write("Enter value OR Press Enter to exit!: ");
            val = Console.ReadLine();

            while(val.Length > 0)
            {
                computedValue = Calculate(val);
                Console.WriteLine("The answer is: {0}", computedValue.ToString());
                Console.Write("Enter value OR Press Enter to exit!: ");
                val = Console.ReadLine();
            }            

        }
        /// <summary>
        /// This is the main method to calculate the formula
        /// </summary>
        /// <param name="sum">a text that consists numbers and operators is always separated by spaces</param>
        /// <returns></returns>
        public static double Calculate(string sum)
        {
            //Your code starts here 
            string formula = sum;
            double dblAnswer = 0.0;
            bool reCalculate = false;
            string[] equation = sum.Split(' ');
            try
            {
                if (equation.Length == 3)
                {
                    return CalculateEquation(equation[0], equation[2], equation[1]);
                }
                else
                {
                    //first check for brackets and compute the formulas in the bracket
                    bool isNestedBracket = CheckForNestedBracket(ref equation);
                    while (isNestedBracket)
                    {
                        isNestedBracket = CheckForNestedBracket(ref equation);
                    }

                    //Next check for multiplication or Division
                    reCalculate = PerformSubCalculation(ref equation, true);

                    //Finally check for addition or substraction
                    if (!reCalculate)
                    {
                        reCalculate = PerformSubCalculation(ref equation, false);
                    }

                    if (reCalculate)
                        dblAnswer = equation.Length == 1 ? Convert.ToDouble(equation[0]) : Calculate(String.Join(" ", equation));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Computing the calculation. Please make sure empty space in between each character. System Error: {0}", ex.Message);
            }           
            
            return dblAnswer;
        }
        /// <summary>
        /// This function check for brackets. If bracket is found, this is computed in priority 
        /// </summary>
        /// <param name="equation">an array of string</param>
        /// <returns></returns>
        private static bool CheckForNestedBracket(ref string[] equation)
        {
            List<int> openingBracketIndex = new List<int>();
            List<int> closingBracketIndex = new List<int>();
            bool isNestedBracket = false;
            for (int i = 0; i < equation.Length; i++)
            {
                if (equation[i].IndexOf('(') > -1)
                {
                    openingBracketIndex.Add(i);                 
                }
                if (equation[i].IndexOf(')') > -1)
                {
                    closingBracketIndex.Add(i);
                    if (openingBracketIndex.Count == 1)
                    {
                        ComputeNestedValue(openingBracketIndex.FirstOrDefault(), i, ref equation);
                        break;
                    }
                    else
                    {
                        ComputeNestedValue(openingBracketIndex.LastOrDefault(), i, ref equation);
                        isNestedBracket = true;
                        break;
                    }
                }
            }
            return isNestedBracket;
        }
        /// <summary>
        /// This function basically compute the values provided in the brackets
        /// </summary>
        /// <param name="openingBracketIndex">starting element index</param>
        /// <param name="closingBracketIndex">ending element index</param>
        /// <param name="equationList">an array of string</param>
        private static void ComputeNestedValue(int openingBracketIndex, int closingBracketIndex, ref string[] equationList)
        {
            string newEquation = "";
            double subValue = 0;
            if(equationList.Length > 0 && openingBracketIndex < closingBracketIndex)
            {
                //extract the formula within the bracket
                for(int i=openingBracketIndex+ 1; i<closingBracketIndex; i++)
                {
                    if (i == openingBracketIndex + 1)
                        newEquation = equationList[i];
                    else
                        newEquation = newEquation + " " + equationList[i];
                }

                //compute the values in the bracket
                if(!string.IsNullOrWhiteSpace(newEquation))
                {
                    subValue = Calculate(newEquation);
                }

                //replace the value with the opening bracket and then remove subsequence elements till closingbracket. 
                for (int i = openingBracketIndex ; i < closingBracketIndex+ 1; i++)
                {
                    if (i == openingBracketIndex)
                        equationList[i] = subValue.ToString();
                    else
                        equationList[i] = "@";
                }
                equationList = equationList.Where(val => val != "@").ToArray();
            }

        }
        /// <summary>
        /// This function compute the value by using operators such as *, /, +, -
        /// </summary>
        /// <param name="equation">an array of string</param>
        /// <param name="isMultiplyOrDivision">true or false flag to indicate the compute operators</param>
        /// <returns>true or false to indicate if recalculation is required or not</returns>
        private static bool PerformSubCalculation(ref string[] equation, bool isMultiplyOrDivision)
        {
            char[] minorSymbols = { '+', '-' };
            char[] mainSymbols = { '*', '/' };
            double dblAnswer;
            bool reCalculate = false;

            char[] checkingSymbols = null;
            if(isMultiplyOrDivision)
            {
                checkingSymbols = mainSymbols;
            }
            else
            {
                checkingSymbols = minorSymbols;
            }
            for (int i = 0; i < equation.Length; i++)
            {
                if (equation[i].IndexOfAny(checkingSymbols) > -1)
                {
                    dblAnswer = CalculateEquation(equation[i - 1], equation[i + 1], equation[i]);
                    equation[i - 1] = dblAnswer.ToString();
                    equation[i + 1] = "@";
                    equation[i] = "@";
                    equation = equation.Where(val => val != "@").ToArray();
                    reCalculate = true;
                    break;
                }
            }

            return reCalculate;
        }
        /// <summary>
        /// This is the main function that performs multiplication, division, addition and substraction
        /// </summary>
        /// <param name="value1">1st digit</param>
        /// <param name="value2">2nd digit</param>
        /// <param name="equationSymbol">operator (*, /, +, -)</param>
        /// <returns>computed value in numbers</returns>
        private static double CalculateEquation(string value1, string value2, string equationSymbol)
        {
            double dblValue1;
            double dblValue2;
            double dblAnswer;

            dblValue1 = string.IsNullOrWhiteSpace(value1) ? 0.0 : Convert.ToDouble(value1);
            dblValue2 = string.IsNullOrWhiteSpace(value2) ? 0.0 : Convert.ToDouble(value2);

            switch (equationSymbol)
            {
                case "+":
                    dblAnswer = dblValue1 + dblValue2;
                    break;
                case "-":
                    dblAnswer = dblValue1 - dblValue2;
                    break;
                case "*":
                    dblAnswer = dblValue1 * dblValue2;
                    break;
                case "/":
                    dblAnswer = dblValue1 / dblValue2;
                    break;
                default:
                    dblAnswer = 0.0;
                    break;
            }
            return dblAnswer;
        }
    }
}
