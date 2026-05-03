using Pms.Bll.Interfaces;
using Pms.Core.Models;

namespace Pms.Bll.Services
{
    public class DataTransfer : IDataTransfer
    {
        private readonly INotificationService notify;

        public DataTransfer(INotificationService notify)
        {
            this.notify = notify;
        }

        public KeyValuePair<string, byte[]>? ExportTask(TaskModel model, string format)
        {
            var context = new System.Runtime.Loader.AssemblyLoadContext("ExportContext", isCollectible: true);

            try
            {
                string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataTransfer.dll");

                using (var fs = new FileStream(dllPath, FileMode.Open, FileAccess.Read))
                {
                    var assembly = context.LoadFromStream(fs);
                    var type = assembly.GetType("DataTransfer.Exporter");
                    dynamic exporter = Activator.CreateInstance(type);

                    byte[] fileBytes;
                    string fileName;

                    if (format == "word")
                    {
                        fileBytes = exporter.ExportToWord(model.Export());
                        fileName = $"Task_{model.Id}.docx";
                    }
                    else
                    {
                        fileBytes = exporter.ExportToExcel(model.Export());
                        fileName = $"Task_{model.Id}.xlsx";
                    }

                    return fileBytes != null ? new KeyValuePair<string, byte[]>(fileName, fileBytes) : null;
                }
            }
            finally
            {
                context.Unload();

                for (int i = 0; i < 2; i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }
    }
}
