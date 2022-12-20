﻿using Microsoft.AspNetCore.Mvc;
using KeplerCMS.Filters;
using KeplerCMS.Services.Interfaces;
using System.Threading.Tasks;
using KeplerCMS.Areas.Housekeeping.Models.Views;
using KeplerCMS.Models;
using KeplerCMS.Data.Models;
using System.Collections;
using System.Collections.Generic;

namespace KeplerCMS.Areas.Housekeeping
{
    [Area("Housekeeping")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HousekeepingFilter(Fuse.fuse_kick)]
        public async Task<IActionResult> Index(string search = null, int currentPage = 1, string letter = null)
        {
            const int pageSize = 25;
            var take = pageSize;
            var skip = (currentPage - 1) * pageSize;
            var searchResult = await _userService.SearchUsers(search, take, skip, letter);
            var model = new UsersViewModel
            {
                Users = searchResult.Users,
                Search = search,
                Letter = letter,
                CurrentPage = currentPage,
                TotalPages = searchResult.TotalResults / pageSize
            };
            return View(model);
        }
       
    }
}
