using ExemploCore3.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace WebApplication3.DAL
{
    public class Home
    {

        public static bool set(Acessa form)
        {
            try
            {
                var db = new dbcrudContext();
                if (form.Id > 0)
                {
                    var obj = db.Acessa.Where(c => c.Id == form.Id && c.DataExclusao == null).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.Email = form.Email;
                        obj.DataAlteracao = DateTime.Now;
                        obj.Nome = form.Nome;
                        obj.Role = form.Role;
                        obj.Chave = Guid.NewGuid().ToString();
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                    else
                        return false;


                }
                else
                {
                    var obj = new Acessa
                    {
                        Email = form.Email,
                        Ativo = "N",
                        DataInclusao = DateTime.Now,
                        Nome = form.Nome,
                        Role = form.Role,
                        Chave = Guid.NewGuid().ToString(),
                        Senha = CreateMD5(form.Senha)

                    };

                    db.Add(obj);
                    db.SaveChanges();
                    EmailConfirmacao(obj.Email);
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static IEnumerable<Acessa> GetAll()
        {
            try
            {
                var db = new dbcrudContext();
                var obj = db.Acessa.Where(c => c.DataExclusao == null).ToList();
                if (obj.Count > 0)
                    return obj;
                else
                    return null;

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public static bool Delet(int id)
        {
            try
            {
                var db = new dbcrudContext();
                var obj = db.Acessa.Where(c => c.Id == id && c.DataExclusao == null).FirstOrDefault();
                if (obj != null)
                {
                    obj.DataExclusao = DateTime.Now;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public static bool ForgotPassword(string email)
        {
            try
            {
                var db = new dbcrudContext();
                var obj = db.Acessa.Where(c => c.Email.ToUpper() == email.ToUpper() && c.DataExclusao == null).FirstOrDefault();
                if (obj != null)
                {
                    MailMessage messageUsuario = new MailMessage();
                    SmtpClient smtpClientUser = new SmtpClient("smtp01.hbsaude.com.br");
                    smtpClientUser.Port = 587;
                    smtpClientUser.Host = "smtp01.hbsaude.com.br";
                    //smtpClientUser.EnableSsl = true;
                    smtpClientUser.Credentials = (ICredentialsByHost)new NetworkCredential("no-reply", "noreply874598REPLY");
                    messageUsuario.From = new MailAddress("no-reply@hbsaude.com.br");
                    messageUsuario.To.Add(obj.Email);
                    messageUsuario.Subject = "Recuperar Senha - API TESTE LUCAS";

                    var html = "<h3>:: Alteração de senha TESTE API LUCAS JUAN ::</h3>";
                    html = html + "<h4>Prezado(a) " + obj.Nome + "</h4>";
                    html = html + "<p>Foi efetuada uma solicitação de <b>ALTERAÇÃO DE SENHA</b> em um de nossos sites. Para continuar clique no link abaixo</p>";
                    //html = html + "<a href='http://localhost:55303/Account/RecuperarSenha?key=" + usuario.chave_cadastro + "'>Clique aqui para redefinir a senha</a><br />";
                    html = html + "<a href='https://localhost:44336/Home/RecuperarSenha?to=" + obj.Chave + "'>Clique aqui para continuar com a solicitação</a><br />";
                    html = html + "<br /><br /><p><i>Se você recebeu este e-mail por engano, por favor desconsidere!</i></p>";
                    messageUsuario.Body = html;
                    messageUsuario.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    messageUsuario.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    messageUsuario.IsBodyHtml = true;
                    smtpClientUser.Send(messageUsuario);
                    return true;
                }
                else
                {
                    return false;
                }

            }

            catch (Exception)
            {
                return false;
            }


        }
        public static void EmailConfirmacao(string email)
        {
            try
            {
                var db = new dbcrudContext();
                var obj = db.Acessa.Where(c => c.Email.ToUpper() == email.ToUpper() && c.DataExclusao == null).FirstOrDefault();
                if (obj != null)
                {
                    MailMessage messageUsuario = new MailMessage();
                    SmtpClient smtpClientUser = new SmtpClient("smtp01.hbsaude.com.br");
                    smtpClientUser.Port = 587;
                    smtpClientUser.Host = "smtp01.hbsaude.com.br";
                    //smtpClientUser.EnableSsl = true;
                    smtpClientUser.Credentials = (ICredentialsByHost)new NetworkCredential("no-reply", "noreply874598REPLY");
                    messageUsuario.From = new MailAddress("no-reply@hbsaude.com.br");
                    messageUsuario.To.Add(obj.Email);
                    messageUsuario.Subject = "Confirmação de usuário - API TESTE LUCAS";

                    var html = "<h3>:: Confirmação de cadastro - TESTE API LUCAS JUAN ::</h3>";
                    html = html + "<h4>Prezado(a) " + obj.Nome + "</h4>";
                    html = html + "<p>Foi efetuada um <b>CADASTRO</b> com este e-mail em um de nossos sites. Para ativar o cadastro clique no link abaixo</p>";
                    //html = html + "<a href='http://localhost:55303/Account/RecuperarSenha?key=" + usuario.chave_cadastro + "'>Clique aqui para redefinir a senha</a><br />";
                    html = html + "<a href='https://localhost:44336/Home/ConfirmaCredentials?to=" + obj.Chave + "'>Clique aqui para continuar com a solicitação</a><br />";
                    html = html + "<br /><br /><p><i>Se você recebeu este e-mail por engano, por favor desconsidere!</i></p>";
                    messageUsuario.Body = html;
                    messageUsuario.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    messageUsuario.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    messageUsuario.IsBodyHtml = true;
                    smtpClientUser.Send(messageUsuario);

                }


            }

            catch (Exception)
            {
                throw;
            }
        }
        public static Acessa BuscaEditar(int codigo)
        {
            var db = new dbcrudContext();
            var obj = db.Acessa.Where(c => c.Id == codigo && c.DataExclusao == null).FirstOrDefault();
            return obj;
        }
        public static ExemploCore3.Models.Acessa RenewPassword(Acessa form)
        {
            try
            {
                var db = new dbcrudContext();
                var obj = db.Acessa.Where(c => c.Email == form.Email && c.Chave == form.Chave && c.DataExclusao == null).FirstOrDefault();
                if (obj == null)
                {
                    return null;
                }
                // Gravar nova senha dando update na tabela
                obj.Senha = DAL.Home.CreateMD5(form.Senha);
                obj.DataAlteracao = DateTime.Now;
                obj.Chave = Guid.NewGuid().ToString();

                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string CreateMD5(string s)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var encoding = Encoding.ASCII;
                var data = encoding.GetBytes(s);

                Span<byte> hashBytes = stackalloc byte[16];
                md5.TryComputeHash(data, hashBytes, out int written);
                if (written != hashBytes.Length)
                    throw new OverflowException();


                Span<char> stringBuffer = stackalloc char[32];
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashBytes[i].TryFormat(stringBuffer.Slice(2 * i), out _, "x2");
                }
                return new string(stringBuffer);
            }
        }
        public static Acessa validaUsuario(Acessa model)
        {
            var db = new dbcrudContext();
            var nSenha = DAL.Home.CreateMD5(model.Senha.ToString());
            var user = db.Acessa.Where(c => c.Email == model.Email && c.Senha == nSenha && c.DataExclusao == null).FirstOrDefault();

            return user;
        }
        public static bool validaUsuarioAtivo(Acessa model)
        {
            var db = new dbcrudContext();
            var nSenha = DAL.Home.CreateMD5(model.Senha.ToString());
            var user = db.Acessa.Where(c => c.Email == model.Email && c.Senha == nSenha && c.DataExclusao == null && c.Ativo != "N").FirstOrDefault();

            if (user != null)
                return true;
            else
                return false;
        }
        public static Acessa ConfirmCredentials(Acessa form)
        {
            try
            {
                var db = new dbcrudContext();
                var obj = db.Acessa.Where(c => c.Email == form.Email && c.Chave == form.Chave && c.DataExclusao == null && c.Ativo == "N").FirstOrDefault();
                if (obj == null)
                {
                    return null;
                }
                // Gravar nova chafe
                obj.Ativo = "S";
                obj.DataAlteracao = DateTime.Now;
                obj.Chave = Guid.NewGuid().ToString();

                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool VerificaChaveAtiva(Acessa form)
        {
            var db = new dbcrudContext();
            var obj = db.Acessa.Where(c => c.Chave == form.Chave && c.DataExclusao == null).FirstOrDefault();
            if (obj != null)
                return true;
            else
                return false;


        }


    }
}
