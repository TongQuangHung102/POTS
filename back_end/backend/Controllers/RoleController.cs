﻿using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("get-all-role")]
        public async Task<IActionResult> GetAllRoles()
        {
            return await _roleService.GetAllRolesAsync();
        }

        [HttpGet("get-role-by/{roleId}")]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            return await _roleService.GetRoleByIdAsync(roleId);
        }
    }
}
