using AutoMapper;
using ElAlProjectCore.DTOs.RequstDTOs.AdminRequests;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs;
using ElAlProjectCore.Models;

namespace ElAlProjectService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AdminRequest_FlightDTO, Flight>();
            CreateMap<Flight, AdminResponse_FlightDTO>();
            CreateMap<Flight, PassengerResponse_FlightDTO>();

            CreateMap<AdminRequest_AmenityDTO, Amenity>();
            CreateMap<Amenity, AdminResponse_AmenityDTO>();
            CreateMap<Amenity, PassengerResponse_AmenityDTO>();

            CreateMap<Order, AdminResponse_OrderDTO>();
            CreateMap<Order, PassengerResponse_OrderDTO>();

            CreateMap<Passenger, AdminResponse_PassengerDTO>();
            CreateMap<Admin, AdminResponse_AdminDTO>();

            CreateMap<Passenger,PassengerResponse_PassengerDTO>();
        }
    }
}
