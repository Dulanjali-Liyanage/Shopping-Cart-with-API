using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using ShoppingCartDemo.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    public class AuthenticateController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel logmuser)
        {
            HttpResponseMessage response = GlobalVariables.webApiClient.PostAsJsonAsync("Authenticate/login", logmuser).Result;
            
            //read the response to a model
            TokenResponse tokenResponse = response.Content.ReadAsAsync<TokenResponse>().Result;
            
            //set the authorization header
            GlobalVariables.webApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.token);

            //decode the token an get the username and the role
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(tokenResponse.token);
            var tokenS = jsonToken as JwtSecurityToken;

            var username = tokenS.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value;
            var role = tokenS.Claims.First(claim => claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;

            LoggedUser.UserName = username;
            LoggedUser.UserRole = role;

            Debug.WriteLine(username);
            return RedirectToAction("Index","Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel reguser)
        {
            HttpResponseMessage response = GlobalVariables.webApiClient.PostAsJsonAsync("Authenticate/register", reguser).Result;
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            SessionHelper.SetObjectAsJson(HttpContext.Session, "Items", null);
            GlobalVariables.webApiClient.DefaultRequestHeaders.Authorization = null;
            LoggedUser.UserName = null;
            LoggedUser.UserRole = null;
            return RedirectToAction("Index","Home");
        }
    }
}
