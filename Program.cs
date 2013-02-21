using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchStyleCopAsErrors
{
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;

    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(Environment.CurrentDirectory, "*.csproj", SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                try
                {
                    var document = new XmlDocument();
                    document.Load(filePath);

                    XPathNavigator nav = document.DocumentElement.CreateNavigator();
                    bool flag = false;

                    if (nav.MoveToChild("PropertyGroup", "http://schemas.microsoft.com/developer/msbuild/2003"))
                    {
                        do
                        {
                            if (nav.Name == "PropertyGroup")
                            {
                                flag |= ProcessItemGroupNode(nav);
                            }
                        }
                        while (nav.MoveToNext());

                        if (flag)
                        {
                            document.Save(filePath);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Unable to update the file {0}: {1}", filePath, exception.Message);
                }
            }
        }

        private static bool ProcessItemGroupNode(XPathNavigator nav)
        {
            try
            {
                if (nav.MoveToChild("StyleCopTreatErrorsAsWarnings", "http://schemas.microsoft.com/developer/msbuild/2003"))
                {
                    if (nav.Value == "false")
                    {
                        nav.ReplaceSelf("<StyleCopTreatErrorsAsWarnings>true</StyleCopTreatErrorsAsWarnings>");
                    }
                    else
                    {
                        nav.ReplaceSelf("<StyleCopTreatErrorsAsWarnings>false</StyleCopTreatErrorsAsWarnings>");
                    }
                }
                else
                {
                    nav.AppendChild("<StyleCopTreatErrorsAsWarnings>true</StyleCopTreatErrorsAsWarnings>");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
