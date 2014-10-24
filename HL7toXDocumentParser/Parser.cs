using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HL7toXDocumentParser
{
    public class Parser
    {
        readonly int[] defaultSegmentDelims = new int[] {'\r','\n'};
        readonly int defaultFieldDelim = '|';
        readonly int defaultComponentDelim = '^';
        readonly int defaultRepetitionSep = '~';
        readonly int defaultEscapeChar = '\\';
        readonly int defaultSubcomponentDelim = '&';
        readonly string[] headerSegments = new string[] { "MSH", "FHS", "FTS", "BHS", "BTS" };
        readonly string[] headerSegmentsWithDelimiters = new string[] { "MSH", "FHS", "BHS" };

        public XDocument Parse(string hl7)
        {
            if (hl7 == null)
                throw new ArgumentNullException("hl7");

            using (var reader = new StringReader(hl7))
            {
                return Parse(reader);
            }
        }


        public XDocument Parse(TextReader reader)
        {
            var startSegment = string.Concat((char)reader.Read(), (char)reader.Read(), (char)reader.Read());

            //is this a HL7 message?
            if (startSegment != "MSH" && startSegment != "FHS" && startSegment != "BHS")
                throw new XmlException("Not a valid HL7. (HL7 must start with MSH, FHS or BHS)");

            //get the delimiters
            var fieldDelim = reader.Read();
            var componentDelim = reader.Read();
            var repetitionSep = reader.Read();
            var escapeChar = reader.Read();
            var subcomponentDelim = reader.Read();

            var doc = new XDocument(
                new XElement("HL7")
            );

            string segmentName = null;
            XElement segment = null;
            XElement field = null;
            XElement component = null;
            XElement subComponent = null;
            int fieldIndex = 1;
            int componentIndex = 1;
            int subComponentIndex = 1;
            bool isHeader = true;
            bool isHeaderWithDelimiters = true;
            string token = null;

            // Get the first data element from the file.
            var val = new TokenDelim(startSegment, fieldDelim);
            segmentName = val.Token;
            segment = new XElement(segmentName);
            doc.Root.Add(segment);

            field = new XElement(segmentName + "." + fieldIndex, (char)fieldDelim);
            segment.Add(field);
            fieldIndex++;
            field = new XElement(segmentName + "." + fieldIndex, string.Concat((char)componentDelim, (char)repetitionSep, (char)escapeChar, (char)subcomponentDelim));
            segment.Add(field);
            fieldIndex++;

            reader.Read();
            val = NextToken(reader, defaultSegmentDelims, fieldDelim, componentDelim, repetitionSep, escapeChar, subcomponentDelim);

            //create a new element with each data token from //the stream.
            while (val.Delimiter >= 0 && val.Delimiter < short.MaxValue)
            {
                if (defaultSegmentDelims.Contains(val.Delimiter))
                {
                    if (subComponent != null)
                    {
                        subComponent = new XElement(segmentName + "." + fieldIndex + "." + componentIndex + "." + subComponentIndex, token + val.Token);
                        component.Add(subComponent);
                    }
                    else if (component != null)
                    {
                        component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex, token + val.Token);
                        field.Add(component);
                    }
                    else if (segment != null)
                    {
                        field = new XElement(segmentName + "." + fieldIndex, token + val.Token);
                        segment.Add(field);
                    }

                    segment = null;
                    field = null;
                    fieldIndex = 1;
                    component = null;
                    componentIndex = 1;
                    subComponent = null;
                    subComponentIndex = 1;

                    token = "";
                }
                else if (val.Delimiter == fieldDelim)
                {
                    if (segment == null)
                    {
                        segmentName = token + val.Token;
                        segment = new XElement(segmentName);
                        doc.Root.Add(segment);
                        isHeader = headerSegments.Contains(segmentName);
                        isHeaderWithDelimiters = headerSegmentsWithDelimiters.Contains(segmentName);

                        if (isHeaderWithDelimiters)
                        {
                            /*fieldDelim*/
                            reader.Read();
                            /*componentDelim*/
                            reader.Read();
                            /*repetitionSep*/
                            reader.Read();
                            /*escapeChar*/
                            reader.Read();
                            /*subcomponentDelim*/
                            reader.Read();

                            field = new XElement(segmentName + "." + fieldIndex, (char)fieldDelim);
                            segment.Add(field);
                            fieldIndex++;
                            field = new XElement(segmentName + "." + fieldIndex, string.Concat((char)componentDelim, (char)repetitionSep, (char)escapeChar, (char)subcomponentDelim));
                            segment.Add(field);
                        }
                        else
                            fieldIndex--;
                    }
                    else
                    {
                        if (fieldIndex > 0)
                        {
                            if (isHeaderWithDelimiters && fieldIndex == 1)
                            {
                                field = new XElement(segmentName + "." + fieldIndex, string.Concat((char)componentDelim, (char)repetitionSep, (char)escapeChar, (char)subcomponentDelim));
                                segment.Add(field);
                            }
                            else if (componentIndex == 1)
                            {
                                field = new XElement(segmentName + "." + fieldIndex, token + val.Token);
                                segment.Add(field);
                            }
                            else
                            {
                                component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex, token + val.Token);
                                field.Add(component);
                            }
                        }
                        component = null;
                        componentIndex = 1;
                        subComponent = null;
                        subComponentIndex = 1;
                    }
                    fieldIndex++;
                    token = "";
                }
                else if (val.Delimiter == componentDelim)
                {
                    if (!isHeader || fieldIndex != 1)
                    {
                        if (componentIndex == 1)
                        {
                            field = new XElement(segmentName + "." + fieldIndex);
                            segment.Add(field);
                        }

                        if (subComponentIndex == 1)
                        {
                            component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex, token + val.Token);
                            field.Add(component);
                        }
                        else
                        {
                            subComponent = new XElement(segmentName + "." + fieldIndex + "." + componentIndex + "." + subComponentIndex, token + val.Token);
                            component.Add(subComponent);
                        }

                        componentIndex++;
                        subComponent = null;
                        subComponentIndex = 1;
                        token = "";
                    }
                }
                else if (val.Delimiter == repetitionSep)
                {
                    if (!isHeader || fieldIndex != 1)
                    {

                    }
                }
                else if (val.Delimiter == escapeChar)
                {
                    //\Cxxyy\ 	Single-byte character set escape sequence with two hexadecimal values not converted
                    //\E\ 	    Escape character converted to escape character (e.g., ‘\’)
                    //\F\ 	    Field separator converted to field separator character (e.g., ‘|’)
                    //\H\ 	    Start highlighting not converted
                    //\Mxxyyzz\ Multi-byte character set escape sequence with two or three hexadecimal values (zz is optional) not converted
                    //\N\ 	    Normal text (end highlighting) not converted
                    //\R\ 	    Repetition separator converted to repetition separator character (e.g., ‘~’)
                    //\S\ 	    Component separator converted to component separator character (e.g., ‘^’)
                    //\T\ 	    Subcomponent separator converted to subcomponent separator character (e.g., ‘&’)
                    //\Xdd…\ 	Hexadecimal data (dd must be hexadecimal characters) converted to the characters identified by each pair of digits
                    //\Zdd…\ 	Locally defined escape sequence not converted
                    if (!isHeader || fieldIndex != 1)
                    {
                        if (!string.IsNullOrEmpty(val.Token))
                        {
                            if (val.Token.StartsWith("C"))
                            {

                            }
                            else if (val.Token == "E")
                                token += (char)escapeChar;
                            else if (val.Token == "F")
                                token += fieldDelim;
                            else if (val.Token == "H")
                            {
                            }
                            else if (val.Token.StartsWith("M"))
                            {

                            }
                            else if (val.Token == "N")
                            {

                            }
                            else if (val.Token == "R")
                                token += (char)repetitionSep;
                            else if (val.Token == "S")
                                token += (char)componentDelim;
                            else if (val.Token == "T")
                                token += (char)subcomponentDelim;
                            else if (val.Token.StartsWith("X"))
                            {

                            }
                            else if (val.Token.StartsWith("Z"))
                            {

                            }
                            else
                                token += val.Token;
                        }
                    }
                }
                else if (val.Delimiter == subcomponentDelim)
                {
                    if (!isHeader || fieldIndex != 1)
                    {
                        if (subComponentIndex == 1)
                        {
                            component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex);
                            field.Add(component);
                        }

                        subComponent = new XElement(segmentName + "." + fieldIndex + "." + componentIndex + "." + subComponentIndex, token + val.Token);
                        component.Add(subComponent);
                        subComponentIndex++;
                        token = "";
                    }
                }

                val = NextToken(reader, defaultSegmentDelims, fieldDelim, componentDelim, repetitionSep, escapeChar, subcomponentDelim);
            }

            //add the last token
            if (subComponentIndex > 1 && component != null)
            {
                subComponent = new XElement(segmentName + "." + fieldIndex + "." + componentIndex + "." + subComponentIndex, token + val.Token);
                component.Add(subComponent);
            }
            else if (componentIndex > 1 && field != null)
            {
                component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex, token + val.Token);
                field.Add(component);
            }
            else if (segment != null)
            {
                field = new XElement(segmentName + "." + fieldIndex, token + val.Token);
                segment.Add(field);
            }

            return doc;
        }

        TokenDelim NextToken(TextReader reader, int[] segmentDelims, int fieldDelim, int componentDelim, int repetitionSep, int escapeChar, int subcomponentDelim)
        {
            var token = "";
            var temp = reader.Read();
            while (temp != -1 && !segmentDelims.Contains(temp) && temp != fieldDelim && temp != componentDelim && temp != repetitionSep && temp != escapeChar && temp != subcomponentDelim)
            {
                token += (char)temp;
                temp = reader.Read();
            }
            return new TokenDelim(token, temp);
        }

        class TokenDelim
        {
            public TokenDelim(string token, int delimiter)
            {
                Token = token;
                Delimiter = delimiter;
            }

            public string Token { get; private set; }
            public int Delimiter { get; private set; }
        }
    }
}