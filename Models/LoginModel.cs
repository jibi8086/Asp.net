using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace NuNetwork.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


    }

    public class ForgetModel
    {
        [Required(ErrorMessage = "Please enter your email address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }
    }


    public class Crypto
    {

        public static string Encryption(string clearText)
        {
            MemoryStream ms = null;
            CryptoStream cs = null;
            string EncryptionKey = "MAKV2SPBNI99212";
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            try

            {


                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

                using (Aes encryptor = Aes.Create())

                {


                    encryptor.Key = pdb.GetBytes(32);

                    encryptor.IV = pdb.GetBytes(16);

                    using (ms = new MemoryStream())

                    {

                        using (cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))

                        {

                            cs.Write(clearBytes, 0, clearBytes.Length);

                            cs.Close();

                        }

                        clearText = Convert.ToBase64String(ms.ToArray());

                    }

                }

                return clearText;

            }

            catch (Exception e)
            {
                ExceptionLog.Log(e, "no ip : login model error encryption \n ");
            }
            finally
            {
                pdb.Dispose();
                cs.Dispose();
                ms.Dispose();
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            MemoryStream ms = null;
            CryptoStream cs = null;
            string EncryptionKey = "MAKV2SPBNI99212";
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            try

            {

              

                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes encryptor = Aes.Create())

                {

                   
                    encryptor.Key = pdb.GetBytes(32);

                    encryptor.IV = pdb.GetBytes(16);

                    using (ms = new MemoryStream())

                    {

                        using (cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))

                        {

                            cs.Write(cipherBytes, 0, cipherBytes.Length);

                            cs.Close();

                        }

                        cipherText = Encoding.Unicode.GetString(ms.ToArray());

                    }

                }

                return cipherText;

            }

            catch (Exception e)
            {
                ExceptionLog.Log(e, "no ip :login model error decryption ");
            }
            finally
            {
                pdb.Dispose();
                ms.Dispose();
                cs.Dispose();
             
            }
            return cipherText;
        }





    }
    public class Mail
    {
        public static string SendMail(string mail,string subject,string body)
        {
            System.Net.Mail.SmtpClient smtp;
            string NotifyMessage="Mail not Send";
            MailAddress fromAddress = new MailAddress("devd9216@gmail.com");
            string fromPassword = "devd@9216";
            MailAddress toAddress = new MailAddress(mail);



            smtp = new System.Net.Mail.SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)

            };
            try
            {
         

                using (MailMessage message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                    smtp.Send(message);
                 NotifyMessage = "Mail has been send";
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, "no ip : login model error email ");
            }

            finally
            {
                smtp.Dispose();
            }

            return NotifyMessage;

        }

    }

 




}