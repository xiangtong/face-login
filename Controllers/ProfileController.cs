using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FaceLogin.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace FaceLogin.Controllers
{
public class ProfileController : Controller
    {
        private UserContext _context;
        private readonly IHostingEnvironment _env;
        private readonly MSApiKeyOption _msoptions;
        public ProfileController(UserContext context,IHostingEnvironment env,IOptions<MSApiKeyOption> optionsAccessor)
        {
            _context = context;
            _env = env;
            _msoptions = optionsAccessor.Value;
        }
        // GET: /Record list
        [HttpGet]
        [Route("/account/{userid}")]
        public IActionResult Profilepage(int userid)
        {
           int? Uid = HttpContext.Session.GetInt32("UserId");
           ViewData["ifidentical"] = HttpContext.Session.GetString("isIdentical");
           ViewData["confidence"] = HttpContext.Session.GetString("confidence");

            if(Uid==null)
            {
                return RedirectToAction("Index","User");
            }
            else
            {
                // int userid=(int)Uid;
                User curuser = _context.users
                    .SingleOrDefault(user => user.UserId==userid);
                if(curuser.RegImgUrl!=null)
                {
                    var filename= curuser.RegImgUrl;
                    var webRoot = _env.WebRootPath;
                    var file = System.IO.Path.Combine(webRoot, filename);
                    Byte[] imgcontent = System.IO.File.ReadAllBytes(file);
                    string base64string = Convert.ToBase64String(imgcontent, 0, imgcontent.Length);
                    string imageurl = @"data:image/png;base64," + base64string;
                    ViewBag.imageurl=imageurl;
                }
            return View("Profile",curuser);
            }            
        }

    } 
}
