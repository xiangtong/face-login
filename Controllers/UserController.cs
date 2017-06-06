using System;
using System.Collections.Generic;
// using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FaceLogin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
// using FaceLogin;

namespace FaceLogin.Controllers
{
 public class UserController : Controller
    {
        private UserContext _context;
        private readonly IHostingEnvironment _env;
        private readonly MSApiKeyOption _msoptions;
        public UserController(UserContext context,IHostingEnvironment env,IOptions<MSApiKeyOption> optionsAccessor)
        {
            _context = context;
            _env = env;
            _msoptions = optionsAccessor.Value;

        }
        // GET: /Login/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
           int? Uid = HttpContext.Session.GetInt32("UserId");
            if(Uid!=null){
                int userid=(int)Uid;
                return RedirectToAction("Profilepage","Profile",new {userid=userid});
            }
            else
            {
                string uid=Request.Cookies["uid"];
                if(uid!=null)
                {
                    int userid = Int32.Parse(uid);
                    User user =_context.users 
                                .SingleOrDefault(u=>u.UserId==userid);
                    if(user!=null && user.RegImgUrl!=null && user.RegImgUrl!="")
                    {
                        ViewBag.ifhasimage=true;
                    }
                    ViewBag.user=user;  
                }
   
                return View();
            }            
        }
        
        // Post : Register
        [HttpPost]
        [Route("/register")]
        public IActionResult Register(RegisterViewModel regmodel)
        {
            if(ModelState.IsValid)
            {
                List<User> all=_context.users.ToList();
                List<User> users=_context.users.Where(user => user.Email==regmodel.Email).ToList();
                if(users.Count!=0)
                {
                    ViewBag.emailexist="Email has been registered!";
                    return View("Index");
                }
                else
                {
                    User newuser = new User
                    {
                        FirstName = regmodel.FirstName,
                        LastName = regmodel.LastName,
                        Password = regmodel.Password,
                        Email = regmodel.Email,
                        CreatedAt = DateTime.Now
                    };
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newuser.Password = Hasher.HashPassword(newuser, newuser.Password);
                    _context.Add(newuser);
                    _context.SaveChanges();
                    User curuser= _context.users.SingleOrDefault(user => user.Email == newuser.Email);              
                    HttpContext.Session.SetInt32("UserId", curuser.UserId);
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(3650);
                    Response.Cookies.Append("uid", curuser.UserId.ToString(),options);
                    return RedirectToAction("Profilepage","Profile",new {userid=curuser.UserId});
                }

            }
            return View("Index");
        }

        // Post: Login
        [HttpPost]
        [Route("/login")]
        public IActionResult Login(LoginViewModel logmodel)
        {
            if(ModelState.IsValid){
                User curuser= _context.users.SingleOrDefault(user => user.Email == logmodel.LEmail);      
                if(curuser == null)
                {
                    ViewBag.loginemailexist="Email do not exist!";
                    return View("Index");
                }
                else
                {
                    var Hasher = new PasswordHasher<User>();
                    // if(curuser.Password != logmodel.LPassword){
                    if(0 == Hasher.VerifyHashedPassword(curuser, curuser.Password, logmodel.LPassword))
                    {
                        ViewBag.loginpassword="Password error";
                        return View("Index");
                    }
                    else{
                        HttpContext.Session.SetInt32("UserId", curuser.UserId);
                        CookieOptions options = new CookieOptions();
                        options.Expires = DateTime.Now.AddDays(3650);
                        Response.Cookies.Append("uid", curuser.UserId.ToString(),options);
                        return RedirectToAction("Profilepage","Profile",new {userid=curuser.UserId});
                    }
                }
            }
            ViewBag.fromloginform=1;
            return View("Index");
        }

