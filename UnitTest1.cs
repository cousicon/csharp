using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace FormulaTester
{
    [TestClass]
    public class UnitTest1
    {

        private double lookup(string input)
        {
            if (input == "__3")
            {
                return 3;
            }
            if (input == "A3")
            {
                return 3e1;
            }
            if (input == "Z6")
            {
                return 6;
            }
            if (input == "z0")
            {
                return 0;
            }
            throw new ArgumentException("Invalid Variable");
        }

        private bool validate(string input)
        {

            if (input == "A3")
            {
                return true;
            }
            if (input == "Z6")
            {
                return true;
            }
            if (input == "z0")
            {
                return true;
            }
            return false;
        }


        [TestMethod]
        public void ConstrucotrBasicTest()
        {
            Formula basicConstruction = new Formula("1 + 2");
            Formula basicConstruction2 = new Formula("1 + 2");

            Assert.AreEqual(true, basicConstruction.Equals(basicConstruction2));

            Formula simpleFormula = new Formula("1+3+2");
            Formula simpleFormula2 = new Formula("1+2+3");

            Assert.AreEqual(false, simpleFormula.Equals(simpleFormula2));
        }
        [TestMethod]
        public void BasicTest2()
        {
            Formula simpleFormula = new Formula("1+2+3+4+5+6+7+8+9+10+11");
            Formula simpleFormula2 = new Formula("1+2+3+4+5+6+7+8+9+10");
            Assert.AreEqual(false, simpleFormula.Equals(simpleFormula2));
        }
        [TestMethod]
        public void ConstrucotrBasicTest2()
        {
            Formula basicConstruction = new Formula("1 + 2");
            Assert.AreEqual(3d, basicConstruction.Evaluate(s => 0));
        }
        [TestMethod]
        public void variabletest()
        {
            Formula Variables = new Formula("1 + A3 + __3");
            Assert.AreEqual(34d, Variables.Evaluate(lookup));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionMisplacedParenthesis()
        {
            Formula ParenthesisTest = new Formula("1 * (A3 + __3) - Z6 + ()");

        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionMisplacedNumber()
        {
            Formula MissingOperator = new Formula("1 * (A3 + __3) - Z6 13");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionEmpty()
        {
            Formula Empty = new Formula("");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionMisplacedOperator()
        {
            Formula MissplacedOperator = new Formula("1 * (A3 + __3) - Z6 +");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionInvalidFirstToken()
        {
            Formula InvalidFirstToken = new Formula("+1 * (A3 + __3) - Z6 +13");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionOpeningParen()
        {
            Formula InvalidOPerator = new Formula("1 * (A3 + __3)( - Z6 13");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionMismatchedParen()
        {
            Formula MissplacedParen = new Formula("1 (* (A3 + __3) - (Z6 + 13)");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionMisplacedpParen2()
        {
            Formula MissplacedParen = new Formula("1 * (A3 + __3) - Z6-(13+3))");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionMisplacedpParen3()
        {
            Formula MissplacedParen = new Formula("((1+13) * (A3 + __3) - Z6-(13+3)");
        }
        [TestMethod]
        public void AdvancedFormualEvaluation()
        {
            Formula MixedVariablesandNumberEQ = new Formula("1 * (A3 + __3) - Z6 + (A3/__3)");
            Assert.AreEqual(37d, MixedVariablesandNumberEQ.Evaluate(lookup));
        }
        [TestMethod]
        public void AdvancedFormualEvaluation2()
        {
            Formula DevideByZero = new Formula("A3/0");
            Assert.IsInstanceOfType(DevideByZero.Evaluate(lookup), typeof(FormulaError));
        }
        [TestMethod]
        public void AdvancedFormualEvaluation3()
        {
            Formula basicConstruction = new Formula("1 * (A3 + __3) - Z6 +(A3/z0)");
            Assert.IsInstanceOfType(basicConstruction.Evaluate(lookup), typeof(FormulaError));
        }
        [TestMethod]
        public void AdvancedFormualEvaluation4()
        {
            Formula MediumEq = new Formula("1 * (A3 + __3) - Z6 + (A3/10) - 13*A3 ");
            Assert.AreEqual(-360d, MediumEq.Evaluate(lookup));
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionInvalidToken()
        {
            Formula InvalidToken = new Formula("12 - $ + 13 * (A3 + __3) - Z6-(13+3)");
        }
        [TestMethod]
        public void VariablesTest()
        {
            HashSet<String> Variables = new HashSet<String>() { "A3", "__3", "Z6" };
            Formula GeneratedVariables = new Formula("(13 + 13) * (A3 + __3) - Z6-(13+3)");

            Assert.IsTrue(Variables.SetEquals(GeneratedVariables.GetVariables()));
        }
        [TestMethod]
        public void EqualsEqualsTest()
        {
            Formula StringOfPLus = new Formula("1+2+3+4+5+6+7+8+9+10+11");
            Formula OtherStringOfPLus = new Formula("1+2+3+4+5+6+7+8+9+10");
            Assert.AreEqual(false, StringOfPLus == OtherStringOfPLus);
        }
        [TestMethod]
        public void DoesnotEqualsTest()
        {
            Formula StringOfPlus = new Formula("1+2+3+4+5+6+7+8+9+10+11");
            Formula OtherStringOfPLus = new Formula("1+2+3+4+5+6+7+8+9+10");
            Assert.AreEqual(true, StringOfPlus != OtherStringOfPLus);
        }
        [TestMethod]
        public void AdvancedFormualEvaluation5()
        {
            Formula DivideByZero = new Formula("15/(A3/0)");
            Assert.IsInstanceOfType(DivideByZero.Evaluate(lookup), typeof(FormulaError));
        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaExceptionInvalidNumberToken()
        {
            Formula IvalidToken = new Formula("(A3 + __3) 15 ");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void AdvancedVariabletest()
        {
            Formula simpleNwithvalidator = new Formula("1 + A3 +Z6-z0+ __3", s => s, validate);

        }
    }

}
