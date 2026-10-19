
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    private const int MaxSearchLength = 100;

    private static readonly string[] AllowedStatuses =
    {
        "Pending", "In Progress", "Completed"
    };

    private static readonly string[] AllowedPriorities =
    {
        "Low", "Medium", "High"
    };

    public TasksController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private string? CurrentUserId => _userManager.GetUserId(User);

    public async Task<IActionResult> Index(
        string? searchTerm, string? status, string? priority)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();

            if (searchTerm.Length > MaxSearchLength)
                searchTerm = searchTerm[..MaxSearchLength];
        }
        else
        {
            searchTerm = null;
        }

        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (searchTerm != null)
            query = query.Where(t => t.Title.Contains(searchTerm));

        if (status != null && AllowedStatuses.Contains(status))
            query = query.Where(t => t.Status == status);
        else
            status = null;

        if (priority != null && AllowedPriorities.Contains(priority))
            query = query.Where(t => t.Priority == priority);
        else
            priority = null;

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.SelectedStatus = status;
        ViewBag.SelectedPriority = priority;

        return View(tasks);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string? status)
    {
        if (status == null || !AllowedStatuses.Contains(status))
            return BadRequest("Invalid task status.");

        var userId = CurrentUserId;

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var task = await _context.Tasks.FirstOrDefaultAsync(t =>
            t.Id == id && t.UserId == userId);

        if (task == null)
            return NotFound();

        task.Status = status;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Task status updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Title,Description,Status,Priority,DueDate")]
        TaskItem task)
    {
        if (task.Status == null ||
            !AllowedStatuses.Contains(task.Status))
        {
            ModelState.AddModelError(
                nameof(task.Status), "Please select a valid status.");
        }

        if (task.Priority == null ||
            !AllowedPriorities.Contains(task.Priority))
        {
            ModelState.AddModelError(
                nameof(task.Priority), "Please select a valid priority.");
        }

        if (!ModelState.IsValid)
            return View(task);

        var userId = CurrentUserId;

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        task.UserId = userId;
        task.CreatedAt = DateTime.UtcNow;

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Task created successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var task = await _context.Tasks.FirstOrDefaultAsync(t =>
            t.Id == id && t.UserId == userId);

        if (task == null)
            return NotFound();

        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Title,Description,Status,Priority,DueDate")]
        TaskItem updatedTask)
    {
        if (id != updatedTask.Id)
            return BadRequest();

        if (updatedTask.Status == null ||
            !AllowedStatuses.Contains(updatedTask.Status))
        {
            ModelState.AddModelError(
                nameof(updatedTask.Status), "Please select a valid status.");
        }

        if (updatedTask.Priority == null ||
            !AllowedPriorities.Contains(updatedTask.Priority))
        {
            ModelState.AddModelError(
                nameof(updatedTask.Priority), "Please select a valid priority.");
        }

        if (!ModelState.IsValid)
            return View(updatedTask);

        var userId = CurrentUserId;

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var task = await _context.Tasks.FirstOrDefaultAsync(t =>
            t.Id == id && t.UserId == userId);

        if (task == null)
            return NotFound();

        task.Title = updatedTask.Title ?? string.Empty;
        task.Description = updatedTask.Description ?? string.Empty;
        task.Status = updatedTask.Status ?? "Pending";
        task.Priority = updatedTask.Priority ?? "Medium";
        task.DueDate = updatedTask.DueDate;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Task updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var task = await _context.Tasks.FirstOrDefaultAsync(t =>
            t.Id == id && t.UserId == userId);

        if (task == null)
            return NotFound();

        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var task = await _context.Tasks.FirstOrDefaultAsync(t =>
            t.Id == id && t.UserId == userId);

        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Task deleted successfully!";

        return RedirectToAction(nameof(Index));
    }
}
