using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PluginCore.Localization;
using ASCompletion.Context;

namespace ASCompletion.Model
{
    /// <summary>
    /// Object representation of an Actionscript class
    /// </summary>
    [Serializable()]
    public class ClassModel: MemberModel
    {
        static public ClassModel VoidClass;
        static private List<ClassModel> extensionList;

        static ClassModel()
        {
            VoidClass = new ClassModel();
            VoidClass.Name = "void";
            VoidClass.InFile = new FileModel("");
        }

        static private void EndResolveExtend()
        {
            extensionList = null;
        }

        static private void BeginResolveExtend(ClassModel firstClass)
        {
            extensionList = new List<ClassModel>();
            if (firstClass != null) extensionList.Add(firstClass);
        }

        public string Constructor;
        public MemberList Members;

        private string extendsType;
        private string indexType;
        public List<string> Implements;
        private WeakReference resolvedExtend;

        public string QualifiedName
        {
            get { return (InFile.Package == "") ? Name : InFile.Package + "." + Name; }
        }
        public string ExtendsType
        {
            get { return extendsType; }
            set { extendsType = value; }
        }
        public string IndexType
        {
            get { return indexType; }
            set { indexType = value; }
        }

        /// <summary>
        /// Resolved extended type. Update using ResolveExtends()
        /// </summary>
        public ClassModel Extends
        {
            get 
            {
                if (resolvedExtend == null || !resolvedExtend.IsAlive)
                {
                    resolvedExtend = null;
                    return ClassModel.VoidClass;
                }
                else return resolvedExtend.Target as ClassModel ?? ClassModel.VoidClass;
            }
        }

        /// <summary>
        /// Resolve inheritance chain starting with this class
        /// </summary>
        public void ResolveExtends()
        {
            ClassModel aClass = this;
            BeginResolveExtend(aClass);
            try
            {
                while (!aClass.IsVoid())
                {
                    aClass = aClass.ResolveExtendedType();
                }
            }
            finally { EndResolveExtend(); }
        }

        private ClassModel ResolveExtendedType()
        {
            if (InFile.Context == null)
            {
                resolvedExtend = null;
                return VoidClass;
            }
            if (extendsType == null || extendsType.Length == 0)
            {
                if (this == VoidClass || (Flags & FlagType.Interface) > 0)
                {
                    resolvedExtend = null;
                    return VoidClass;
                }
                extendsType = InFile.Context.DefaultInheritance(InFile.Package, Name);
                if (extendsType == QualifiedName)
                {
                    extendsType = InFile.Context.Features.voidKey;
                    resolvedExtend = null;
                    return VoidClass;
                }
            }
            ClassModel extends = InFile.Context.ResolveType(extendsType, InFile);
            if (!extends.IsVoid())
            {
                // check loops in inheritance
                if (extensionList != null)
                {
                    foreach(ClassModel model in extensionList)
                    if (model.QualifiedName == extends.QualifiedName)
                    {
                        string info = String.Format(TextHelper.GetString("ASCompletion.Info.InheritanceLoop"), Type, extensionList[0].Type);
                        PluginCore.Controls.MessageBar.ShowWarning(info);
                        resolvedExtend = null;
                        return VoidClass;
                    }
                    extensionList.Add(extends);
                }
                extends.InFile.Check();
            }
            resolvedExtend = new WeakReference(extends);
            return extends;
        }

        public ClassModel()
        {
            Name = null;
            Members = new MemberList();
        }

        public bool IsVoid()
        {
            return this == VoidClass;
        }

        public bool IsEnum() {
            return (this.Flags & FlagType.Enum) != 0;
        }

        public new object Clone()
        {
            ClassModel copy = new ClassModel();
            copy.Name = Name;
            copy.Flags = Flags;
            copy.Access = Access;
            copy.Namespace = Namespace;
            if (Parameters != null)
            {
                copy.Parameters = new List<MemberModel>();
                foreach (MemberModel param in Parameters)
                    copy.Parameters.Add(param.Clone() as MemberModel);
            }
            copy.Type = Type;
            copy.Comments = Comments;
            copy.InFile = InFile;
            copy.Constructor = Constructor;
            if (Implements != null)
            {
                copy.Implements = new List<string>();
                foreach (string cname in Implements) copy.Implements.Add(cname);
            }
            copy.extendsType = extendsType;
            copy.indexType = indexType;
            copy.Members = new MemberList();
            foreach (MemberModel item in Members)
                copy.Members.Add(item.Clone() as MemberModel);

            return copy;
        }

        
        #region Completion-dedicated methods

        public MemberModel ToMemberModel()
        {
            MemberModel self = new MemberModel();
            //int p = Name.LastIndexOf(".");
            //self.Name = (p >= 0) ? Name.Substring(p + 1) : Name;
            self.Name = Name;
            self.Type = QualifiedName;
            self.Flags = Flags;
            return self;
        }