        //Get: Logout
        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/uploadregimg")]
        public JsonResult UploadRegImg(string img)
        {
            int? Uid = HttpContext.Session.GetInt32("UserId");
            if(Uid==null)
            {
                return Json(new {result="fail"});
            }
            else if(Uid==9){
                return Json(new {result="refused"});
            }
            else
            {
                int userid=(int)Uid;
                var parts = img.Split(new char[] { ',' }, 2);
                var bytes = Convert.FromBase64String(parts[1]);
                // DateTime now = DateTime.Now;
                // var filename =string.Format("images/{0}-reg-{1}.png", userid.ToString(),now.ToString("MM-dd-yyyy-HH-mm-ss"));
                var filename =string.Format("images/{0}-reg.png", userid.ToString());
                var webRoot = _env.WebRootPath;
                var file = System.IO.Path.Combine(webRoot, filename);
                // System.IO.File.WriteAllText(file, "Hello World!");
                System.IO.File.WriteAllBytes(file, bytes); 
                Byte[] imgcontent = System.IO.File.ReadAllBytes(file);
                User curuser= _context.users.SingleOrDefault(user => user.UserId == userid);
                curuser.RegImgUrl=Convert.ToString(filename);
                _context.SaveChanges(); 
                // string msapikey=_msoptions.MSApiKey;
                // var FaceInfo = new List<Dictionary<string, object>>();
                // MSFaceApiRequest.FaceDetect(msapikey,imgcontent,ApiResponse =>
                // {
                //     FaceInfo = ApiResponse;
                //     System.Console.WriteLine("*******************");
                //     System.Console.WriteLine(FaceInfo);
                //     // return Json(new {result="success",faceinfo=FaceInfo});
                // }).Wait();
                string base64string = Convert.ToBase64String(imgcontent, 0, imgcontent.Length);
                string imageurl = @"data:image/png;base64," + base64string;
                return Json(new {result="success",imageurl=imageurl});   
            }                 
        }
        [HttpPost]
        [Route("/imglogin")]
        public JsonResult ImgLogin(string img)
        {
            string uid=Request.Cookies["uid"];
            if(img==null||img==""){
                return Json(new {result="fail"});
            }
            int userid;
            if(uid==null || uid=="")
            {
                userid=9;
            //    return Json(new {result="fail"});
            }
            else
            { 
                userid = Int32.Parse(uid);
            }
            var parts = img.Split(new char[] { ',' }, 2);
            Byte[] newbytes = Convert.FromBase64String(parts[1]);
            // string filename =string.Format("images/{0}-reg.png", userid.ToString());
            User curuser= _context.users.SingleOrDefault(user => user.UserId == userid);
            string filename=curuser.RegImgUrl;
            var webRoot = _env.WebRootPath;
            var regfile = System.IO.Path.Combine(webRoot, filename);
            Byte[] regimgcontent = System.IO.File.ReadAllBytes(regfile);
            string msapikey=_msoptions.MSApiKey;
            string faceid1;
            string faceid2;
            faceid1=MSFaceApiRequest.FaceDetect(msapikey,newbytes).Result;
            Console.WriteLine($"Faceid1:{faceid1}");
            faceid2=MSFaceApiRequest.FaceDetect(msapikey,regimgcontent).Result;
            Console.WriteLine($"Faceid2:{faceid2}");
            var VerifyResult = new Dictionary<string, object>();
            Console.WriteLine($"Verify Response: {VerifyResult}");
            VerifyResult=MSFaceApiRequest.FaceVerify(msapikey,faceid1,faceid2).Result;
            Boolean ifidentical=Convert.ToBoolean(VerifyResult["isIdentical"]);
            double confidence=Convert.ToDouble(VerifyResult["confidence"]);
            Console.WriteLine($"isIdentical: {ifidentical}");
            Console.WriteLine($"confidence: {confidence}");
            if(ifidentical==true &&confidence>0.5)
            {
                HttpContext.Session.SetInt32("UserId", curuser.UserId);
                HttpContext.Session.SetString("isIdentical",ifidentical.ToString());
                HttpContext.Session.SetString("confidence",confidence.ToString());
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(3650);
                Response.Cookies.Append("uid", curuser.UserId.ToString(),options);
                return Json(new {result="success"});;                         
            }
            else
            {
                return Json(new {result="fail"}); 
            }                
        }

        [HttpGet]
        [Route("/deleteimg")]
        public JsonResult deleteRegImg()
        {
            int? Uid = HttpContext.Session.GetInt32("UserId");
            if(Uid==null)
            {
                return Json(new {result="fail"});
            }
            else if(Uid==9){
                return Json(new {result="refused"});
            }
            else
            {
                int userid=(int)Uid;
                User curuser= _context.users.SingleOrDefault(user => user.UserId == userid);
                curuser.RegImgUrl="";
                _context.SaveChanges(); 
                return Json(new {result="success"});   
            }                 
        }
        // [HttpGet]
        // [Route("/verify")]
        // public JsonResult Verify()
        // {
        //     int? Uid = HttpContext.Session.GetInt32("UserId");
        //     if(Uid==null)
        //     {
        //         return Json(new {result="fail"});
        //     }
        //     else
        //     {
        //         int userid=(int)Uid;
        //         string faceid1="91295e3b-dd05-4974-9870-c2e4e8ae6491";
        //         string faceid2="a6abb71b-2224-45ca-8c11-d0e5055ec982";
        //         string msapikey=_msoptions.MSApiKey;
        //         var VerifyResult = new Dictionary<string, object>();
        //         MSFaceApiRequest.FaceVerify(msapikey,faceid1,faceid2,ApiResponse =>
        //         {
        //             VerifyResult = ApiResponse;
        //             System.Console.WriteLine("*******************");
        //             System.Console.WriteLine(VerifyResult["isIdentical"]);
        //             System.Console.WriteLine(VerifyResult["confidence"]);
        //             // return Json(new {result="success",faceinfo=FaceInfo});
        //         }).Wait();
        //         return Json(new {result="success"});   
        //     }                 
        // }

    }
}
