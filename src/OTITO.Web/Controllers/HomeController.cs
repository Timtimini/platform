using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OTITO.Web.Models;
using OTITO.Web.Models.Contact;
using OTITO.Web.Models.Email;
using OTITO.Web.Models.Enum;
using OTITO.Web.Models.Topic;
using OTITO_Services;


namespace OTITO.Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly IHomeService _service;
        private readonly ILogger<HomeController> _log;
        private EmailSettings _emailSettings { get; }
        
        public HomeController(IHomeService service, IOptions<EmailSettings> emailSettings, ILogger<HomeController> log) 
        {
            _service = service;
            _log = log;
            _emailSettings = emailSettings.Value;
        }
        
        public IActionResult Index()

                {
            ViewBag.Title = meta.title.Value;
            ViewBag.Description =meta.description.Value.ToString();
            //"Society's most democratic tool for documenting and sharing objectively verifiable political and socio-cultural truth";

            try
            {
                HomeViewModel _model = new HomeViewModel();
                var data = _service.GetTopics(1, 0, 10, null);
                _model.Topics = data.Select(x => new TopicViewModel
                {
                    DateCreated=x.DateCreated,
                    DateUpdated=x.DateUpdated,
                    Id=x.Id,
                    Slug=x.Slug,
                    TopicName=x.TopicName,
                    isSticky=x.isSticky

                }).ToList();
                var i = 0;
                foreach (var item in _model.Topics)
                {
                    Array values = Enum.GetValues(typeof(Gradient));
                    Random random = new Random();
                    Gradient randomGradient = (Gradient)values.GetValue(i);
                    item.GradientName = randomGradient.ToString();
                    i++;
                    if (i == 3)
                        i = 0;
                }

                return View(_model);
                
             }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "There is an error fetching your request.";
            }
            return View();
        }

        public JsonResult PageStream(int PageNo,string SearchTerm)
        {
            try
            {
                HomeViewModel _model = new HomeViewModel();
                var data = _service.GetTopics(PageNo,0,10,SearchTerm);
                _model.Topics = data.Select(x => new TopicViewModel
                {
                    DateCreated = x.DateCreated,
                    Id = x.Id,
                    Slug=x.Slug,
                    TopicName = x.TopicName

                }).ToList();
                var i = 0;
                foreach (var item in _model.Topics)
                {
                    Array values = Enum.GetValues(typeof(Gradient));
                    Random random = new Random();
                    Gradient randomGradient = (Gradient)values.GetValue(i);
                    item.GradientName = randomGradient.ToString();
                    i++;
                    if (i == 3)
                        i = 0;
                }


                return Json(new { success = true, data = _model });


            }
            catch
            {
                return Json(new { success = false, message="An error occured" });
            }

        }
        [Route("/about")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            ViewBag.Title = "Why we built òtító for the information age";
            ViewBag.Description = meta.description.Value.ToString();
            //"Learn more about thinking underlying òtító—a place where truth is a utilitarian, pluralistic and cohesive representation of the soundest set of conclusions, based on the least contested bodies of knowledge.";
            return View();
        }
        //public IActionResult Help()
        //{
        //    return View();
        //}
        [Route("/donate")]
        public IActionResult Donate()
        {
            ViewBag.Title = "Donate to òtító—a platform for fighting misinformation";
            ViewBag.Description = meta.description.Value.ToString();
            
                //"We believe we can build a community that supports you with the reliable information you need to form healthy opinions. Please support us by paying whatever you believe the pursuit of this goal is worth.";
            return View();
        }
        [HttpGet]
        [Route("/contact")]
        public IActionResult Contact()
        {
            ContactIn _model = new ContactIn();
            ViewBag.Title = "Got a question, idea or some feedback? Let us know";
            ViewBag.Description =meta.description.Value.ToString();
            //"Society's most democratic tool for documenting and sharing objectively verifiable political and socio-cultural truth";
            return View(_model);
        }
        [HttpPost]
        [Route("/contact")]
        public IActionResult Contact(ContactIn _model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var message = new MailMessage())
                    {
                        message.To.Add(new MailAddress(_emailSettings.ToEmail, _emailSettings.ToName));
                        message.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                        message.Subject = "òtító contact";
                        message.Body = "<h2>Dispute</h2>" +
                            "<p><strong>Name:</strong>" + _model.Name + "</p>" +
                            "<p><strong>Email:</strong>" + _model.Email + "</p>" +
                            "<p><strong>Message:</strong>" + _model.Message + "</p>";
                        message.IsBodyHtml = true;

                        using (var client = new SmtpClient(_emailSettings.PrimaryDomain))
                        {
                            client.Port = _emailSettings.PrimaryPort;

                            client.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                            client.EnableSsl = _emailSettings.EnableSsl;
                            client.Send(message);
                        }
                    }

                    //send the email
                    _model.sent = true;
                }
                catch (SmtpException ex)
                {
                    TempData["ErrorMessage"] = "There was a problem disputing the source. Please check back later.";
                    _log.LogError(ex, "Failed to send contact email");
                }
            }

                ViewBag.Title = "Got a question, idea or some feedback? Let us know";
            ViewBag.Description = meta.description.Value.ToString();
            return View(_model);
        }
        [Route("/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[Route("signin")]
        //public IActionResult SignIn() => View();

        //[Route("signin/{provider}")]
        //public IActionResult SignIn(string provider, string returnUrl = null) =>
        //    Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/Users/testProfile" }, provider);

        //[Route("signout")]
        //[HttpPost]
        //public async Task<IActionResult> SignOut()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return RedirectToAction("Index", "Home");
        //}
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [Route("error/404")]
        public IActionResult Status404()
        {
            return View();
        }
        [Route("/sitemap.xml")]
        public void SitemapXml()
        {
            string host = Request.Scheme + "://" + Request.Host;

            Response.ContentType = "application/xml";
            var today = "2019-10-13";
            using (var xml = XmlWriter.Create(Response.Body, new XmlWriterSettings { Indent = true }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", host);
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/topic/this-is-an-example-topic-ie-an-issue-or-question-to-be-discussed-click-me-7aaf3a0b20");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/topic");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/topic/sources");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/topic/ask");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/about");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();


                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/privacy");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/donate");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/contact");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/Users/Login");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteStartElement("url");
                xml.WriteElementString("loc", "https://www.otito.io/Users/Signup");
                xml.WriteElementString("lastmod", today);
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
        }

    }
}
