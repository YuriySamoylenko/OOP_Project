using Pms.Core.Models;

namespace Pms.Bll.Interfaces
{
    public interface IDataTransfer
    {
        KeyValuePair<string, byte[]>? ExportTask(TaskModel model, string format);
    }
}
