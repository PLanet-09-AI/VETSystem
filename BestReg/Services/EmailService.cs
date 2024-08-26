using MimeKit;
using MailKit.Net.Smtp;
using System.IO;
using System.Text.Encodings.Web;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using IronBarCode;
using ZXing.QrCode;
using ZXing;
using ZXing.QrCode;
using System.Drawing;
using System.Drawing.Imaging;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string callbackUrl);
    Task SendQrCodeEmailAsync(string email, string qrCodeBase64);
    byte[] GenerateQrCode(string text);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendConfirmationEmailAsync(string email, string callbackUrl)
    {
        var smtpSettings = _configuration.GetSection("Smtp");
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Admin", "adaboost6@gmail.com"));

        message.To.Add(new MailboxAddress(string.Empty, email));

        message.Subject = "Confirm your email";

        message.Body = new TextPart("html")
        {
            Text = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(smtpSettings["smtp.gmail.com"], int.Parse(smtpSettings["587"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpSettings["adaboost6@gmail.com"], smtpSettings["@Thelab1"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

    public async Task SendQrCodeEmailAsync(string email, string qrCodeBase64)
    {
        var smtpSettings = _configuration.GetSection("Smtp");
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Admin", smtpSettings["adaboost6@gmail.com"]));
        message.To.Add(new MailboxAddress(string.Empty, email));
        message.Subject = "Your QR Code";

        message.Body = new TextPart("html")
        {
            Text = $"<p>Your QR code is below:</p><img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' />"
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(smtpSettings["Server"], int.Parse(smtpSettings["Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpSettings["UserName"], smtpSettings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
    public byte[] GenerateQrCode(string text)
    {
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.QrCode.QrCodeEncodingOptions
            {
                Height = 500,
                Width = 500,
                Margin = 1
            }
        };

        var pixelData = qrCodeWriter.Write(text);

        using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                             ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                                                            pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }

}