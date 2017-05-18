using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace WeddingPlanner.Controllers
{
public class AccountController : Controller
    {
        private BankContext _context;
        private readonly IHostingEnvironment _env;
        private readonly MSApiKeyOption _msoptions;
        public AccountController(BankContext context,IHostingEnvironment env,IOptions<MSApiKeyOption> optionsAccessor)
        {
            _context = context;
            _env = env;
            _msoptions = optionsAccessor.Value;
        }
        // GET: /Record list
        [HttpGet]
        [Route("/account/{userid}")]
        public IActionResult Records(int userid)
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
                    .Include(user => user.Activities)
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
                curuser.Activities=curuser.Activities.OrderByDescending(a =>a.CreatedAt).ToList();
                float balance=0;
                for(int i=0;i<curuser.Activities.Count;i++)
                {
                    balance += curuser.Activities[i].Amount;
                }
                ViewData["balance"]=balance;
                if(TempData["error"]!=null)
                {
                    ViewData["error"]=TempData["error"];
                }
            return View("Records",curuser);
            }            
        }
        
        // Post : deposit or withdraw
        [HttpPost]
        [Route("/newactivity")]
        public IActionResult DepositorWithdraw(float amount)
        {
           int? Uid = HttpContext.Session.GetInt32("UserId");
            if(Uid==null)
            {
                return RedirectToAction("Index","User");
            }
            else
            {
                int userid=(int)Uid;
                if(amount==0)
                {
                    TempData["error"]="Please input a number";
                    return RedirectToAction("Records",new {userid=userid});
                }
                else
                {
                    User curuser = _context.users
                        .Include(user => user.Activities)
                        .SingleOrDefault(user => user.UserId==userid);
                    float balance=0;
                    for(int i=0;i<curuser.Activities.Count;i++)
                    {
                        balance += curuser.Activities[i].Amount;
                    }
                    if(balance+amount<0)
                    {
                        TempData["error"]="You could not withdraw more money than your balance";
                        return RedirectToAction("Records",new {userid=userid});                      
                    }
                    else
                    {
                        Activity newactivity=new Activity();
                        newactivity.Amount=amount;
                        newactivity.UserId=(int)Uid;
                        newactivity.CreatedAt=DateTime.Now;
                        _context.Add(newactivity);
                        _context.SaveChanges();
                        return RedirectToAction("Records",new {userid=userid});  
                    }
                }
            }
        }
    } 
}
