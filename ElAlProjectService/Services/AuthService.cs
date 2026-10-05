using AutoMapper;
using ElAlProjectCore.DTOs.RequstDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs;
using ElAlProjectCore.Models;
using ElAlProjectCore.Repositories;
using ElAlProjectCore.Services;
using ElAlProjectService.Security;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;


        public AuthService(
           IAdminRepository adminRepository,
           IPassengerRepository passengerRepository,
           
           IMapper mapper,
           ILogger<AuthService> logger)
        {
            _adminRepository = adminRepository;
            _passengerRepository = passengerRepository;
            
            _mapper = mapper;
            _logger = logger;
        }



        public async Task<LoginResultDTO> RegisterAsync(AuthRequestDTO authRequest, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetByEmailForLoginAsync(authRequest.Email, cancellationToken);
            var existing = await _passengerRepository.GetByEmailForLoginAsync(authRequest.Email, cancellationToken);

            if (admin != null || existing != null)
                throw new InvalidOperationException("Email already exists.");

            var passenger = new Passenger
            {
                Name = authRequest.Name,
                Email = authRequest.Email,
                Passward = PasswordHasher.Hash(authRequest.Password)
            };

            passenger = await _passengerRepository.AddPassenger(passenger, cancellationToken);

            return BuildResult("Passenger", passenger.Id, passenger.Name, passenger.Email,
                passengerProfile: _mapper.Map<PassengerResponse_PassengerDTO>(passenger));
        }
        public async Task<LoginResultDTO> LoginAsync(LoginModel loginModel, CancellationToken cancellationToken)
        {

            var admin = await _adminRepository.GetByEmailForLoginAsync(loginModel.Email, cancellationToken);
            if (admin != null)
            {

                if (!PasswordHasher.Verify(loginModel.Password, admin.Password))
                    return null;

                return BuildResult("Admin", admin.Id, admin.Name, admin.Email,
                    adminProfile: _mapper.Map<AdminResponse_AdminDTO>(admin));
            }



            var passenger = await _passengerRepository.GetByEmailForLoginAsync(loginModel.Email, cancellationToken);
            if (passenger != null)
            {

                if (!PasswordHasher.Verify(loginModel.Password, passenger.Passward))
                    return null;

                return BuildResult("Passenger",passenger.Id, passenger.Name, passenger.Email,
                    passengerProfile: _mapper.Map<PassengerResponse_PassengerDTO>(passenger));
            }

            throw new KeyNotFoundException($"No account registered with email {loginModel.Email}.");

        }





        private static LoginResultDTO BuildResult(
            string role, int id, string name, string email,
            AdminResponse_AdminDTO? adminProfile = null,
            PassengerResponse_PassengerDTO? passengerProfile = null
            )
        {



            return new LoginResultDTO
            {
                Id = id,
                Role = role,
                Name = name,
                Email = email,
                AdminProfile = adminProfile,
                PassengerProfile = passengerProfile

            };
        }

    }
}
