
using TaskFlow.Models;

namespace TaskFlow.ViewModels;

public class DashboardViewModel
{
    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int InProgressTasks { get; set; }

    public int PendingTasks { get; set; }

    public List<TaskItem> RecentTasks { get; set; } = new();
}
