namespace RaceDay.API.DTOs;

public record EventCreateDto(
    string Title,
    string Description,
    DateTime Date,
    string Location,
    double Distance,
    string EventType
);