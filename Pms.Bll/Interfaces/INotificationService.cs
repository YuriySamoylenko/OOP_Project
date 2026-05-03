namespace Pms.Bll.Interfaces
{
    public interface INotificationService
    {
        event Action<string>? OnErrorMessage;

        void NotifyError(string message);
    }
}
