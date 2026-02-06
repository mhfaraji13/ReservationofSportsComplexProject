using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ReservationSportsComplex.Application.DTOs;
using ReservationSportsComplex.Domain.Entities.Model;

namespace ReservationSportsComplex.Application.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<SportHall, AddSportHallDTO>().ReverseMap();
            CreateMap<SportHall, SportHallDTO>().ReverseMap();
            CreateMap<SportHall, UpdateSportHallDTO>().ReverseMap();
        }
    }
}
