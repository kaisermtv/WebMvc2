using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Application.Entities;

namespace WebMvc.ViewModels
{
    public class EmployeesViewModel
    {
        public List<EmployeesRole> employeesRoles;
        public List<Employees> employees;
    }
}