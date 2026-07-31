namespace TaskManagement.Api.DTOS
{
    public class TaskQuery:PaginationQuery
    {
        public string? Search { get; set; }
        public bool? Completed { get; set; }
        public string SortBy { get; set; } = "id";
        public bool Descending { get; set; } = false;
        public bool IncludeDeleted { get; set; } = false;
    }
}
