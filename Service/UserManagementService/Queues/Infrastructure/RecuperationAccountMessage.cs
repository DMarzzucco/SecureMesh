using System;

namespace UserManagementService.Queues.Infrastructure;

public class RecuperationAccountMessage
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public int Id { get; set; }
}
