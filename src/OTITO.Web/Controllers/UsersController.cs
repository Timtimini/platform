using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;



using OTITO_Services;
using OTITO_Services.Model;

using OTITO.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OTITO.Web.Models.Authentication;
using OTITO.Web.Models.User;
using Microsoft.Extensions.Options;
using reCAPTCHA.AspNetCore;
using System.IO;
using System.Text;
using OTITO.Web.Models.Enum;
using Microsoft.AspNetCore.Authentication.Cookies;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Auth;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OTITO.Web.Controllers
{
    public class UsersController : Controller
    {
        private IUserService _userService;

        private IRecaptchaService _recaptcha;


        public UsersController(IUserService userService, IRecaptchaService recaptcha)
        {
            _userService = userService;
            _recaptcha = recaptcha;
        }
        [HttpGet]
        public IActionResult Login()
        {
            BasicUser _user = new BasicUser();
            ViewBag.Title = meta.title.Value.ToString();
            //"òtító | a platform for documenting and sharing truth";
            ViewBag.Description = meta.description.Value.ToString();
            //"Society's most democratic tool for documenting and sharing objectively verifiable political and socio-cultural truth";
            return View(_user);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(BasicUser userParam)
        {

            if (ModelState.IsValid)
            {
                var user = _userService.Authenticate(userParam.Email, userParam.Password);

                if (user != null)
                {
                    var claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim(ClaimTypes.Sid, user.Id.ToString()),
                        new System.Security.Claims.Claim(ClaimTypes.Email, user.Email),
                        new System.Security.Claims.Claim(ClaimTypes.Role, user.Role??"")

                    };
                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                    System.Security.Claims.ClaimsPrincipal principal = new System.Security.Claims.ClaimsPrincipal(userIdentity);

                    HttpContext.SignInAsync(principal);
                    return Redirect("/");
                }
                else
                {
                    TempData["ErrorMessage"] = "Wrong credentials.";


                }
            }
            ViewBag.Title = meta.title.Value.ToString();// "òtító | a platform for documenting and sharing truth";
            ViewBag.Description = meta.description.Value.ToString(); //"Society's most democratic tool for documenting and sharing objectively verifiable political and socio-cultural truth";
            return View(userParam);




        }
        //[AllowAnonymous]
        //public IActionResult Login()
        //{
        //    BasicUser user = new BasicUser();
        //    return View(user);
        //}
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        [HttpGet]
        public IActionResult Signup()
        {
            UserSignup user = new UserSignup();
            user.ValidCaptcha = true;
            ViewBag.Title = meta.title.Value.ToString();

            //"òtító | a platform for documenting and sharing truth";
            ViewBag.Description = meta.description.Value.ToString();
            
                //"Society's most democratic tool for documenting and sharing objectively verifiable political and socio-cultural truth";
            return View(user);
        }

        public JsonResult IfEmailExist(string Email)
        { 
            try
            {
                bool exists = _userService.IfEmailExist(Email);
                return Json(new { success = true, exists });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, exists=false });
            }
        }
        [HttpPost]
        public async Task<IActionResult> LoginWithFacebook(FacebookSignup _signup)
        {

            try
            {
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(_signup.uid);
                var UserEmail = userRecord.Email;
                var firstName = userRecord.DisplayName.Split(" ")[0];
                var lastName = userRecord.DisplayName.Split(" ")[1];

                var user = _userService.AuthenticateSocial(UserEmail, firstName, lastName);

                if (user != null)
                {
                    var claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim(ClaimTypes.Sid, user.Id.ToString()),
                        new System.Security.Claims.Claim(ClaimTypes.Email, user.Email),
                        new System.Security.Claims.Claim(ClaimTypes.Role, user.Role??"")

                    };
                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                    System.Security.Claims.ClaimsPrincipal principal = new System.Security.Claims.ClaimsPrincipal(userIdentity);

                    await HttpContext.SignInAsync(principal);
                    return Redirect("/");
                }
                else
                {
                    TempData["ErrorMessage"] = "Wrong credentials.";
                    return Redirect("/Users/Login");


                }
            }
            catch (Exception ex)
            {
                return Redirect("/Users/Login");
            }
            //
            //// See the UserRecord reference doc for the contents of userRecord.
            //Console.WriteLine($"Successfully fetched user data: {userRecord.Uid}");

            //return View();
        }

        [HttpPost]

        public async Task<IActionResult> Signup(UserSignup _user)
        {

            var recaptcha = await _recaptcha.Validate(Request);

            if (ModelState.IsValid && recaptcha.success)
            {

                try
                {
                    int a = _userService.SaveUser(_user.Email, _user.Password);
                    if (a == -1)
                    {
                        TempData["ErrorMessage"] = "User already exists";
                        return View(_user);
                    }
                    else
                    {
                        // do the login process;

                        var claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim(ClaimTypes.Sid, a.ToString()),
                        new System.Security.Claims.Claim(ClaimTypes.Email, _user.Email),
                        new System.Security.Claims.Claim(ClaimTypes.Role, "")
                    };
                        ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                        System.Security.Claims.ClaimsPrincipal principal = new System.Security.Claims.ClaimsPrincipal(userIdentity);

                        await HttpContext.SignInAsync(principal);
                        return Redirect("/");

                    }
                }
                catch
                {
                    ViewBag.Title = meta.title.Value.ToString();
                    ViewBag.Description = meta.description.Value.ToString();
                    TempData["ErrorMessage"] = "An unknown error occured!";
                    return View(_user);
                }

            }
            else
            {
                ViewBag.Title = meta.title.Value.ToString();
                ViewBag.Description = meta.description.Value.ToString();
                _user.ValidCaptcha = recaptcha.success;
                return View(_user);
            }
        }



        [HttpGet]
        public IActionResult Signup_V2()
        {
            UserSignup user = new UserSignup();
            return View(user);
        }
        [HttpPost]
        [ValidateGoogleCaptcha]
        public IActionResult Signup_V2(UserSignup _user)
        {
            var recaptcha = _recaptcha.Validate(Request);

            if (ModelState.IsValid && recaptcha.IsCompletedSuccessfully)
            {

                try
                {
                    int a = _userService.SaveUser(_user.Email, _user.Password);
                    if (a == -1)
                    {
                        TempData["ErrorMessage"] = "User already exists";
                        return View(_user);
                    }
                    else
                    {
                        // do the login process;

                        var claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim(ClaimTypes.Sid, a.ToString()),
                        new System.Security.Claims.Claim(ClaimTypes.Email, _user.Email),
                        new System.Security.Claims.Claim(ClaimTypes.Role, "")
                    };
                        ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                        System.Security.Claims.ClaimsPrincipal principal = new System.Security.Claims.ClaimsPrincipal(userIdentity);

                        HttpContext.SignInAsync(principal);
                        return Redirect("/");

                    }
                }
                catch
                {
                    TempData["ErrorMessage"] = "An unknown error occured!";
                    return View(_user);
                }

            }
            else
            {
                ViewBag.Title = meta.title.Value.ToString();
                ViewBag.Description = meta.description.Value.ToString();
                return View(_user);
            }
        }



        [Authorize]
        public IActionResult Profile()
        {
            UserProfile _user = new UserProfile();
            try
            {
                var UserId = User.FindFirst(ClaimTypes.Sid).Value;
                var user = _userService.GetProfile(Convert.ToInt32(UserId));
                _user.Id = user.Id;
                _user.IsSocial = user.IsSocial;
                _user.Name = user.Name;
                double positive = user.TotalVote == 0 ? 0 : (double)user.PositiveKarma / (double)(user.TotalVote);
                double negative = user.TotalVote == 0 ? 0 : (double)user.NegativeKarma / (double)(user.TotalVote);
                _user.PositiveKarma = positive * 100;
                _user.NegativeKarma = negative * 100;

                _user.Activity = user.activity.Select(x => new UserProfileActivity
                {
                    ClaimId = x.ClaimId,
                    Title = x.activity_type+" "+x.activity_title,
                    TopicId = x.TopicId,

                    URL = x.ClaimId == 0 ? "/topic/counter/" + x.TopicSlug : "/topic/sources/" + x.ClaimSlug,
                }).ToList();
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occured while fetching details";
            }
            ViewBag.Title = meta.title.Value.ToString();
            ViewBag.Description = meta.description.Value.ToString();
            return View(_user);
        }
        [Authorize]
        public JsonResult ChangePassword([FromBody] ChangePassword _password)
        {
            var UserId = User.FindFirst(ClaimTypes.Sid).Value;
            try
            {
                int result = _userService.ChangePassword(_password.PreviousPassword, _password.NewPassword, Convert.ToInt32(UserId));
                if (result == 1)
                    return Json(new { success = true, message = "Your password has been changed successfully." });
                else
                    return Json(new { success = false, message = "Old Password seems to be wrong. Please use correct password." });
            }
            catch
            { }

            return Json(new { success = false, message = "Unable to change your password due to a server error. Please check back later." });

        }
        public IActionResult ChangePasswordModal()
        {
            return PartialView();
        }

        [Authorize]
        public IActionResult Statistics(UserStatisticsModel model)
        {

            if (ModelState.IsValid)
            { 
                try
                {
                    var Role = User.FindFirst(ClaimTypes.Role).Value;

                    if (Role.ToString().ToLower().Equals("admin"))
                    {
                        var result = _userService.GetStatistics(model.formData.DateFrom, model.formData.DateTo);
                        model.NoOfClaims = result.NoOfClaims;
                        model.NoOfCounterClaims = result.NoOfCounterClaims;
                        model.NoOfSources = result.NoOfSources;
                        model.NoOfTopics = result.NoOfTopics;
                        model.NoOfUsers = result.NoOfUsers;
                        model.NoOfVotes = result.NoOfVotes;
                        model.hasData = true;
                    }
                        

                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occured while fetching your result. Please contact admin.";
                }

            }

            return View(model);
        }
        [Authorize]
        public IActionResult UserList()
        {
            try
            {
                var result = _userService.UserList();
                var stream = new MemoryStream();
                var writeFile = new StreamWriter(stream);
                CsvWriter csv = new CsvWriter();

                var Users = (from user in result
                                       select new object[]
                                       {
                                            user
                                       }).ToList();

                // Build the file content
                var UserCSV = new StringBuilder();
                Users.ForEach(line =>
                {
                    UserCSV.AppendLine(string.Join(",", line));
                });

                byte[] buffer = Encoding.ASCII.GetBytes($"{UserCSV.ToString()}");
                return File(buffer, "text/csv", $"Users.csv");

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public IActionResult testProfile()
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email).Value;
            var firstName = User.Claims.FirstOrDefault(x => x.Type.Contains("givenname"));
            var lastName = User.Claims.FirstOrDefault(x => x.Type.Contains("surname"));
            HttpContext.SignOutAsync();
            try
            {
                var user = _userService.AuthenticateSocial(UserEmail, firstName.Value,lastName.Value);

                if (user != null)
                {
                    var claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim(ClaimTypes.Sid, user.Id.ToString()),
                        new System.Security.Claims.Claim(ClaimTypes.Email, user.Email),
                        new System.Security.Claims.Claim(ClaimTypes.Role, user.Role??"")

                    };
                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                    System.Security.Claims.ClaimsPrincipal principal = new System.Security.Claims.ClaimsPrincipal(userIdentity);

                    HttpContext.SignInAsync(principal);
                    return Redirect("/");
                }
                else
                {
                    TempData["ErrorMessage"] = "Wrong credentials.";
                    return Redirect("/Users/Login");


                }
            }
            catch(Exception ex)
            {
                return Redirect("/Users/Login");
            }

            //var vm = new ProfileViewModel
            //{
            //    Claims = User.Claims,
            //    Name = User.Identity.Name
            //};
            //return View(vm);
        }



        [Route("signin")]
        public IActionResult SignIn() => View();

        [Route("signin/{provider}")]
        public IActionResult SignIn(string provider, string returnUrl = null) =>
            Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/Users/testProfile" }, provider);

        [Route("signout")]
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
