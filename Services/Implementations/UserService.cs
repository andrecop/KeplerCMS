﻿using KeplerCMS.Data;
using KeplerCMS.Data.Models;
using KeplerCMS.Helpers;
using KeplerCMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeplerCMS.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ICommandQueueService _commandQueueService;
        private readonly IFuseService _fuseService;
        private readonly DataContext _context;

        public UserService(ICommandQueueService commandQueueService, IFuseService fuseService, DataContext context)
        {
            _commandQueueService = commandQueueService;
            _fuseService = fuseService;
            _context = context;
        }

        public async Task<Users> GetUserByUsername(string username)
        {
            var user = await _context.Users.Where(user => user.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();
            if(user != null)
            {
                user.Fuses = await _fuseService.GetFusesByRank(user.Rank);
            }
            return user;
        }

        public async Task<Users> GetUserById(string id)
        {
            var user = await _context.Users.Where(user => user.Id == int.Parse(id)).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Fuses = await _fuseService.GetFusesByRank(user.Rank);
            }
            return user;
        }

        public void UpdateFigure(string userId, string figureData, string newGender)
        {
            var user = _context.Users.Where(u => u.Id == int.Parse(userId)).FirstOrDefault();
            user.Figure = FigureHelper.FixFigure(figureData);
            user.Gender = newGender;
            _context.SaveChanges();

            _commandQueueService.QueueCommand(Models.Enums.CommandQueueType.refresh_appearance, userId);
        }
    }

}