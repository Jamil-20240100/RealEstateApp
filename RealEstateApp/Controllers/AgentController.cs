using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Agent;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Controllers
{
    public class AgentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPropertyService _propertyService;
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;
        private readonly IMapper _mapper;

        public AgentController(IAccountServiceForWebApp accountServiceForWebApp, IMapper mapper, UserManager<AppUser> userManager, IPropertyService propertyService)
        {
            _propertyService = propertyService;
            _userManager = userManager;
            _accountServiceForWebApp = accountServiceForWebApp;
            _mapper = mapper;
        }

        private async Task<UserViewModel?> ValidateUserAsync()
        {
            var userSession = await _userManager.GetUserAsync(User);
            if (userSession == null) return null;

            var user = await _accountServiceForWebApp.GetUserByUserName(userSession.UserName ?? "");
            var mappedUser = _mapper.Map<UserViewModel>(user);
            return mappedUser;
        }

        public async Task<IActionResult> Index()
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var dtos = await _accountServiceForWebApp.GetAllUserByRole(Roles.Agent.ToString());
            var agents = _mapper.Map<List<UserViewModel>>(dtos);

            var allProperties = await _propertyService.GetAll();

            foreach (var agent in agents)
            {
                agent.PropertiesQuantity = allProperties.Count(p => p.AgentId == agent.Id);
            }

            return View(agents);
        }

        public async Task<IActionResult> ShowAgents()
        {
            var agents = await _accountServiceForWebApp.GetAllUserByRole(Roles.Agent.ToString());
            var mappedAgents = _mapper.Map<List<AgentViewModel>>(agents).OrderBy(a => a.Name).ToList();
            return View(mappedAgents);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            bool status = false;
            var agent = await _accountServiceForWebApp.GetUserById(id);
            if (agent == null) return NotFound();

            if(agent.IsActive == false)
                status = true;
            else
                status = false;

            await _accountServiceForWebApp.ChangeStatusAsync(id, status);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteConfirmation(string id)
        {
            var agent = await _accountServiceForWebApp.GetUserById(id);
            if (agent == null) return NotFound();

            var viewModel = _mapper.Map<UserViewModel>(agent);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var agent = await _accountServiceForWebApp.GetUserById(id);
            if (agent == null) return NotFound();

            await _accountServiceForWebApp.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
