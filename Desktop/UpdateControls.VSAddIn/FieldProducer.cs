/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UpdateControls.VSAddIn
{
    public class FieldProducer
    {
        public static string BeginTagGeneratedRegion = "Generated by Update Controls --------------------------------";
        public static string EndTagGeneratedRegion = "End generated code --------------------------------";

        private static string CSIndependentDeclaration = "private Independent _ind{0} = new Independent();\r\n";
        private static string CSIndependentDeclarationExpression = ".*private Independent _ind[0-9a-zA-Z_]+ = new Independent();.*";
        private static string CSCollectionPropertyDeclaration =
            "\r\npublic {0} New{3}()\r\n{{\r\n_ind{1}.OnSet();\r\n{0} {4} = new {0}();\r\n{2}.Add({4});\r\nreturn {4};\r\n}}\r\n" +
            "\r\npublic void Delete{3}({0} {4})\r\n{{\r\n_ind{1}.OnSet();\r\n{2}.Remove({4});\r\n}}\r\n" +
            "\r\npublic IEnumerable<{0}> {1}\r\n{{\r\nget {{ _ind{1}.OnGet(); return {2};}}\r\n}}\r\n";
        private static string CSAtomicPropertyDeclaration =
            "\r\npublic {0} {1}\r\n{{\r\nget {{ _ind{1}.OnGet(); return {2}; }}\r\n" +
            "set {{ _ind{1}.OnSet(); {2} = value; }}\r\n}}\r\n";
        private static string CSBeginRegion =
            "\r\n#region Independent properties\r\n// Generated by Update Controls --------------------------------\r\n";
        private static string CSEndRegion =
            "// End generated code --------------------------------\r\n#endregion\r\n";

        private static string VBIndependentDeclaration = "Private _ind{0} As New UpdateControls.Independent\r\n";
        private static string VBIndependentDeclarationExpression = ".*Private _ind[0-9a-z_]+ As New UpdateControls[.]Independent.*";
        private static string VBCollectionPropertyDeclaration =
            "\r\nPublic Function New{3}() As {0}\r\n_ind{1}.OnSet()\r\nDim {4} As New {0}\r\n{2}.Add({4})\r\nReturn {4}\r\nEnd Function\r\n" +
            "\r\nPublic Sub Delete{3}({4} As {0})\r\n_ind{1}.OnSet()\r\n{2}.Remove({4})\r\nEnd Sub\r\n" +
            "\r\nPublic ReadOnly Property {1} As IEnumerable(Of {0})\r\nGet\r\n_ind{1}.OnGet()\r\nReturn {2}\r\nEnd Get\r\nEnd Property\r\n";
        private static string VBAtomicPropertyDeclaration =
            "\r\nPublic Property {1} As {0}\r\nGet\r\n_ind{1}.OnGet()\r\nReturn {2}\r\nEnd Get\r\n" +
            "Set(ByVal value As {0})\r\n_ind{1}.OnSet()\r\n{2} = value\r\nEnd Set\r\nEnd Property\r\n";
        private static string VBBeginRegion =
            "\r\n#Region \"Independent properties\"\r\n' Generated by Update Controls --------------------------------\r\n";
        private static string VBEndRegion =
            "' End generated code --------------------------------\r\n#End Region\r\n";

        private string _independentDeclaration;
        private string _collectionPropertyDeclaration;
        private string _atomicPropertyDeclaration;
        private string _beginRegion;
        private string _endRegion;
        private bool _requiresUsing;

        private Regex _independentDeclarationExpression;

        public FieldProducer(Language language)
        {
            if (language == Language.CS)
            {
                _independentDeclaration = CSIndependentDeclaration;
                _collectionPropertyDeclaration = CSCollectionPropertyDeclaration;
                _atomicPropertyDeclaration = CSAtomicPropertyDeclaration;
                _beginRegion = CSBeginRegion;
                _endRegion = CSEndRegion;
                _requiresUsing = true;

                _independentDeclarationExpression = new Regex(CSIndependentDeclarationExpression);
            }
            else if (language == Language.VB)
            {
                _independentDeclaration = VBIndependentDeclaration;
                _collectionPropertyDeclaration = VBCollectionPropertyDeclaration;
                _atomicPropertyDeclaration = VBAtomicPropertyDeclaration;
                _beginRegion = VBBeginRegion;
                _endRegion = VBEndRegion;
                _requiresUsing = false;

                _independentDeclarationExpression = new Regex(VBIndependentDeclarationExpression, RegexOptions.IgnoreCase);
            }
        }

        public string BeginRegion
        {
            get { return _beginRegion; }
        }

        public string EndRegion
        {
            get { return _endRegion; }
        }

        public void AppendCollection(StringBuilder dynamics, StringBuilder properties, string type, string name, string propertyName, string singular, string valueName)
        {
            dynamics.AppendFormat(_independentDeclaration, propertyName);
            properties.AppendFormat(
                _collectionPropertyDeclaration,
                type, propertyName, name, singular, valueName);
        }

        public void AppendAtom(StringBuilder dynamics, StringBuilder properties, string propertyName, string type, string name)
        {
            dynamics.AppendFormat(_independentDeclaration, propertyName);
            properties.AppendFormat(
                _atomicPropertyDeclaration,
                type, propertyName, name);
        }

        public bool RequiresUsing
        {
            get { return _requiresUsing; }
        }

        public bool IsIndependentSentry(string line)
        {
            return _independentDeclarationExpression.IsMatch(line);
        }
    }
}
