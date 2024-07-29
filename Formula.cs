// Skeleton written by Joe Zachary and Connor Cousineau for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private readonly string validFormula;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
        this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            IEnumerable<string> tokens = new List<String>(GetTokens(formula));
            validFormula = "";

            if (tokens.Count() < 1)
            {
                throw new FormulaFormatException("Only one token in the Formual");
            }
            if (Regex.IsMatch(tokens.First(), @"^[\+\-*/]$") || tokens.First() == ")")
            {

                throw new FormulaFormatException("Invalid First Token");

            }
            if (Regex.IsMatch(tokens.Last(), @"^[\+\-*/]$") || tokens.Last() == "(")
            {

                throw new FormulaFormatException("Invalid Last Token");

            }
            int Parenthesiscount = 0;
            String lastToken = "";
            foreach (String token in tokens)
            {

                if (token == "(")
                {
                    if (lastToken == ")" || Regex.IsMatch(token, @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$") || Regex.IsMatch(token, @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"))
                    {
                        throw new FormulaFormatException("Misplaced opening parenthesis in formula");
                    }
                    Parenthesiscount++;
                    if (Parenthesiscount < 0)
                        throw new FormulaFormatException("Right Parenthesis Rule");
                    validFormula += token;

                }
                else if (token == ")")
                {
                    if (lastToken == "(" || Regex.IsMatch(lastToken, @"^[\+\-*/]$"))
                    {
                        throw new FormulaFormatException("Invalid token: misplaced operator or parenthesis found");
                    }
                    Parenthesiscount--;
                    if (Parenthesiscount < 0)
                        throw new FormulaFormatException("Right Parenthesis Rule");
                    validFormula += token;
                }
                else if (token == "+" || token == "-" || token == "/" || token == "*")
                {
                    if (lastToken == "(" || Regex.IsMatch(lastToken, @"^[\+\-*/]$"))
                    {
                        throw new FormulaFormatException("Invalid token: misplaced operator or parenthesis found");
                    }
                    validFormula += token;
                }
                else if (Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                {
                    String normalizedToken = normalize(token);
                    if (Regex.IsMatch(normalizedToken, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                    {
                        if (!isValid(normalizedToken))
                        {
                            throw new FormulaFormatException("Invalid Varible");
                        }

                    }
                    if (lastToken == ")" || Regex.IsMatch(lastToken, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*") || Regex.IsMatch(lastToken, @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"))
                    {
                        throw new FormulaFormatException("Misplaced number in formula");
                    }
                    validFormula += normalizedToken;
                }
                else if (Regex.IsMatch(token, @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"))
                {
                    if (lastToken == ")" || Regex.IsMatch(lastToken, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*") || Regex.IsMatch(lastToken, @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"))
                    {
                        throw new FormulaFormatException("Misplaced variable in formula");
                    }
                    validFormula += token;
                }
                else
                    throw new FormulaFormatException("Invalid Token in Formula");


                lastToken = token;
            }
            if (Parenthesiscount > 0)
                throw new FormulaFormatException("Unbalanced Parenthesis");





            
        }


        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<String> Operator = new Stack<string>();
            Stack<double> value = new Stack<double>();
            List<String> Tokens = new List<String>(GetTokens(validFormula));
            foreach (string token in Tokens)
            {
                if (token == "+" || token == "-") //add or sub if this token is + or -
                {
                    if (Operator.IsOnTop("+") || Operator.IsOnTop("-"))
                    {
                        addorsub();
                        Operator.Push(token);
                    }
                    else
                    {
                        Operator.Push(token);
                    }
                }
                else if (token == "*" || token == "/")
                {
                    Operator.Push(token);
                } //Multiply or Divide if the token is * or /
                else if (token == "(") // add top (
                {
                    Operator.Push(token);
                }
                else if (token == ")") // add ) and compute math inside the ()
                {
                    if (Operator.IsOnTop("+") || Operator.IsOnTop("-"))
                    {
                        addorsub();
                    }
                    Operator.Pop();

                    if (Operator.IsOnTop("*") || Operator.IsOnTop("/"))
                    {
                        if (multordev(value.Pop()) != null)
                        {
                            return new FormulaError("Divided by 0");
                        }

                    }

                }
                else if (Regex.IsMatch(token, @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$")) //detects any numbers
                {

                    if (Operator.IsOnTop("*") || Operator.IsOnTop("/"))
                    {

                        if (multordev(double.Parse(token)) != null)
                        {
                            return new FormulaError("Divided by 0");
                        }
                    }
                    else
                    {
                        double Stringtonumber = double.Parse(token);
                        value.Push(Stringtonumber);
                    }
                }
                else if (token == " " || token == "")
                {
                    //if for whatever reason a space token is left. Do nothing.
                }
                else if (Regex.IsMatch(token, @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$"))//detects valid variables
                {
                    if (Operator.IsOnTop("*") || Operator.IsOnTop("/"))
                    {

                        String Popedoperator = Operator.Pop();
                        if (Popedoperator == "/")
                        {
                            double top = value.Pop();
                            double bottom = lookup(token);
                            if (bottom == 0)
                            {
                                return new FormulaError("division by 0");
                            }


                            double division = top / bottom;
                            value.Push(division);
                        }

                        else if (Popedoperator == "*")
                        {
                            double value1 = value.Pop();

                            double multiply = lookup(token) * value1;
                            value.Push(multiply);

                        }
                    }

                    else
                    {
                        try
                        {
                            value.Push(lookup(token));
                        }
                        catch
                        {
                            return new FormulaError("invalid token");
                        }

                        
                    }
                }

            }
            if (Operator.Count() == 1)
                addorsub();
            return value.Pop();

            ///This is a helper method that when called will add the top two values and operator IF its a + or - on top of the OPerators.
            void addorsub()
            {
                if (Operator.IsOnTop("+") || Operator.IsOnTop("-"))
                {
                    String operator1 = Operator.Pop();

                    if (operator1 == "-")
                    {
                        double value1 = value.Pop();
                        double value2 = value.Pop();
                        double number = value2 - value1;
                        value.Push(number);
                    }
                    else if (operator1 == "+")
                    {
                        double value1 = value.Pop();
                        double value2 = value.Pop();
                        double number = value1 + value2;
                        value.Push(number);
                    }
                }
            }
            ///this is a helper Method that will multply or devide the top two numbers IF there is a * or / on top of the OPerators.
            object multordev(double number)
            {

                if (Operator.IsOnTop("*") || Operator.IsOnTop("/"))
                {

                    String Popedoperator = Operator.Pop();
                    if (Popedoperator == "/")
                    {

                        double top = value.Pop();
                        double bottom = number;

                        if (bottom == 0)
                        {
                            return new FormulaError("division by 0");
                        }


                        double division = top / bottom;
                        value.Push(division);
                    }

                    else if (Popedoperator == "*")
                    {
                        double value2 = value.Pop();
                        double value1 = number;
                        double multiply = value2 * value1;
                        value.Push(multiply);

                    }


                }
                return null;
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<String> variables = new HashSet<string>();
            List<String> validVariables = new List<string>(GetTokens(validFormula));
            foreach (String token in validVariables)
            {
                if (Regex.IsMatch(token, @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$"))
                {
                    variables.Add(token);
                }
            }

            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            String StringtoReturn = "";
            foreach (String token in GetTokens(validFormula))
            {
                if (token == "" || token == " ")
                {

                }
                else if (Regex.IsMatch(token, @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"))
                {
                   StringtoReturn += double.Parse(token).ToString();
                }
                else
                {
                    StringtoReturn += token;
                }

            }
            return StringtoReturn;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if(obj.GetType() != this.GetType())
            {
                return false;
            }

            if (obj.ToString().GetHashCode() == this.GetHashCode())
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 is null)
            {
                return f2 is null;
            }


            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1 == null && f2 == null)
                return false;
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }

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

