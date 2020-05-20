using UnityEditor;
using UnityEngine;

namespace Smart
{
    using static GUILayout;
    using static EditorGUILayout;
    using System.IO;

    public class CreateScript : EditorWindow
    {
        public enum Accesability
        {
            Public,
            Private,
            Protected,
            Internal,
            ProtectedInternal,
            PrivateProtected
        }

        private string script;
        private string fileName;
        private string className;
        private string inheritanceName;
        private string namespaceName;
        private bool isPartial;
        private bool isStatic;
        private bool isSealed;
        private bool isAbstract;
        private bool hasNamespace;
        private Accesability access;

        [MenuItem("Tools/Smart/Create Script")]
        static void Open()
        {
            GetWindow<CreateScript>("Create Script");
        }

        private void OnGUI()
        {
            Fields();
            ValidateFields();
            CreateButton();
        }

        void Fields()
        {
            fileName = TextField("File Name", fileName);
            className = TextField("Class Name", className);
            inheritanceName = TextField("Inheritance Name", inheritanceName);
            namespaceName = TextField("Namespace Name", namespaceName);
            isPartial = Toggle("Partial?", isPartial);
            isStatic = Toggle("Static?", isStatic);
            isSealed = Toggle("Sealed?", isSealed);
            isAbstract = Toggle("Abstract?", isAbstract);
            access = (Accesability)EnumFlagsField("Accesability", access);
        }

        void ValidateFields()
        {

            if(isStatic && isSealed)
            {
                HelpBox("A class can't be both static and sealed", MessageType.Error);
            }

            bool staticOrSealed = isStatic || isSealed;

            if (isAbstract && staticOrSealed)
            {
                HelpBox("An abstract class can't be static or sealed", MessageType.Error);
            }
        }

        void CreateButton()
        {
            if (Button("Create"))
            {
                Create();
            }
        }

        void Create()
        {
            string path = GetPath();

            if (string.IsNullOrWhiteSpace(path)) { return; }

            script = "";
            BeginNamespace();
            Format("{0}{1} {2} class {3}{4}\n", GetIndentantion(), GetAcess(), GetModifiers(), className, GetInheritance());
            script += GetIndentantion() + "{\n";
            script += GetIndentantion() + "}\n";
            EndNamespace();

            Debug.Log(script);

            File.WriteAllText(path, script);
            AssetDatabase.Refresh();
        }

        string GetPath()
        {
            string path = EditorUtility.SaveFilePanel("Create Script", "Assets", fileName, "cs");

            path = path.Replace(Application.dataPath, "Assets");

            return path;
        }

        void BeginNamespace()
        {
            hasNamespace = !string.IsNullOrWhiteSpace(namespaceName);

            if (!hasNamespace) { return; }

            Format("namespace {0}\n", namespaceName);
            script += "{\n";
        }

        void EndNamespace()
        {
            if (!hasNamespace) { return; }

            script += "}\n";
        }

        string GetIndentantion()
        {

            string value = "";

            if(hasNamespace)
            {
                value += "\t";
            }

            return value;
        }

        void Format(string format, params object[] args)
        {
            script += string.Format(format, args);
        }

        object GetAcess()
        {
            return access.ToString().ToLower();
        }

        object GetModifiers()
        {
            string value = "";
            bool addSpace = false;

            if (isAbstract)
            {
                value += addSpace ? "abstract " : "abstract";
                addSpace = true;
            }

            if (isStatic)
            {
                value += addSpace ? "static " : "static";
                addSpace = true;
            }

            if (isSealed)
            {
                value += addSpace ? "sealed " : "sealed";
                addSpace = true;
            }

            if (isPartial)
            {
                value += addSpace ? "partial " : "partial";
                addSpace = true;
            }

            return value;
        }

        object GetInheritance()
        {
            string value = "";

            if (string.IsNullOrWhiteSpace(inheritanceName)) { return value; }

            return string.Format(" : {0}", inheritanceName);
        }
    }
}
