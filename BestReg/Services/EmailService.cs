using MimeKit;
using MailKit.Net.Smtp;
using System.IO;
using System.Text.Encodings.Web;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Extensions.Logging;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string callbackUrl);
    Task SendQrCodeEmailAsync(string email, string qrCodeBase64);
    byte[] GenerateQrCode(string text);
    byte[] GenerateBarcode(string text);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendConfirmationEmailAsync(string email, string callbackUrl)
    {
        var smtpSettings = _configuration.GetSection("Smtp");
        string smtpServer = smtpSettings["Server"];
        string portString = smtpSettings["Port"];
        string userName = smtpSettings["UserName"];
        string password = smtpSettings["Password"];

        if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(portString) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            _logger.LogError("SMTP settings are not properly configured.");
            throw new ArgumentNullException("SMTP settings are not properly configured.");
        }

        if (!int.TryParse(portString, out int port))
        {
            _logger.LogError("Invalid SMTP port number.");
            throw new ArgumentException("Invalid SMTP port number.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Admin", userName));
        message.To.Add(new MailboxAddress(string.Empty, email));
        message.Subject = "Confirm your email";

        message.Body = new TextPart("html")
        {
            Text = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
        };

        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(userName, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the confirmation email.");
            throw;
        }
    }

    public async Task SendQrCodeEmailAsync(string email, string qrCodeBase64)
    {
        var smtpSettings = _configuration.GetSection("Smtp");
        string smtpServer = smtpSettings["Server"];
        string portString = smtpSettings["Port"];
        string userName = smtpSettings["UserName"];
        string password = smtpSettings["Password"];

        if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(portString) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            _logger.LogError("SMTP settings are not properly configured.");
            throw new ArgumentNullException("SMTP settings are not properly configured.");
        }

        if (!int.TryParse(portString, out int port))
        {
            _logger.LogError("Invalid SMTP port number.");
            throw new ArgumentException("Invalid SMTP port number.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Admin", userName));
        message.To.Add(new MailboxAddress(string.Empty, email));
        message.Subject = "Your QR Code";

        message.Body = new TextPart("html")
        {
            Text = $"<p>Your QR code is below:</p><img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' />"
        };

        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(userName, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the QR code email.");
            throw;
        }
    }


    // QR Code generation remains the same
    public byte[] GenerateQrCode(string text)
    {
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
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

    public byte[] GenerateBarcode(string text)
    {
        var barcodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 150,
                Width = 300,
                Margin = 10
            }
        };

        var pixelData = barcodeWriter.Write(text);
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
