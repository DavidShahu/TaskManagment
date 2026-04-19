using Application.Notifications.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagment.Extencions; 

namespace TaskManagment.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationAppService _notificationService;

    public NotificationsController(
        INotificationAppService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications( CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var notifications = await _notificationService.GetMyNotificationsAsync(userId, cancellationToken);
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount( CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var count = await _notificationService.GetUnreadCountAsync(userId, cancellationToken);
        return Ok(new { count });
    }

    [HttpPatch("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead( CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
        return Ok();
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _notificationService.MarkAsReadAsync(id, cancellationToken);
        return Ok();
    }
}