using AutoMapper;
using Application.Dtos.Payment;
using Domain.Models;

namespace Application.MappingProfiles.Payment
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Domain.Models.Payment, PaymentResponseDto>();
            CreateMap<InitiatePaymentRequestDto, Domain.Models.Payment>();
        }
    }
}
