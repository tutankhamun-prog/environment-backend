using AutoMapper;
using EnvironmentsService.Application.DTOs.Requests;
using EnvironmentsService.Application.DTOs.Responses;
using EnvironmentsService.Domain.Entities;
using Environment = EnvironmentsService.Domain.Entities.Environment;

namespace EnvironmentsService.Application.Mappings
{
    public class EnvironmentMappingProfile : Profile
    {
        public EnvironmentMappingProfile()
        {
            // Mapping : Entité → DTO Response
            CreateMap<Environment, EnvironmentResponseDto>();

            // Mapping : DTO Request → Entité
            CreateMap<CreateEnvironmentRequestDto, Environment>();
            CreateMap<UpdateEnvironmentRequestDto, Environment>();

            // Mapping pour EnvironmentMember
            CreateMap<EnvironmentMember, EnvironmentMemberResponseDto>();
        }
    }
}