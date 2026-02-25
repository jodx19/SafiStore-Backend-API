public class CreateReviewDto
{
    public int ProductId { get; set; }
    public int Rating { get; set; } // من 1 لـ 5
    public string Comment { get; set; }
}
