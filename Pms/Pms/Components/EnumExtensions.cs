using Pms.Core.Enums;

namespace Pms.Components
{
    public static class EnumExtensions
    {
        public static string GetStatusClass(this Status status) => status switch
        {
            Status.InProgress => "bg-success",
            Status.New => "bg-info",
            Status.Completed => "bg-secondary",
            _ => "bg-dark"
        };
        public static string GetStatusClass(this PmsTaskStatus status) => status switch
        {
            PmsTaskStatus.InProgress => "bg-success",
            PmsTaskStatus.ToDo => "bg-info",
            PmsTaskStatus.Reopened => "bg-info",
            PmsTaskStatus.Resolved => "bg-secondary",
            PmsTaskStatus.CouldNotReproduce => "bg-secondary",
            PmsTaskStatus.NotABug => "bg-secondary",
            _ => "bg-dark"
        };
    }
}
