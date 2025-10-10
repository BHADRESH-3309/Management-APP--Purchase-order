                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
using WebManagementApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebManagementApp.DataAccess.Interfaces;

namespace WebManagementApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class ManageUserController : Controller
    {
        private readonly IManageUserService _manageUserService;

        public ManageUserController(IManageUserService manageUserService)
        {
            _manageUserService = manageUserService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetManageUserData()
        {
            ResponseModel result = new ResponseModel();
            try
            {
                var response = await _manageUserService.GetManageUserData();
                return new JsonResult(new { Error = false, Result = response });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Error = true, Message = ex.Message });
            }
        }

        // Add User
        [HttpPost]
        public async Task<JsonResult> AddUser(string name, string email, string password)
        {
            try
            {
                string message = string.Empty;
                string idUser = string.Empty;

                bool isExistingEmail = _manageUserService.IsExistingEmail(email, idUser);
                if (isExistingEmail)
                {
                    message = "Email already exists";
                    return new JsonResult(new { Error = true, Message = message });
                }

                await _manageUserService.AddUser(name, email, password);
                message = "New user added successfully.";

                var response = await _manageUserService.GetManageUserData();

                return new JsonResult(new { Error = false, Message = message, Result = response });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Error = true, Message = ex.Message });
            }
        }

        //Update User
        [HttpPost] 
        public async Task<JsonResult> UpdateUser(string name, string email, string password, string idUser )
        {
            try
            {
                string message = string.Empty;

                bool isExistingEmail = _manageUserService.IsExistingEmail(email, idUser);
                if (isExistingEmail)
                {
                    message = "Email already exists";
                    return new JsonResult(new { Error = true, Message = message });
                }

                await _manageUserService.UpdateUser(name, email, password, idUser);
                message = "User email and password updated successfully.";

                var response = await _manageUserService.GetManageUserData();

                return new JsonResult(new { Error = false, Message = message, Result = response });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Error = true, Message = ex.Message });
            }
        }

        //Delete User
        [HttpPost]
        public async Task<JsonResult> DeleteUser(string idUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(idUser))
                {
                    await _manageUserService.DeleteUser(idUser);
                }
                var response = await _manageUserService.GetManageUserData();

                return new JsonResult(new { Error = false, Message = "User deleted successfully.", Result = response });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { Error = true, Message = ex.Message });
            }
        }
    }
}
