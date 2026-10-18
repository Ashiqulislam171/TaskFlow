
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

    public TasksController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(
        string? searchTerm,
        string? status,
        string? priority)
    {
        var userId = _userManager.GetUserId(User);

        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t =>
                t.Title.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(t => t.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(priority))
        {
            query = query.Where(t => t.Priority == priority);
        }

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
    public async Task<IActionResult> UpdateStatus(
        int id,
        string status)
    {
        var allowedStatuses = new[]
        {
            "Pending",
            "In Progress",
            "Completed"
        };

        if (!allowedStatuses.Contains(status))
        {
            return BadRequest("Invalid task status.");
        }

        var userId = _userManager.GetUserId(User);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId);

        if (task == null)
        {
            return NotFound();
        }

        task.Status = status;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Task status updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Title,Description,Status,Priority,DueDate")]
        TaskItem task)
    {
        if (!ModelState.IsValid)
        {
            return View(task);
        }

        task.UserId = _userManager.GetUserId(User);
        task.CreatedAt = DateTime.UtcNow;

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Task created successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId);

        if (task == null)
        {
            return NotFound();
        }

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
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(updatedTask);
        }

        var userId = _userManager.GetUserId(User);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId);

        if (task == null)
        {
            return NotFound();
        }

        task.Title = updatedTask.Title;
        task.Description = updatedTask.Description;
        task.Status = updatedTask.Status;
        task.Priority = updatedTask.Priority;
        task.DueDate = updatedTask.DueDate;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Task updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = _userManager.GetUserId(User);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId);

        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Task deleted successfully!";

        return RedirectToAction(nameof(Index));
    }
}
