using BlogProject.BLL.Abstract;
using BlogProject.BLL.Results;
using BlogProject.Common.Helpers;
using BlogProject.DAL.EntityFramework;
using BlogProject.Entities;
using BlogProject.Entities.Messages;
using BlogProject.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogProject.BLL
{
    public class UserManager:ManagerBase<User>
    {
        
        public BusinessLayerResult<User> RegisterUser(RegisterViewModel data)
        {
            // Kullanıcı username kontrolü
            // Kullanıcı e-mail kontrolü
            // Kayıt işlemi..
            // Aktivasyon emaili gönderimi

            User user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<User> res = new BusinessLayerResult<User>();

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kayıtlı kullanıcı adı");
                    
                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "Kayıtlı email adresi");                   
                }
            }
            else
            {
                int dbResult = base.Insert(new User()
                {
                    Username=data.Username,
                    Email=data.Email,
                    Password=data.Password,
                    ActivateGuid=Guid.NewGuid(),   
                    ProfileImageFileName="user.png",
                    IsActive=false,
                    IsAdmin=false
                });

                if (dbResult > 0)
                {
                    res.Result = Find(x => x.Email == data.Email && x.Username == data.Username);

                    string siteUrl = ConfigHelper.Get<string>("SiteRootUrl");
                    string activaUrl = $"{siteUrl}Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Username};<br><br>Hesabınızı aktifleştirmek için <a href='{activaUrl}' target='_blank'>tıklayınız</a>";

                    MailHelper.SendMail(body, res.Result.Email, "Blog Project Hesap Aktifleştirme");
                }
            }
            return res;
        }

        public BusinessLayerResult<User> LoginUser(LoginViewModel data)
        {
            //Giriş Kontrolü
            //Hesap aktive edilmiş mi?

            BusinessLayerResult<User> res = new BusinessLayerResult<User>();
            res.Result = Find(x => x.Username == data.Username && x.Password == data.Password);

            if (res.Result != null)
            {
                if (!res.Result.IsActive) //res.Resut.IsActive==false
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessageCode.CheckYourEail, "Lütfen Email adresinizi kontrol ediniz.");
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı adı veya şifre hatası.");
            }

            return res;
       

        }

        public BusinessLayerResult<User> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<User> res = new BusinessLayerResult<User>();
            res.Result = Find(x => x.ActivateGuid == activateId);

            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActivate, "Kullanıcı zaten aktif edilmiştir.");
                    return res;
                }

                res.Result.IsActive = true;
                Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessageCode.ActivateIdDoesNotExists, "Aktifleştirilecek kullanıcı bulunamadı");
            }

            return res;
        }

        public BusinessLayerResult<User> GetUserById(int? id)
        {
            BusinessLayerResult<User> res = new BusinessLayerResult<User>();
            if (id == null)
            {
                res.AddError(ErrorMessageCode.UserIsNotFound, "Kullanıcı bulunamadı");                
            }
            else
            {
                res.Result = Find(x => x.Id == id);

                if (res.Result == null)
                {
                    res.AddError(ErrorMessageCode.UserIsNotFound, "Kullanıcı bulunamadı");
                }
            }
           
            return res;

        }

        public BusinessLayerResult<User> UpdateProfile(User data)
        {
            User db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));

            BusinessLayerResult<User> res = new BusinessLayerResult<User>();

            if(db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "Email adresi kayıtlı");
                }

                return res;
            }

            res.Result = Find(x => x.Id==data.Id);
            res.Result.Email = data.Email;
            res.Result.Username = data.Username;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;

            if(string.IsNullOrEmpty(data.ProfileImageFileName) == false)
            {
                res.Result.ProfileImageFileName=data.ProfileImageFileName;
            }
            if (base.Update(res.Result)==0)
            {
                res.AddError(ErrorMessageCode.ProfileCouldNotUpdated, "Profil güncellenemedi.");
            }

            return res;
        }

        public BusinessLayerResult<User> RemoveUserById(int id)
        {
            BusinessLayerResult<User> res = new BusinessLayerResult<User>();
            User user = Find(x => x.Id == id);

            if(user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi.");
                    return res;
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UserCouldNotFind, "Kullanıcı bulunamadı.");
            }

            return res;
        }

        public new BusinessLayerResult<User> Insert(User data)
        {
            User user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<User> res = new BusinessLayerResult<User>();

            res.Result = data;
            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kayıtlı kullanıcı adı");

                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "Kayıtlı email adresi");
                }
            }
            else
            {
                res.Result.ProfileImageFileName = "user.png";
                res.Result.ActivateGuid = Guid.NewGuid();


                if (base.Insert(res.Result) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı Eklenemedi.");
                }
            }
            return res;
        }

        public new BusinessLayerResult<User> Update(User data)
        {

            User db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));

            BusinessLayerResult<User> res = new BusinessLayerResult<User>();

            res.Result = data;

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "Email adresi kayıtlı");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Username = data.Username;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;

         
            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.ProfileCouldNotUpdated, "Profil güncellenemedi.");
            }

            return res;
        }
    }
}
