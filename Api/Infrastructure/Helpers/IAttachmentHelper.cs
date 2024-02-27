using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Infrastructure.Helpers
{
    public interface IAttachmentHelper
    {
        Task CreateAttachment(string base64, string fileName, string folderPath);
    }
}
