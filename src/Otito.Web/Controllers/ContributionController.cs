using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Otito.Web.Models.Topic;
using Otito.Services;
using Otito.Services.HelperModel.Topic;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Otito.Web.Models.Email;
using Otito.Web.Models.Slug;
using Otito.Web.Models.Enum;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Otito.Web.Controllers
{
    public class ContributionController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger _logger;

        public ContributionController(
            ITopicService topicService,
            IOptions<EmailSettings> emailSettings,
            ILogger<ContributionController> logger
        )
        {
            _topicService = topicService;
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        [Authorize]
        [Route("/topic/addtopic")]
        // GET: /<controller>/
        public IActionResult AddContribution()
        {
            return View();
        }

        [Authorize]
        [Route("topic/addclaim/{slug}/{claimGuid?}")]
        public IActionResult AddClaim(string slug, string claimGuid = null)
        {
            AddTopicModel _model = new AddTopicModel();
            try
            {
                var topic = _topicService.GetTopic(slug);
                _model.TopicId = topic.Id;
                _model.Slug = topic.Slug;
                _model.Guid = topic.Guid;
                _model.TopicName = topic.TopicName;
                _model.Claim = new AddClaimModel {Guid = claimGuid};
            }
            catch
            {
                TempData["ErrorMessage"] = "There is an issue retrieving data.";
            }

            ViewBag.Title = meta.title.Value.ToString();
            //"òtító | a platform for documenting and sharing truth";

            return View(_model);
        }

        [Authorize]
        [HttpPost]
        [Route("topic/addtopic")]
        public JsonResult AddSource([FromBody] AddTopicModel topic)
        {
            try
            {
                var UserId = User.FindFirst(ClaimTypes.Sid).Value;
                IList<AddSource> _sources = new List<AddSource>();
                foreach (var item in topic.Claim.Sources)
                {
                    _sources.Add(new AddSource
                    {
                        URL = item.URL,
                        Guid = Slug.ToNewGuid()
                    });
                }

                AddClaim _claim = new AddClaim
                {
                    ClaimDescription = topic.Claim.ClaimDescription,
                    ClaimTitle = topic.Claim.ClaimTitle,
                    Sources = _sources,
                    Guid = Slug.ToNewGuid()
                };


                AddTopic _topic = new AddTopic
                {
                    Claim = _claim,
                    TopicName = topic.TopicName,
                    Guid = Slug.ToNewGuid(),
                    Slug = Slug.ToUrlSlug(topic.TopicName),
                };
                string slug = _topicService.SaveTopic(_topic, Convert.ToInt32(UserId));
                return Json(new {success = true, data = new {slug}});
            }
            catch (Exception ex)
            {
                return Json(new {success = false,});
            }
        }

        [Authorize]
        [HttpPost]
        [Route("topic/saveclaim")]
        public JsonResult AddClaim([FromBody] AddTopicModel topic)
        {
            try
            {
                var UserId = User.FindFirst(ClaimTypes.Sid).Value;
                IList<AddSource> _sources = new List<AddSource>();
                foreach (var item in topic.Claim.Sources)
                {
                    _sources.Add(new AddSource
                    {
                        URL = item.URL,
                        Guid = Slug.ToNewGuid()
                    });
                }

                AddClaim _claim = new AddClaim
                {
                    ClaimDescription = topic.Claim.ClaimDescription,
                    ClaimTitle = topic.Claim.ClaimTitle,
                    ParentGuid = topic.Claim.Guid,
                    Sources = _sources,
                    Guid = Slug.ToNewGuid(),
                    TopicSlug = Slug.GetSlug(topic.Slug),
                };
                int a = _topicService.SaveClaim(_claim, topic.TopicId, Convert.ToInt32(UserId));
                return Json(new {success = true, data = new {TopicId = a}});
            }
            catch
            {
                return Json(new {success = false,});
            }
        }

        [Route("topic/ask/{slug}")]
        public IActionResult SolicitEvidenceTopic(string slug)
        {
            var topic = _topicService.TopicSimpleDetail(slug);
            TopicModel _model = new TopicModel
            {
                TopicName = topic.TopicName,
                Slug = topic.Slug
            };
            ViewBag.Title = topic.TopicName;
            ViewBag.Description = meta.preamble.Value.ToString() + topic.Claims.FirstOrDefault().ClaimDescription;
            return View(_model);
        }

        public IActionResult SolicitEvidence(int ClaimId, int TopicId)
        {
            var _claim = _topicService.ClaimDetail(ClaimId, TopicId);
            TopicModel _model = new TopicModel();
            _model.TopicName = _claim.TopicName;
            _model.Id = _claim.Id;
            var claim = new ClaimModel
            {
                ClaimTitle = _claim.Claims.FirstOrDefault().ClaimTitle,
                ClaimDescription = _claim.Claims.FirstOrDefault().ClaimDescription,
                Id = ClaimId,
            };

            ViewBag.Description = _claim.TopicName;
            _model.Claim = claim;
            return View(_model);
        }

        [Route("topic/thankyou/{slug}")]
        public IActionResult ThankYou(string slug)
        {
            var topic = _topicService.TopicSimpleDetail(slug);
            TopicModel _model = new TopicModel
            {
                TopicName = topic.TopicName,
                Slug = slug
            };

            ViewBag.Description = topic.TopicName;
            ViewBag.Title = meta.preamble.Value.ToString() + topic.Claims.FirstOrDefault().ClaimDescription +
                            meta.postfix.Value.ToString();
            return View(_model);
        }

        public IActionResult ClaimThankYou(int ClaimId, int TopicId)
        {
            var _claim = _topicService.ClaimDetail(ClaimId, TopicId);
            var _model = new ClaimModel
            {
                ClaimTitle = _claim.Claims.FirstOrDefault().ClaimTitle,
                ClaimDescription = _claim.Claims.FirstOrDefault().ClaimDescription,
                Id = ClaimId,
            };


            return View(_model);
        }

        [Route("topic/simple/{slug}")]
        public IActionResult simple(string slug)
        {
            return Redirect("/topic/" + slug);
        }

        [Route("topic/counter/{slug}/{searchString?}")]
        public IActionResult TopicEditor(string slug, string searchString = null)
        {
            return Redirect("/topic/" + slug + "/" + searchString);
        }

        [Route("topic/{slug}/{searchString?}")]
        public IActionResult TopicEditorView(string slug, string searchString = null)
        {
            var topic = _topicService.TopicEditorDetail(slug, searchString);

            var model = new TopicViewModel();
            model.Id = topic.Id;
            model.TopicName = topic.TopicName;
            model.Slug = topic.Slug;
            model.TotalVotes = topic.TotalVotes;
            model.Guid = topic.Guid;
            model.DateCreated = topic.DateCreated;
            model.search = searchString;
            model.isSticky = topic.isSticky;
            model.Contributors = topic.Contributors;

            model.Claims = (from c in topic.Claims
                    select new ClaimViewModel
                    {
                        ClaimDescription = c.ClaimDescription,
                        ClaimTitle = c.ClaimTitle,
                        Id = c.Id,
                        Guid = c.Guid,
                        Slug = c.Slug,
                        Status = c.Status,
                        TotalVotes = c.TotalVotes,
                        UpVotes = c.UpVotes,
                        DownVotes = c.DownVotes,
                        Sources = c.Sources.Select(v => new SourceViewModel
                        {
                            Id = v.Id,
                            URL = v.URL,
                            Title = v.Title,
                            Vote = v.Vote,
                            NegativeKarma = (v.TotalVote == 0 ? 0 : (double) v.NegativeKarma / (double) (v.TotalVote)) *
                                            100,
                            PositiveKarma = (v.TotalVote == 0 ? 0 : (double) v.PositiveKarma / (double) (v.TotalVote)) *
                                            100,
                            Slug = v.Slug,
                            Guid = v.Guid
                        }).ToList(),
                        KarmaPercent = c.TotalVotes == 0 ? 0 : ((double) c.DownVotes / (double) c.TotalVotes) * 100,
                        CounterClaims = (from cc in c.CounterClaims
                                select new ClaimViewModel
                                {
                                    ClaimDescription = cc.ClaimDescription,
                                    ClaimTitle = cc.ClaimTitle,
                                    Id = cc.Id,
                                    Slug = cc.Slug,
                                    Guid = cc.Guid,
                                    ParentGuid = cc.ParentGuid,
                                    TotalVotes = cc.TotalVotes,
                                    UpVotes = cc.UpVotes,
                                    DownVotes = cc.DownVotes,
                                    KarmaPercent = c.TotalVotes == 0
                                        ? 0
                                        : ((double) cc.DownVotes / (double) cc.TotalVotes) * 100,
                                    Sources = cc.Sources.Select(t => new SourceViewModel
                                    {
                                        Id = t.Id,
                                        URL = t.URL,
                                        Title = t.Title,
                                        Vote = t.Vote,
                                        NegativeKarma =
                                            (t.TotalVote == 0 ? 0 : (double) t.NegativeKarma / (double) (t.TotalVote)) *
                                            100,
                                        PositiveKarma =
                                            (t.TotalVote == 0 ? 0 : (double) t.PositiveKarma / (double) (t.TotalVote)) *
                                            100,
                                        Slug = t.Slug,
                                        Guid = t.Guid
                                    }).ToList()
                                }
                            ).ToList()
                    }
                ).ToList();
            var description = topic.Claims.Count > 0
                ? topic.Claims.FirstOrDefault().ClaimDescription
                : topic.TopicName;
            ViewBag.Description = meta.preamble.Value.ToString() + description + meta.postfix.Value.ToString();
            ViewBag.Title = topic.TopicName;
            return View(model);
        }

        [Route("topic/sources/{slug}")]
        public IActionResult ViewSources(string slug)
        {
            TopicViewModel _model = new TopicViewModel();
            try
            {
                _logger.LogWarning("before execution", null, "GetById({SLUG})", slug);
                var result = _topicService.ClaimDetailSource(slug);
                if (result != null)
                {
                    _model.TopicName = result.TopicName;
                    _model.Id = result.Id;
                    _model.Slug = result.Slug;
                    _model.Guid = result.Guid;
                    _model.Claims = result.Claims.Select(x => new ClaimViewModel
                    {
                        Id = x.Id,
                        ClaimDescription = x.ClaimDescription,
                        ClaimTitle = x.ClaimTitle,
                        isMostValidated = (x.ClaimId == 0),
                        Guid = x.Guid,
                        Slug = x.Slug,
                        Status = x.Status,
                        CounterClaims = (from cc in x.CounterClaims
                                select new ClaimViewModel
                                {
                                    ClaimDescription = cc.ClaimDescription,
                                    ClaimTitle = cc.ClaimTitle,
                                    Id = cc.Id,
                                    Slug = cc.Slug,
                                    Guid = cc.Guid,
                                    ParentGuid = cc.ParentGuid
                                }
                            ).ToList(),
                        Sources = x.Sources.Select(v => new SourceViewModel
                        {
                            Id = v.Id,
                            URL = v.URL,
                            Title = v.Title,
                            Vote = v.Vote,
                            NegativeKarma = (v.TotalVote == 0 ? 0 : (double) v.NegativeKarma / (double) (v.TotalVote)) *
                                            100,
                            PositiveKarma = (v.TotalVote == 0 ? 0 : (double) v.PositiveKarma / (double) (v.TotalVote)) *
                                            100,
                            Slug = v.Slug,
                            Guid = v.Guid
                        }).ToList()
                    }).ToList();
                }
                else
                {
                    TempData["ErrorMessage"] = "null happened";
                }


                ViewBag.Description = meta.preamble.Value.ToString() + result.Claims.FirstOrDefault().ClaimDescription +
                                      meta.postfix.Value;
                ViewBag.Title = result.Claims.FirstOrDefault().ClaimTitle;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "There is an error fetching your request";
                TempData["Exception"] = ex;
                _logger.LogWarning("Inside Exception", ex, "GetById({SLUG}) NOT FOUND", slug);
            }
            //ViewBag.Description = meta.preamble+result.Claims.FirstOrDefault().ClaimDescription+meta.postfix;
            //ViewBag.Title = result.TopicName;

            return View(_model);
        }

        [Authorize]
        public JsonResult AddVote(int SourceId, int ClaimId, int TopicId, int Vote)
        {
            var UserId = User.FindFirst(ClaimTypes.Sid).Value;
            if (Vote == 1 || Vote == -1)
            {
                try
                {
                    var _vote = _topicService.AddVote(SourceId, ClaimId, TopicId, Vote, Convert.ToInt32(UserId));
                    if (!_vote.success)
                    {
                        return Json(new {success = false, Message = "You cannot vote multiple times"});
                    }

                    return Json(new {success = true, Vote = _vote.Vote});
                }
                catch
                {
                    return Json(new {success = false, Message = "There was an error posting data"});
                }
            }

            return Json(new {success = false, Message = "Invalid Vote"});
        }

        public JsonResult GetVote(int ClaimId)
        {
            try
            {
                var data = _topicService.GetVotes(ClaimId);
                return Json(new {success = true, data = data});
            }
            catch (Exception ex)
            {
                return Json(new {success = false, Message = "There was an error fetching data"});
            }
        }

        public JsonResult GetSourceVotes(string SourceIds)
        {
            try
            {
                List<int> Sources = SourceIds.Split(',').Select(int.Parse).ToList();


                var data = _topicService.GetSourceVotes(Sources);
                return Json(new {success = true, data = data});
            }
            catch (Exception ex)
            {
                return Json(new {success = false, Message = "There was an error fetching data"});
            }
        }

        [Authorize]
        [Route("topic/addsource")]
        public JsonResult addNewSource(string slug, string Source)
        {
            try
            {
                string slugWithoutGuid = Slug.GetSlug(slug);
                var UserId = User.FindFirst(ClaimTypes.Sid).Value;
                var Id = _topicService.SaveSource(Slug.ToNewGuid(), slug, slugWithoutGuid, Source,
                    Convert.ToInt32(UserId));
                return Json(new {success = true, slug = Id});
            }
            catch (Exception ex)
            {
                return Json(new {success = false, Message = "There was an error posting data"});
            }
        }

        [Authorize]
        [Route("topic/delete/{slug}")]
        public IActionResult DeleteTopic(string slug)
        {
            try
            {
                var Role = User.FindFirst(ClaimTypes.Role).Value;

                if (Role.ToString().ToLower().Equals("admin"))
                {
                    _topicService.DeleteTopic(slug);
                    TempData["SuccessMessage"] = "Topic has been deleted successfully";
                    return Redirect("/");
                }
            }
            catch
            {
            }

            TempData["ErrorMessage"] = "An occured while deleting this record.";
            return Redirect("/topic/simple/" + slug);
        }

        [Authorize]
        [Route("topic/deletesource/{slug}/{claimSlug}")]
        public IActionResult DeleteSource(string slug, string ClaimSlug)
        {
            try
            {
                var Role = User.FindFirst(ClaimTypes.Role).Value;

                if (Role.ToString().ToLower().Equals("admin"))
                {
                    _topicService.DeleteSource(slug);
                    TempData["SuccessMessage"] = "Source has been deleted successfully";
                    return Redirect("/");
                }
            }
            catch
            {
            }

            TempData["ErrorMessage"] = "An occured while deleting this record.";
            return Redirect("topic/sources/" + ClaimSlug);
        }

        [Authorize]
        [Route("topic/deleteclaim/{slug}")]
        public IActionResult DeleteClaim(string slug)
        {
            try
            {
                var Role = User.FindFirst(ClaimTypes.Role).Value;

                if (Role.ToString().ToLower().Equals("admin"))
                {
                    _topicService.DeleteClaim(slug);
                    TempData["SuccessMessage"] = "Source has been deleted successfully";
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
            }

            TempData["ErrorMessage"] = "An occured while deleting this record.";
            return Redirect("/topic/counter/" + slug);
        }

        [HttpGet]
        [Authorize]
        [Route("topic/dispute/{slug}")]
        public IActionResult SourceDispute(string slug)
        {
            TopicViewModel _model = new TopicViewModel();
            try
            {
                var result = _topicService.SourceDispute(slug);
                _model.TopicName = result.TopicName;
                _model.Id = result.Id;
                _model.Slug = result.Slug;
                _model.Guid = result.Guid;
                _model.Claims = result.Claims.Select(x => new ClaimViewModel
                {
                    Id = x.Id,
                    ClaimDescription = x.ClaimDescription,
                    ClaimTitle = x.ClaimTitle,
                    Guid = x.Guid,
                    Slug = x.Slug,
                    Sources = x.Sources.Select(v => new SourceViewModel
                    {
                        Id = v.Id,
                        URL = v.URL,
                        Title = v.Title,
                        Vote = v.Vote,
                        Slug = v.Slug,
                        Guid = v.Guid,
                    }).ToList()
                }).ToList();
                var dispute = _model.Claims.FirstOrDefault().Sources.FirstOrDefault();
                _model.Dispute = new AddDispute
                {
                    ClaimSlug = _model.Claims.FirstOrDefault().Slug,
                    TopicSlug = _model.Slug,
                    SourceSlug = slug,
                    SourceTitle = dispute.Title,
                    SourceURL = dispute.URL,
                };

                ViewBag.Description = meta.preamble.Value.ToString() + result.Claims.FirstOrDefault().ClaimDescription +
                                      meta.postfix.Value.ToString();
                ViewBag.Title = result.TopicName;
            }
            catch
            {
                TempData["ErrorMessage"] = "There is an error fetching your request";
            }

            return View(_model);
        }

        [HttpPost]
        [Authorize]
        [Route("topic/adddispute")]
        public IActionResult SourceDispute(AddDispute dispute)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var message = new MailMessage())
                    {
                        message.To.Add(new MailAddress(_emailSettings.ToEmail, _emailSettings.ToName));
                        message.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                        message.Subject = "òtító dispute";
                        message.Body = "<h2>Dispute</h2>" +
                                       "<p><strong>Reason:</strong>" + dispute.Title + "</p>" +
                                       "<p><strong>Double Check:</strong>" + dispute.Source + "</p>" +
                                       "<p><strong>Source:</strong>" + dispute.SourceURL + "</p>" +
                                       "<p><strong>View:</strong><a href='http://otito.io/topic/sources/" +
                                       dispute.ClaimSlug + "'>Click to View</a></p>";
                        message.IsBodyHtml = true;

                        using (var client = new SmtpClient(_emailSettings.PrimaryDomain))
                        {
                            client.Port = _emailSettings.PrimaryPort;

                            client.Credentials = new NetworkCredential(_emailSettings.UsernameEmail,
                                _emailSettings.UsernamePassword);
                            client.EnableSsl = _emailSettings.EnableSsl;
                            client.Send(message);
                        }
                    }

                    //send the email
                    TempData["SuccessMessage"] = "Your dispute has been initiated. We will act accordingly.";
                    return Redirect("/topic/sources/" + dispute.ClaimSlug);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "There was a problem disputing the source. Please check back later.";
                    return Redirect("/topic/sources/" + dispute.ClaimSlug);
                }
            }
            else
            {
                TopicViewModel _model = new TopicViewModel();
                try
                {
                    var result = _topicService.SourceDispute(dispute.SourceSlug);
                    _model.TopicName = result.TopicName;
                    _model.Id = result.Id;
                    _model.Slug = result.Slug;
                    _model.Guid = result.Guid;
                    _model.Claims = result.Claims.Select(x => new ClaimViewModel
                    {
                        Id = x.Id,
                        ClaimDescription = x.ClaimDescription,
                        ClaimTitle = x.ClaimTitle,
                        Guid = x.Guid,
                        Slug = x.Slug,
                        Sources = x.Sources.Select(v => new SourceViewModel
                        {
                            Id = v.Id,
                            URL = v.URL,
                            Title = v.Title,
                            Vote = v.Vote,
                            Slug = v.Slug,
                            Guid = v.Guid,
                        }).ToList()
                    }).ToList();
                    var _dispute = _model.Claims.FirstOrDefault().Sources.FirstOrDefault();
                    _model.Dispute = new AddDispute
                    {
                        ClaimSlug = _model.Claims.FirstOrDefault().Slug,
                        TopicSlug = _model.Slug,
                        SourceSlug = _dispute.Slug,
                        SourceTitle = dispute.Title,
                        SourceURL = dispute.SourceURL
                    };
                }
                catch
                {
                    TempData["ErrorMessage"] = "There is an error fetching your request";
                }

                ViewBag.Description = meta.preamble.Value.ToString() + _model.Claims.FirstOrDefault().ClaimDescription +
                                      meta.postfix.Value.ToString();
                ViewBag.Title = _model.TopicName;

                return View(_model);
            }
        }

        [Authorize]
        [Route("topic/addsticky/{slug}")]
        public IActionResult AddSticky(string slug)
        {
            try
            {
                var Role = User.FindFirst(ClaimTypes.Role).Value;

                if (Role.ToString().ToLower().Equals("admin"))
                {
                    _topicService.addSticky(slug);
                    TempData["SuccessMessage"] = "Topic has been sticked";
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
            }

            TempData["ErrorMessage"] = "An occured while sticking this record.";
            return Redirect("/topic/simple" + slug);
        }

        [Authorize]
        [Route("topic/removesticky/{slug}")]
        public IActionResult RemoveSticky(string slug)
        {
            try
            {
                var Role = User.FindFirst(ClaimTypes.Role).Value;

                if (Role.ToString().ToLower().Equals("admin"))
                {
                    _topicService.removeSticky(slug);
                    TempData["SuccessMessage"] = "Topic has been unsticked";
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
            }

            TempData["ErrorMessage"] = "An occured while unsticking this record.";
            return Redirect("/topic/simple" + slug);
        }

        [Authorize]
        [Route("topic/addslug")]
        public IActionResult addSlug()
        {
            try
            {
                var Role = User.FindFirst(ClaimTypes.Role).Value;

                if (Role.ToString().ToLower().Equals("admin"))
                {
                    _topicService.addSlugs();
                    TempData["SuccessMessage"] = "Slugs are made";
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
            }

            TempData["ErrorMessage"] = "An occured while unsticking this record.";
            return Redirect("/");
        }
    }
}