        internal MemberList GetSortedMembersList()
        {
            MemberList items = new MemberList();
            foreach (MemberModel item in Members)
                if ((item.Flags & FlagType.Constructor) == 0) items.Add(item);
            items.Sort();
            return items;
        }

        #endregion

        #region Sorting

        public void Sort()
        {
            Members.Sort();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ClassModel)) return false;
            return Name.Equals(((ClassModel)obj).Name);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #region Text output

        public override string ToString()
        {
            return ClassDeclaration(this);
        }

        public string GenerateIntrinsic(bool caching)
        {
            StringBuilder sb = new StringBuilder();
            string nl = (caching) ? "" : "\r\n";
            char semi = ';';
            string tab0 = (!caching && InFile.Version == 3) ? "\t" : "";
            string tab = (caching) ? "" : ((InFile.Version == 3) ? "\t\t" : "\t");

            // CLASS
            sb.Append(CommentDeclaration(Comments, tab0)).Append(tab0);
            if (!caching && InFile.Version != 3 && (this.Flags & (FlagType.Intrinsic | FlagType.Interface)) == 0)
            {
                sb.Append((InFile.haXe) ? "extern " : "intrinsic ");
            }
            sb.Append(ClassDeclaration(this, InFile.Version < 3));

            if (ExtendsType != null)
            {
                sb.Append(" extends ").Append(extendsType);
            }
            if (Implements != null)
            {
                sb.Append(" implements ");
                bool addSep = false;
                foreach (string iname in Implements)
                {
                    if (addSep) sb.Append(", ");
                    else addSep = true;
                    sb.Append(iname);
                }
            }
            sb.Append(nl).Append(tab0).Append('{');

            // MEMBERS
            int count = 0;
            foreach (MemberModel var in Members)
                if ((var.Flags & FlagType.Variable) > 0)
                {
                    String comment = CommentDeclaration(var.Comments, tab);
                    if (count == 0 || comment != "") sb.Append(nl);
                    sb.Append(comment);
                    sb.Append(tab).Append(MemberDeclaration(var)).Append(semi).Append(nl);
                    count++;
                }

            // MEMBERS
            string decl;
            MemberModel temp;
            string prevProperty = null;
            foreach (MemberModel property in Members)
                if ((property.Flags & (FlagType.Getter | FlagType.Setter)) > 0)
                {
                    if (prevProperty != property.Name) sb.Append(nl);
                    prevProperty = property.Name;
                    sb.Append(CommentDeclaration(property.Comments, tab));
                    FlagType flags = (property.Flags & ~(FlagType.Setter | FlagType.Getter)) | FlagType.Function;

                    if ((property.Flags & FlagType.Getter) > 0)
                    {
                        temp = (MemberModel)property.Clone();
                        temp.Name = "get " + temp.Name;
                        temp.Flags = flags;
                        temp.Parameters = null;
                        sb.Append(tab).Append(MemberDeclaration(temp)).Append(semi).Append(nl);
                    }
                    if ((property.Flags & FlagType.Setter) > 0)
                    {
                        temp = (MemberModel)property.Clone();
                        temp.Name = "set " + temp.Name;
                        temp.Flags = flags;
                        temp.Type = (InFile.Version == 3) ? "void" : "Void";
                        sb.Append(tab).Append(MemberDeclaration(temp)).Append(semi).Append(nl);
                    }
                }

            // MEMBERS
            foreach (MemberModel method in Members)
                if ((method.Flags & FlagType.Function) > 0)
                {
                    decl = MemberDeclaration(method);
                    sb.Append(nl).Append(CommentDeclaration(method.Comments, tab));
                    sb.Append(tab).Append(decl).Append(semi).Append(nl);
                }

            // END CLASS
            sb.Append(tab0).Append('}');
            return sb.ToString();
        }

        static public string ClassDeclaration(ClassModel ofClass)
        {
            return ClassDeclaration(ofClass, true);
        }

