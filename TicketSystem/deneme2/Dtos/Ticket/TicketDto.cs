﻿using TicketSystem.Models;

namespace TicketSystem.Dtos.Ticket
{
     public class TicketDto
     {
          public int Id { get; set; }
          public string? Title { get; set; }
          public string? Description { get; set; }
          public DateTime Created { get; set; }
          public string CreatedBy { get; set; } = string.Empty;
     }
}