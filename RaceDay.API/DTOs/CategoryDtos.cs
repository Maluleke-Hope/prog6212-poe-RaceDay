namespace RaceDay.API.DTOs
{
	public class CreateCategoryDto
	{
		public string Name { get; set; } = string.Empty;
		public int EventId { get; set; }
	}

	public class CategoryReadDto
	{
		public int CategoryId { get; set; }
		public string Name { get; set; } = string.Empty;
		public int EventId { get; set; }
	}
}