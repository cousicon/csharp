//Written By Connor Cousineau && Joe Zachary
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using SpreadsheetUtilities;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private readonly DependencyGraph localDependency; 

        private readonly Dictionary<string, Cell> spreadsheet;


        public Spreadsheet() : this(s => true, s => s, "default")
        {
            localDependency = new DependencyGraph();
            spreadsheet = new Dictionary<string, Cell>();
    }

        public override bool Changed
        {
            
            get; protected set;
        }


        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {

            localDependency = new DependencyGraph();
            spreadsheet = new Dictionary<string, Cell>();
        }
        public Spreadsheet(String filepath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {

            localDependency = new DependencyGraph();
            spreadsheet = new Dictionary<string, Cell>();
        }

        public override object GetCellContents(string name)
        {
            if (name is null || !Regex.IsMatch(name, @"^[a-zA-Z](?:[a-zA-Z]|\d)*$") || !IsValid(name))//checks all variables
            {
                throw new InvalidNameException(); //throws if invalid
            }
            name = Normalize(name);//Normalized to the provided normalizer
            if (spreadsheet.ContainsKey(name))// if in SS
                return spreadsheet[name].GetContents();//Get the contents

            return "";
        }

        public override object GetCellValue(string name)
        {
            if (name is null || !Regex.IsMatch(name, @"^[a-zA-Z](?:[a-zA-Z]|\d)*$") || !IsValid(name))
                throw new InvalidNameException();
            name = Normalize(name);
            return spreadsheet[name].GetValue();//get the Value
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return spreadsheet.Keys;// returns all the Cells with Names
        }

        public override string GetSavedVersion(string filename)
        {
           string thisVersion = "";//creates a Version
            
                // Create an XmlReader inside this block, and automatically Dispose() it at the end.
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    object CellContents = null;
                    object _Name = null;

                    while (reader.Read())//starts reader
                    {
                        if (reader.IsStartElement())//reads the first element
                        {
                            switch (reader.Name)//Checks the names
                            {
                            case "Spreadsheet":
                                
                                
                                if(reader["Version"] != (this.Version))
                                    throw new SpreadsheetReadWriteException("Invalid Version");
                                thisVersion = reader["Version"];
                                break; 
                            case "Cell":

                                break;


                            case "Name":

                                reader.Read();
                                _Name = reader.Value;

                                break;

                            case "Contents":

                                reader.Read();
                                CellContents = reader.Value;
                                break;
                            }
                        }
      
                          
                    }
                }

            return thisVersion;
        }

        public override void Save(string filename)
        {
            // We want some non-default settings for our XML writer.
            // Specifically, use indentation to make it more readable.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            // Create an XmlWriter inside this block, and automatically Dispose() it at the end.
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {

                writer.WriteStartDocument();//Writes an XML
                writer.WriteStartElement("Spreadsheet");
                writer.WriteAttributeString("Version", Version);
                writer.WriteStartElement("Cell");
                foreach (KeyValuePair<string, Cell> s in spreadsheet)
                {
                    s.Value.WriteXml(writer);
                }
                writer.WriteEndElement(); // Ends the Cell block
                writer.WriteEndElement(); // Ends the Spreadsheet block
                writer.WriteEndDocument();

            }
            if(Changed)
            {
                //Tests the Getter
            }
            Changed = false;
        }

        protected override IList<string> SetCellContents(string name, double number)
        {
            
            if (!spreadsheet.ContainsKey(name))// if SS doesnt contain the key
            {
                spreadsheet.Add(name, new Cell(number, name)); // add it
            }
            else
            {
                if (spreadsheet[name].GetContents() is Formula)//otherwise its in the SS
                {
                    Formula oldFormula = (Formula)spreadsheet[name].GetContents();
                    IEnumerable<String> newDependees = oldFormula.GetVariables();//Change it
                    foreach (string s in newDependees)
                    {
                        localDependency.RemoveDependency(name, s);
                    }
                }
                spreadsheet[name].SetContents(number);
            }

            return GetCellsToRecalculate(name).ToList();//returns a list

        }

        protected override IList<string> SetCellContents(string name, string text)//sets the Contents to the provided Text
        {
            if (text == "")
                return GetCellsToRecalculate(name).ToList();

            if (!spreadsheet.ContainsKey(name))
                spreadsheet.Add(name, new Cell(text, name));
            else
            {
                if(spreadsheet[name].GetContents() is Formula)
                {
                    Formula oldFormula = (Formula)spreadsheet[name].GetContents();
                    IEnumerable<String> newDependees = oldFormula.GetVariables();
                    foreach (string s in newDependees)
                    {
                        localDependency.RemoveDependency(name, s);
                    }
                }
                    
                spreadsheet[name].SetContents(text);
            }


            return GetCellsToRecalculate(name).ToList();

        }

        protected override IList<string> SetCellContents(string name, Formula formula)//Sets the Cell to the Fomrula
        {
            object Storage;
            if (!spreadsheet.ContainsKey(name))
            {

                spreadsheet.Add(name, new Cell(formula, name));
                 Storage = spreadsheet[name].GetContents();
                IEnumerable<String> newDependees = formula.GetVariables();
                foreach(string s in newDependees)
                {
                    localDependency.AddDependency(name, s);
                }
            }
            else
            {
                Storage = spreadsheet[name].GetContents();
                spreadsheet[name].SetContents(formula);
                localDependency.ReplaceDependents(name, formula.GetVariables());
                
            }

            spreadsheet[name].SetValue(formula.Evaluate(Lookup));

            try { return GetCellsToRecalculate(name).ToList(); }//Try to return the list, otherwise undo the error.
            catch
            {
                if (Storage is Formula)
                { 
                Formula StoredFormula = (Formula)Storage;
                spreadsheet[name].SetContents(StoredFormula);
                    localDependency.ReplaceDependents(name, StoredFormula.GetVariables());
                    
                }
                if(Storage is double)
                {
                    spreadsheet[name].SetContents(Storage);
                    
                }
                if(Storage is string)
                {
                    spreadsheet[name].SetContents(Storage);
                    
                }
                throw new CircularException();

            }
        }

        public override IList<string> SetContentsOfCell(string name, string content)//Set the cell to any content
        {
            IList<String> list = null;

            if (content is null)
                throw new ArgumentNullException();
            if (name is null || !Regex.IsMatch(name, @"^[a-zA-Z](?:[a-zA-Z]|\d)*$") || !IsValid(name))
                throw new InvalidNameException();
                name = Normalize(name);
            if(double.TryParse(content, out double result))
            {
               list = SetCellContents(name, result);
            }
            else if (content.StartsWith("="))
            {
                String providedFormula = content.Substring(1);
                Formula potentialFormula = new Formula(providedFormula, Normalize, IsValid);
                list = SetCellContents(name, potentialFormula);
                object number = potentialFormula.Evaluate(Lookup);
                spreadsheet[name].SetValue(number);
            }
            else list = SetCellContents(name, content);

            for(int i = 1; i <list.Count; i++)
            {
                spreadsheet[list[i]].SetValue(((Formula)spreadsheet[list[i]].GetContents()).Evaluate(Lookup));
            }
            Changed = true;
            return list;
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if(localDependency.HasDependees(name))
                return  new HashSet<String>(localDependency.GetDependees(name));
            return new HashSet<String>();
        }


        internal class Cell
        {
            private object contents;
            private object value;
            private readonly string name;

            public Cell(object _contents, string _name)
            {
                contents = _contents;
                name = _name;
            }

            public void SetContents(object replacement)
            {
                if (replacement is String)
                { 
                contents = replacement;
                    value = replacement;
                }
                if (replacement is double)
                {
                    contents = replacement;
                    value = replacement;
                }
                if (replacement is Formula)
                {
                    contents = replacement;
                }

                
            }
            public object GetContents()
            {
                return contents;
            }
            public void SetValue(object replacement)
            {
                value = replacement;
            }

            public object GetValue()
              {
                  return value;
              }

            /// <summary>
            /// Write this State to an existing XmlWriter
            /// </summary>
            /// <param name="writer"></param>
            public void WriteXml(XmlWriter writer)
            {
                writer.WriteStartElement("Cell");
                // We use a shortcut to write an element with a single string
                writer.WriteElementString("Name",name);
                writer.WriteElementString("Contents", contents.ToString());
                writer.WriteEndElement();
            }

        }

        private double Lookup(string s)
        {
            if (spreadsheet.Keys.Contains(s))
            {
                if (spreadsheet[s].GetValue() is double)
                {
                    return (double)spreadsheet[s].GetValue();
                }
                else
                    throw new ArgumentException("invalid variable");

            }
            else
                throw new ArgumentException("No value associated with key");
        }

    }





}
    



