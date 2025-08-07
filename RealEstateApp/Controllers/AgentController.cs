using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Agent;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Agent;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Controllers
{
    public class AgentController : Controller
    {
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;
        private readonly IMapper _mapper;

        public AgentController(IAccountServiceForWebApp accountServiceForWebApp, IMapper mapper)
        {
            _accountServiceForWebApp = accountServiceForWebApp;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ShowAgents()
        {
            var agents = await _accountServiceForWebApp.GetAllUserByRole(Roles.Agent.ToString(), false);
            var mappedAgents = _mapper.Map<List<AgentViewModel>>(agents).OrderBy(a => a.Name).ToList();
            return View(mappedAgents);
        }
    }
}