        static public string ClassDeclaration(ClassModel ofClass, bool qualified)
        {
            // package
            if (ofClass.Flags == FlagType.Package)
            {
                return "package " + ofClass.Name.Replace('\\', '.');
            }
            else
            {
                // modifiers
                FlagType ft = ofClass.Flags;
                Visibility acc = ofClass.Access;
                string modifiers = "";
                if ((ofClass.Flags & FlagType.Intrinsic) > 0)
                {
                    if ((ofClass.Flags & FlagType.Extern) > 0) modifiers += "extern ";
                    else modifiers += "intrinsic ";
                }
                else if (ofClass.InFile.Version > 2)
                    if (ofClass.Namespace != null && ofClass.Namespace.Length > 0 
                        && ofClass.Namespace != "internal") 
                    {
                            modifiers += ofClass.Namespace + " ";
                    }
                    else
                    {
                        if ((acc & Visibility.Public) > 0) modifiers += "public ";
                        else if ((acc & Visibility.Internal) > 0) modifiers += "internal ";
                        else if ((acc & Visibility.Protected) > 0) modifiers += "protected ";
                        else if ((acc & Visibility.Private) > 0) modifiers += "private ";
                    }

                if ((ofClass.Flags & FlagType.Dynamic) > 0)
                    modifiers += "dynamic ";
                string classType = "class";
                if ((ofClass.Flags & FlagType.Interface) > 0) classType = "interface";
                else if ((ofClass.Flags & FlagType.Enum) > 0) classType = "enum";
                else if ((ofClass.Flags & FlagType.TypeDef) > 0) classType = "typedef";
                // signature
                if (qualified)
                    return String.Format("{0}{1} {2}", modifiers, classType, ofClass.QualifiedName);
                else
                    return String.Format("{0}{1} {2}", modifiers, classType, ofClass.Name);
            }
        }

        static public string MemberDeclaration(MemberModel member)
        {
            // modifiers
            FlagType ft = member.Flags;
            Visibility acc = member.Access;
            string modifiers = "";
            if ((ft & FlagType.Intrinsic) > 0)
            {
                if ((ft & FlagType.Extern) > 0) modifiers += "extern ";
                else modifiers += "intrinsic ";
            }
            else if (member.Namespace != null && member.Namespace.Length > 0 
                && member.Namespace != "internal")
            {
                modifiers = member.Namespace + " ";
            }
            else
            {
                if ((acc & Visibility.Public) > 0) modifiers += "public ";
                //else if ((acc & Visibility.Internal) > 0) modifiers += "internal "; // AS3 default
                else if ((acc & Visibility.Protected) > 0) modifiers += "protected ";
                else if ((acc & Visibility.Private) > 0) modifiers += "private ";
            }

            if ((ft & FlagType.Class) > 0)
            {
                if ((ft & FlagType.Dynamic) > 0)
                    modifiers += "dynamic ";
                string classType = "class";
                if ((member.Flags & FlagType.Interface) > 0) classType = "interface";
                else if ((member.Flags & FlagType.Enum) > 0) classType = "enum";
                else if ((member.Flags & FlagType.TypeDef) > 0) classType = "typedef";
                return String.Format("{0}{1} {2}", modifiers, classType, member.Type);
            }
            else if ((ft & FlagType.Enum) == 0)
            {
                if ((ft & FlagType.Native) > 0)
                    modifiers += "native ";
                if ((ft & FlagType.Static) > 0)
                    modifiers += "static ";
            }
            // signature
            if ((ft & FlagType.Enum) > 0)
                return member.ToString();
            else if ((ft & (FlagType.Getter | FlagType.Setter)) > 0)
                return String.Format("{0}property {1}", modifiers, member.ToString());
            else if ((ft & FlagType.Function) > 0)
            {
                return String.Format("{0}function {1}", modifiers, member.ToString());
            }
            else if ((ft & FlagType.Namespace) > 0)
            {
                return String.Format("{0}namespace {1}", modifiers, member.Name);
            }
            else if ((ft & FlagType.Variable) > 0)
            {
                if ((ft & FlagType.LocalVar) > 0) modifiers = "local ";
                if ((ft & FlagType.Constant) > 0)
                {
                    if (member.Value == null)
                        return String.Format("{0}const {1}", modifiers, member.ToString());
                    else
                        return String.Format("{0}const {1} = {2}", modifiers, member.ToString(), member.Value);
                }
                else return String.Format("{0}var {1}", modifiers, member.ToString());
            }
            else if (ft == FlagType.Package)
                return String.Format("Package {0}", member.Type);
            else if (ft == FlagType.Template)
                return String.Format("Template {0}", member.Type);
            else if (ft == FlagType.Declaration)
                return String.Format("Declaration {0}", member.Type);
            else
                return String.Format("{0}type {1}", modifiers, member.Type);
        }

        static public string CommentDeclaration(string comment, string tab)
        {
            if (comment == null) return "";
            comment = comment.Trim();
            if (comment.Length == 0) return "";
            Boolean indent = (PluginCore.PluginBase.Settings.CommentBlockStyle == PluginCore.CommentBlockStyle.Indented);
            if (comment.StartsWith("*") || comment.IndexOf('\n') > 0 || comment.IndexOf('\r') > 0)
            {
                if (comment.IndexOf('\n') < 0) comment = comment.Replace("\r", "\r\n");
                else if (comment.IndexOf('\r') < 0) comment = comment.Replace("\n", "\r\n");
                if (indent) return tab + "/**\r\n" + tab + " " + comment + "\r\n" + tab + " */\r\n";
                else return tab + "/**\r\n" + tab + comment + "\r\n" + tab + "*/\r\n";
            }
            else return tab + "/// " + comment + "\r\n";
        }
        #endregion
    }
}
