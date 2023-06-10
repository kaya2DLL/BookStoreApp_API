﻿using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IServiceManager _service;

        public CategoriesController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            return Ok(await _service
                .CategoryService
                .GetAllCategoriesAsync(false));
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneCategories([FromRoute]int id)
        {
            return Ok(await _service
                .CategoryService
                .GetOneCategoryByIdAsync(id,false));
        }
    }
}
