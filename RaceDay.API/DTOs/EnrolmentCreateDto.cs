namespace RaceDay.API.DTOs;

public record EnrolmentCreateDto(int EventId, int CategoryId)
{
};

public record EnrolmentResponseDto(int EnrolmentId, int EventId, int UserId, string Status);