using AutoMapper;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Model.TaskModel;

namespace TaskManagement.Api.Mapping
{
    public class TaskMappingProfile : Profile //profile: Bu sınıfın içinde mapping kuralları bulunuyor. demek
    {
        public TaskMappingProfile()
        {
            CreateMap<TaskModel, TaskDto>(); //Generic yapıdaki ilk tür kaynak, ikinci tür hedeftir:
            CreateMap<CreateTaskDto, TaskModel>();//Generic yapıdaki ilk tür kaynak, ikinci tür hedeftir:
            CreateMap<UpdateTaskDto, TaskModel>();//Generic yapıdaki ilk tür kaynak, ikinci tür hedeftir:
            //Property isimleri ve türleri aynı olduğu için AutoMapper otomatik eşleştirir:
        }
    }
}
