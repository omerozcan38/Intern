﻿using TicketSystem.Data;
using TicketSystem.Dtos.Ticket;
using TicketSystem.Helpers;
using TicketSystem.Interfaces;
using TicketSystem.Models;
using TicketSystem.Models.TicketModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace TicketSystem.Repository
{
     public class TicketRepository : ITicketRepository
     {
          private readonly ApplicationDbContext _context;
          private readonly UserManager<AppUser> _userManager;
          private readonly ILogger<TicketRepository> _logger;
          public TicketRepository(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<TicketRepository> logger)
          {
               _context = context;
               _userManager = userManager;
               _logger = logger;
          }

          public async Task<Ticket> CreateAsync(Ticket ticketModel)
          {
               ticketModel.Status = 1;
               await _context.Tickets.AddAsync(ticketModel);
               await _context.SaveChangesAsync();
               return ticketModel;
          }

          public async Task<AppUserTicket> CreateAsyncs(AppUserTicket appuserticket)
          {

               await _context.AppUserTickets.AddAsync(appuserticket);
               await _context.SaveChangesAsync();
               return appuserticket;
          }

          public async Task<Ticket?> DeleteAsync(int id)
          {
               var ticketModel = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == id);

               if (ticketModel == null)
               {
                    return null;
               }
               _context.Tickets.Remove(ticketModel);

               await _context.SaveChangesAsync();
               return ticketModel;
          }

          public async Task<List<TicketDto>> GetAllAsync()
          {
               var tickets = await _context.Tickets
                   .Select(ticket => new TicketDto
                   {
                        Id = ticket.Id,
                        Title = ticket.Title,
                        Description = ticket.Description,
                        Created= ticket.Created,
                        Status = ticket.Status,
                        FirmName = (from pt in _context.ProductTickets
                                    join fp in _context.FirmProducts on pt.ProductId equals fp.ProductId
                                    join f in _context.Firms on fp.FirmId equals f.Id
                                    where pt.TicketId == ticket.Id
                                    select f.Name).FirstOrDefault(),
                        ProductName = (from pt in _context.ProductTickets
                                       join p in _context.Products on pt.ProductId equals p.Id
                                       where pt.TicketId == ticket.Id
                                       select p.Name).FirstOrDefault()
                   }).ToListAsync();

               return tickets;
          }

          public async Task<Ticket?> GetByIdAsync(int id)
          {
               return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
          }

          public async Task<Ticket?> GetByNameAsync(string name)
          {
               return await _context.Tickets.FirstOrDefaultAsync(ticket => ticket.Title == name);
          }

          public async Task<Ticket?> UpdateAsync(int id, UpdateTicketRequestDto ticketDto)
          {
               var existingTicket = await _context.Tickets.FirstOrDefaultAsync(ticket => ticket.Id == id);
               if (existingTicket == null)
               {
                    return null;
               }
               existingTicket.Answer = ticketDto.Answer;
               existingTicket.Status = ticketDto.Status;
               existingTicket.Updated = DateTime.Now;

               await _context.SaveChangesAsync();
               return existingTicket;
          }
          public async Task<Ticket?> UpdateTicketStatusAsync(Ticket ticket)
          {
               ticket.Updated = DateTime.Now;
               _context.Tickets.Update(ticket);
               await _context.SaveChangesAsync();
               return ticket;
          }
     }
}