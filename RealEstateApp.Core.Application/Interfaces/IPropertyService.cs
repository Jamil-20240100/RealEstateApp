using RealEstateApp.Core.Application.ViewModels.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public interface IPropertyService
    {
        Task<List<PropertyViewModel>> GetFilteredAvailableAsync(PropertyFilterViewModel filters, string? userId);
    Task<PropertyViewModel?> GetPropertyDetailsAsync(int id, string? userId);
}

