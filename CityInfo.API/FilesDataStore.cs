namespace CityInfo.API;
public class FilesDataStore
{
    public static List<FileInfo> Files { get; set; } = new List<FileInfo>()
    {
        new FileInfo("StaticFiles/EssentialHTTPStatusCodes.docx"),
        new FileInfo("StaticFiles/EssentialHTTPStatusCodes.pdf")
    };
}