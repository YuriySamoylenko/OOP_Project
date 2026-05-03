using Pms.Bll.Interfaces;

public class NotificationService : INotificationService
{
    public event Action<string>? OnErrorMessage;

    public void NotifyError(string message)
    {
        OnErrorMessage?.Invoke(message);
    }
}
