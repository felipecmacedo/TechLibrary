namespace TechLibrary.Communication.Reponses;

public class ResponseBooksJson
{
    public ResponsePaginationJson Pagination { get; set; } = default!;
    public List<ResponseBookJson> Books { get; set; } = [];
}