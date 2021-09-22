using System;
using System.Collections.Generic;
using System.Text;

namespace Sewer56.Patcher.Regravitified.Lib.Utility
{
    public static class ThrowHelpers
    {
        public static void ThrowIfNullOrEmpty(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
                throw new ArgumentException($"Parameter {parameterName} is null or empty.");
        }

        public static void ThrowVerificationFailed(string blurb, List<string> missingFiles, List<string> mismatchFiles)
        {
            var message = new StringBuilder();
            message.AppendLine(blurb);

            if (missingFiles.Count > 0)
            {
                message.AppendLine("Missing Files:");
                foreach (var missingFile in missingFiles)
                    message.AppendLine(missingFile);
            }

            if (mismatchFiles.Count > 0)
            {
                message.AppendLine("Mismatched Files:");
                foreach (var mismatchFile in mismatchFiles)
                    message.AppendLine(mismatchFile);
            }

            throw new Exception(message.ToString());
        }
    }
}
