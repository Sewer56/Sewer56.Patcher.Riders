using System;
using System.IO;
using System.Reflection;

namespace Sewer56.Patcher.Regravitified.Lib.Utility
{
    public static class Paths
    {
        public static readonly string ProgramFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        public static string GetRelativePath(string fullPath, string folderPath) => fullPath.Substring(folderPath.Length);

        public static string AppendRelativePath(string relativePath, string folderPath) => folderPath + relativePath;
    }
}
