﻿using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly IBookService _bookService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICategoryService _categoryService;

        public ServiceManager(IBookService bookService,
            IAuthenticationService authenticationService,
            ICategoryService categoryService)
        {
            _bookService = bookService;
            _authenticationService = authenticationService;
            _categoryService = categoryService;
        }

        public IAuthenticationService AuthenticationService => _authenticationService;

        public ICategoryService CategoryService => _categoryService;

        public IBookService BookService => _bookService;
    }
}
