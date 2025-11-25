using AutoMapper;
using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.Models;

namespace Expense_Tracker.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            //Expense Mappings
            CreateMap<CreateExpenseDTO, Expense>();
            CreateMap<UpdateExpenseDto, Expense>(); 
            CreateMap<Expense, UpdateExpenseDto>();
        }
    }
}
