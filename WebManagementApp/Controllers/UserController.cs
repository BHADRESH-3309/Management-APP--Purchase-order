using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebManagementApp.DataAccess.Interfaces;
using WebManagementApp.Models;

namespace WebManagementApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _config;
        IUserService _userService;
        ITokenService _tokenService;
        public UserController(IUserService userService,ITokenService tokenService,IConfiguration config) 
        { 
            _userService = userService;
            _tokenService = tokenService;
            _config = config;
        }
        [HttpGet]
        public IActionResult Login()
        {
            string token = HttpContext.Session.GetString("Token");

            // If token is not empty and it's valid then redirect on Index page.
            if (!string.IsNullOrEmpty(token))
            {
                if (_tokenService.IsTokenValid(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), token))
                {
                    return RedirectToAction("Index", "Dashboard");
                    //return Redirect("~/MasterSKU/Index");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(UserVM userModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check password length 

                    // Validate user
                    var user = await _userService.GetUser(userModel);
                    if (user == null)
                    {
                        ViewBag.ErrorMessage = "User email address does not exist!";
                        return View();
                    }
                    //Password check
                    string dbHashPassword = user.Password.ToString();
                    string userInputPasswordHash = _userService.GeneratePasswordHash(userModel.Password);
                    if (dbHashPassword != userInputPasswordHash)
                    {
                        ViewBag.ErrorMessage = "Incorrect password!";
                        return View();
                    }
                    
                    string generatedToken = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), user);
                    if (string.IsNullOrEmpty(generatedToken))
                    {
                        ViewBag.ErrorMessage = "Something went wrong!";
                        return View();
                    }

                    //Update lastactivity date
                    _userService.UpdateLastAcivityDate(userModel.Email);

                    HttpContext.Session.SetString("Token", generatedToken);
                    HttpContext.Session.SetString("Email",user.Username);
                    HttpContext.Session.SetString("Name", user.Name);
                    HttpContext.Session.SetString("UserRole", user.UserRole);
                    return RedirectToAction("Index", "Dashboard");
                    //return Redirect("~/MasterSKU/Index");
                }
            }
            catch(Exception ex) 
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View();
        }
        // GET : User/Logout
        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
