using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{


    public static class Evaluator
    {
        public delegate int Lookup(String v);
        /// <summary>
        /// This method will take in any String an evaluate whether or not it is a valid expression. If it is, it will return the numerical value that the Expression represents. Otherwise it will throw an Exception.
        /// </summary>
        /// <param name="exp">The Expression</param>
        /// <param name="variableEvaluator">A delegate pass in for variable purposes</param>
        /// <returns>The number if valid Expression, Exeption if invaild.</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            String[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<String> Operator = new Stack<string>();
            Stack<int> value = new Stack<int>();

            foreach (string token in substrings) // for each token in the Regex String
            {

                string trimmedtoken = token.Trim(); // trim the white space 

                if (trimmedtoken == "+" || trimmedtoken == "-") //add or sub if this token is + or -
                {

                    if (Operator.IsOnTop("+") || Operator.IsOnTop("-"))
                    {
                        addorsub();
                        Operator.Push(trimmedtoken);
                    }
                    else
                    {
                        Operator.Push(trimmedtoken);
                    }
                }
                else if (trimmedtoken == "*" || trimmedtoken == "/")
                {
                    Operator.Push(trimmedtoken);
                } //Multiply or Divide if the token is * or /
                else if (trimmedtoken == "(") // add top (
                {
                    Operator.Push(trimmedtoken);
                }
                else if (trimmedtoken == ")") // add ) and compute math inside the ()
                {
                    if (Operator.IsOnTop("+") || Operator.IsOnTop("-"))
                    {
                        addorsub();
                    }

                    if (!Operator.IsOnTop("("))
                    {
                        throw new ArgumentException();
                    }
                    else
                    {
                        Operator.Pop();
                    }

                    if (Operator.IsOnTop("*") || Operator.IsOnTop("/"))
                    {
                        multordev(value.Pop());

                    }

                }
                else if (Regex.IsMatch(trimmedtoken, "^[0-9]+$")) //detects any numbers
                {

                    if (Operator.IsOnTop("*") || Operator.IsOnTop("/"))
                        multordev(int.Parse(trimmedtoken));
                    else
                    {
                        int Stringtonumber = int.Parse(trimmedtoken);
                        value.Push(Stringtonumber);
                    }
                }

                else if (trimmedtoken == " " || trimmedtoken == "")
                {

                }
                else if (Regex.IsMatch(trimmedtoken, "^[a-zA-Z]+[1-9][0-9]*$"))//detects valid variables
                {
                    if (Operator.IsOnTop("*") || Operator.IsOnTop("/"))
                    {
                        if (value.Count == 0)
                        {
                            throw new ArgumentException();
                        }

                        String Popedoperator = Operator.Pop();
                        if (Popedoperator == "/")
                        {
                            int top = value.Pop();
                            int bottom = variableEvaluator(trimmedtoken);
                            if (bottom == 0)
                            {
                                throw new ArgumentException("division by 0");
                            }


                            int division = top / bottom;
                            value.Push(division);
                        }

                        else if (Popedoperator == "*")
                        {
                            int value1 = value.Pop();

                            int multiply = variableEvaluator(trimmedtoken) * value1;
                            value.Push(multiply);

                        }
                    }

                    else
                    {
                        value.Push(variableEvaluator(trimmedtoken));
                    }
                }

            }


            if (Operator.Count == 0)//this mess determines if the output is a valid expression
            {
                if (value.Count != 1)
                    throw new ArgumentException();
                return value.Pop();
            }
            else if (Operator.Count == 1 && value.Count == 2)
            {
                addorsub();
            }
            else if (value.Count == 0 || Operator.Count >= 1)
                throw new ArgumentException("No number on value stack or Operators > 1");
            


            return value.Pop();//otherwise return the value.

            ///This is a helper method that when called will add the top two values and operator IF its a + or - on top of the OPerators.
            void addorsub()
            {
                if (Operator.IsOnTop("+") || Operator.IsOnTop("-"))
                {
                    if (value.Count < 2)
                    {
                        throw new ArgumentException("value stack has less then two numbers");
                    }

                    String operator1 = Operator.Pop();

                    if (operator1 == "-")
                    {
                        int value1 = value.Pop();
                        int value2 = value.Pop();
                        int number = value2 - value1;
                        value.Push(number);
                    }
                    else if (operator1 == "+")
                    {
                        int value1 = value.Pop();
                        int value2 = value.Pop();
                        int number = value1 + value2;
                        value.Push(number);
                    }
                }
            }
            ///this is a helper Method that will multply or devide the top two numbers IF there is a * or / on top of the OPerators.
            void multordev(int number)
            {

                if (Operator.IsOnTop("*") || Operator.IsOnTop("/"))
                {
                    if (value.Count == 0)
                    {
                        throw new ArgumentException();
                    }

                    String Popedoperator = Operator.Pop();
                    if (Popedoperator == "/")
                    {
                        int top = value.Pop();
                        int bottom = number;

                        if (bottom == 0)
                        {
                            throw new ArgumentException("division by 0");
                        }


                        int division = top / bottom;
                        value.Push(division);
                    }

                    else if (Popedoperator == "*")
                    {
                        int value1 = value.Pop();
                        int multiply = number * value1;
                        value.Push(multiply);

                    }
                }
            }








        }






    }
    /// <summary>
    /// This Extention to the Stack method allows us to call .Peek() without it throwing an expection ever time we use it to see the top of the stack. 
    /// </summary>
    static class PS1StackExtension
    {
        /// <summary>
        /// Returns the value of the Top of the Stack provided
        /// </summary>
        /// <typeparam name="T">Standard Generic Token</typeparam>
        /// <param name="Stack">The stack the user wises to peek on</param>
        /// <param name="val">Any String/Int that may be on top of the Stack</param>
        /// <returns>True if Val == the .Peek() value. False if it does not.</returns>
        public static bool IsOnTop<T>(this Stack<T> Stack, T val)
        {

            if (Stack.Count == 0)
                return false;

            return Stack.Peek().Equals(val);
        }
    }












}