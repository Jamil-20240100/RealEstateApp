using RealEstateApp.Core.Application.ViewModels.Agent;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Application.ViewModels.User;

namespace RealEstateApp.Core.Application.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public List<PropertyViewModel>? Properties { get; set; }
        public List<UserViewModel>? Agents { get; set; }
        public List<UserViewModel>? Clients { get; set; }
        public List<UserViewModel>? Developers { get; set; }
    }
}
