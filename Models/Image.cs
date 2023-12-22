namespace MWF_Web_Api.Models;

public class Image
{
    public byte[] Bytes { get; set; }

    public ImageExtension Extension { get; set; }

    public Image(byte[] bytes, ImageExtension extension)
    {
        Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        Extension = extension;
    }
}