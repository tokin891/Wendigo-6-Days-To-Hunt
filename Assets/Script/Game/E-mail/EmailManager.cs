using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class EmailManager : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] TMP_InputField bodyMessage;
    [SerializeField] TMP_InputField fromEmail;
    [SerializeField] TMP_InputField toEmail;

    private bool isSend = false;

    public void SendMessageEmail()
    {
        if (isSend != true && fromEmail.text.Length != 0 && bodyMessage.text.Length != 0)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Timeout = 10000;
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Port = 587;

            mail.From = new MailAddress("nekotan5454@gmail.com");
            mail.To.Add(new MailAddress(toEmail.text));

            mail.Subject = "Wendigo6DTH_Opinion_From " + fromEmail.text;
            mail.Body = bodyMessage.text;

            SmtpServer.Credentials = new System.Net.NetworkCredential("writtetowendigo6@gmail.com", "ywmoujvwdptgiqqh") as ICredentialsByHost; SmtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            SmtpServer.Send(mail);

            isSend = true;
        }
    }
    public void ExitG()
    {
        Application.Quit();
    }
}
