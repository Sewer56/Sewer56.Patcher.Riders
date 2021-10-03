using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sewer56.DeltaPatchGenerator.Lib.Utility;

namespace Sewer56.Patcher.Riders.Common.Utility
{
    /// <summary>
    /// Struct that can be used to temporarily rename a file.
    /// Dispose it to revert the change, or best, use the `using` statement.
    /// </summary>
    public struct TemporarilyRenameFile : IDisposable
    {
        public string FilePath    { get; private set; }
        public string NewFileName { get; private set; }

        public TemporarilyRenameFile(string filePath, string newFileName)
        {
            NewFileName = newFileName;
            FilePath = filePath;
            File.Move(FilePath, GetNewFilePath(), true);
        }

        public void Dispose()
        {
            try
            {
                File.Move(GetNewFilePath(), FilePath, true);
            }
            catch (Exception e)
            {
                /* Ignore if file was deleted. */
            }
        }

        private string GetNewFilePath() => Path.Combine(Path.GetDirectoryName(FilePath), NewFileName);
    }
}
