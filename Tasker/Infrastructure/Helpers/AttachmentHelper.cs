using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tasker.Infrastructure.Helpers
{
    public class AttachmentHelper : IAttachmentHelper
    {
        public AttachmentHelper()
        {

        }

        public async Task CreateAttachment(string base64, string fileName, string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                    throw new DirectoryNotFoundException($@"Directory path does not exist: {folderPath}");

                await File.WriteAllBytesAsync(Path.Combine(folderPath, fileName), Convert.FromBase64String(base64));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
