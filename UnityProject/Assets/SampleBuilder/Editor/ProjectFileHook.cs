using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using UnityEditor;

#if ENABLE_VSTU

using SyntaxTree.VisualStudio.Unity.Bridge;

[InitializeOnLoad]
public class ProjectFileHook
{
    // necessary for XLinq to save the xml project file in utf8
    class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    static ProjectFileHook()
    {
        ProjectFilesGenerator.ProjectFileGeneration += (string name, string content) =>
        {
            // parse the document and make some changes
            var document = XDocument.Parse(content);
            var ns = document.Root.Name.Namespace;

            document.Root
                .Descendants()
                .First(x => x.Name.LocalName == "PropertyGroup")
                .Add(new XElement(ns + "ImplicitlyExpandNETStandardFacades", "false"),
                new XElement(ns + "ImplicitlyExpandDesignTimeFacades", "false"));

            // save the changes using the Utf8StringWriter
            var str = new Utf8StringWriter();
            document.Save(str);

            return str.ToString();
        };
    }
}

#endif
