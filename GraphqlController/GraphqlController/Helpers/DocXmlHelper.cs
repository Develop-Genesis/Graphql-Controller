using LoxSmoke.DocXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GraphqlController.Helpers
{
    public static class DocXmlHelper
    {
        public static DocXmlReader DocReader { get; private set; }
            = new DocXmlReader((assembly) =>
         {
             string codeBase = assembly.CodeBase;
             UriBuilder uri = new UriBuilder(codeBase);
             string path = Uri.UnescapeDataString(uri.Path);
             var directoryPath = Path.GetDirectoryName(path);
             var fileName = Path.GetFileNameWithoutExtension(path) + ".xml";
             return Path.Combine(directoryPath, fileName);
         });
    }
}
