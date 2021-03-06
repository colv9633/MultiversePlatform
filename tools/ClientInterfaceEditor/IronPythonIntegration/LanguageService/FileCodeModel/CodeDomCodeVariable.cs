/********************************************************************

The Multiverse Platform is made available under the MIT License.

Copyright (c) 2012 The Multiverse Foundation

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, 
merge, publish, distribute, sublicense, and/or sell copies 
of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE.

*********************************************************************/

/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;

namespace Microsoft.Samples.VisualStudio.CodeDomCodeModel {

    [ComVisible(true)]
    [SuppressMessage("Microsoft.Interoperability", "CA1409:ComVisibleTypesShouldBeCreatable")]
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    public class CodeDomCodeVariable : CodeDomCodeElement<CodeMemberField>, CodeVariable {
        private CodeElement parent;

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "0#dte")]
        public CodeDomCodeVariable(DTE dte, CodeElement parent, string name, CodeTypeRef type, vsCMAccess access)
            : base(dte, name) {
            CodeObject = new CodeMemberField(CodeDomCodeTypeRef.ToCodeTypeReference(type), name);
            CodeObject.Attributes = VSAccessToMemberAccess(access);
            CodeObject.UserData[CodeKey] = this;
            this.parent = parent;
        }

        public CodeDomCodeVariable(CodeElement parent, CodeMemberField field)
            : base((null==parent) ? null : parent.DTE, (null==field) ? null : field.Name) {
            this.parent = parent;
            CodeObject = field;
        }

        #region CodeVariable Members

        public override CodeElements Children {
            get { throw new NotImplementedException(); }
        }

        public override CodeElements Collection {
            get { return parent.Children; }
        }

        public override ProjectItem ProjectItem {
            get { return parent.ProjectItem; }
        }

        public vsCMAccess Access {
            get { return MemberAccessToVSAccess(CodeObject.Attributes); }
            [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
            set { 
                CodeObject.Attributes = VSAccessToMemberAccess(value);

                CommitChanges();
            }
        }

        public CodeAttribute AddAttribute(string Name, string Value, object Position) {
            CodeAttribute res =  AddCustomAttribute(CodeObject.CustomAttributes, Name, Value, Position);
            
            CommitChanges();
            
            return res;
        }

        public CodeElements Attributes {
            get { return GetCustomAttributes(CodeObject.CustomAttributes); }
        }

        public string Comment {
            get {
                return GetComment(CodeObject.Comments, false);
            }
            [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
            set {
                ReplaceComment(CodeObject.Comments, value, false);

                CommitChanges();
            }
        }

        public string DocComment {
            get {
                return GetComment(CodeObject.Comments, true);
            }
            [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
            set {
                ReplaceComment(CodeObject.Comments, value, true);

                CommitChanges();
            }
        }

        public object InitExpression {
            get {
                if (CodeObject.InitExpression != null) {
                    return ((CodeSnippetExpression)CodeObject.InitExpression).Value;
                }
                return null;
            }
            [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
            set {
                string strVal = value as string;
                if (strVal != null) {
                    CodeObject.InitExpression = new CodeSnippetExpression(strVal);

                    CommitChanges();

                    return;
                }
                throw new ArgumentException(Microsoft.Samples.VisualStudio.IronPythonLanguageService.Resources.CodeModelVariableExpressionNotString);
            }
        }

        public bool IsConstant {
            get {
                return (CodeObject.Attributes & MemberAttributes.Const) != 0;
            }
            [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
            set {
                if (value) CodeObject.Attributes |= MemberAttributes.Const;
                else CodeObject.Attributes &= ~MemberAttributes.Const;

                CommitChanges();
            }
        }

        public bool IsShared {
            get {
                return (CodeObject.Attributes & MemberAttributes.Static) != 0;
            }
            [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
            set {
                if (value) CodeObject.Attributes |= MemberAttributes.Static;
                else CodeObject.Attributes &= ~MemberAttributes.Static;

                CommitChanges();
            }
        }

        public object Parent {
            get { return parent; }
        }

        public CodeTypeRef Type {
            get {
                return CodeDomCodeTypeRef.FromCodeTypeReference(CodeObject.Type);
            }
            [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
            set {
                CodeObject.Type = CodeDomCodeTypeRef.ToCodeTypeReference(value);

                CommitChanges();
            }
        }

        public string get_Prototype(int Flags) {
            throw new NotImplementedException();
        }

        #endregion

        public override object ParentElement {
            get { return parent; }
        }

        public override string FullName {
            get { return CodeObject.Name; }
        }

        public override vsCMElement Kind {
            get {
                return vsCMElement.vsCMElementVariable;
            }
        }

    }

}
