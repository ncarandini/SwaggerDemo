using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SwaggerDemo.WebApp.DTOs;
using SwaggerDemo.WebApp.Filters;
using SwaggerDemo.WebApp.Models;

namespace SwaggerDemo.WebApp.Controllers
{
    /// <summary>
    /// The Activity Controller
    /// </summary>
    [RoutePrefix("api/v1/sessions")]
    [RequireHttps]
    public class SessionController : ApiController
    {
        // GET: api/v1/sessions/5
        /// <summary>
        /// Gets a specific Session based on the ID you request
        /// </summary>
        /// <param name="sessionId">The Session ID</param>
        /// <returns>A specific Session based on the ID of your request</returns>
        [RequireAppToken]
        [Route("{sessionId:int}")]
        [ResponseType(typeof(GetSessionDto))]
        public async Task<IHttpActionResult> GetSessionByIdAsync(int sessionId)
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var session = await ctx.Sessions
                        .Include(s => s.Proponent)
                        .Include(s => s.Moderator)
                        .Include(s => s.Votes)
                        .Where(m => m.Id == sessionId)
                        .Select(s => new GetSessionDto
                        {
                            Id = s.Id,
                            Title = s.Title,
                            Description = s.Description,
                            SessionState = s.SessionState,
                            ProposedAt = s.ProposedAt,
                            CancelledAt = s.CancelledAt,
                            ProponentFullName = s.Proponent.FirstName + " " + s.Proponent.LastName,
                            ModeratorFullName = s.Moderator.FirstName + " " + s.Moderator.LastName,
                            NumOfVotes = s.Votes.Count,
                            MeetupId = s.MeetupId
                        })
                        .FirstOrDefaultAsync();
                    return Ok(session);
                }
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.SeeOther)
                {
                    Content = new StringContent(ex.Message)
                };
                throw new HttpResponseException(resp);
            }
        }

        // GET: api/v1/sessions
        /// <summary>
        /// Gets a paged set of sessions
        /// </summary>
        /// <param name="searchText">The text used for the fulltext search on the Title and Description</param>
        /// <param name="pageIndex">The page index (use 1 for the fist page)</param>
        /// <param name="pageSize">The number of elements per page</param>
        /// <returns>A paged set of sessions</returns>
        [RequireAppToken]
        [Route("")]
        [ValidateModelState]
        [ResponseType(typeof(List<GetSessionDto>))]
        public async Task<IHttpActionResult> GetSessionsAsync(string searchText = null, int pageIndex = 1, int pageSize = Defaults.PAGE_SIZE)
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    IQueryable<Session> query = ctx.Sessions.Include(s => s.Votes);

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        query = query.Where(s => s.Title.Contains(searchText) || s.Description.Contains(searchText));
                    }

                    if (pageIndex > 1)
                    {
                        query = query.Skip(pageIndex - 1);
                    }

                    if (pageSize > 0)
                    {
                        query = query.Take(pageSize);
                    }

                    var sessions = query.Select(s => new GetSessionDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Description = s.Description,
                        SessionState = s.SessionState,
                        ProposedAt = s.ProposedAt,
                        CancelledAt = s.CancelledAt,
                        ProponentFullName = s.Proponent.FirstName + " " + s.Proponent.LastName,
                        ModeratorFullName = s.Moderator.FirstName + " " + s.Moderator.LastName,
                        NumOfVotes = s.Votes.Count,
                        MeetupId = s.MeetupId
                    });

                    return Ok(await sessions.ToListAsync());
                }
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.SeeOther)
                {
                    Content = new StringContent(ex.Message)
                };
                throw new HttpResponseException(resp);
            }
        }

        // GET: api/v1/sessions/proposed
        /// <summary>
        /// Gets a paged set of proposed sessions still waiting for scheduling
        /// </summary>
        /// <param name="searchText">The text used for the fulltext search on the Title and Description</param>
        /// <param name="pageIndex">The page index (use 1 for the fist page)</param>
        /// <param name="pageSize">The number of elements per page</param>
        /// <returns>A paged set of sessions</returns>
        [RequireAppToken]
        [Route("proposed")]
        [ValidateModelState]
        [ResponseType(typeof(List<GetSessionDto>))]
        public async Task<IHttpActionResult> GetProposedSessionsAsync(string searchText = null, int pageIndex = 1, int pageSize = Defaults.PAGE_SIZE)
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    IQueryable<Session> query = ctx.Sessions.Include(s => s.Votes);

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        query = query.Where(s => (s.SessionState == SessionState.Proposed) && (s.Title.Contains(searchText) || s.Description.Contains(searchText)));
                    }
                    else
                    {
                        query = query.Where(s => s.SessionState == SessionState.Proposed);
                    }

                    if (pageIndex > 1)
                    {
                        query = query.Skip(pageIndex - 1);
                    }

                    if (pageSize > 0)
                    {
                        query = query.Take(pageSize);
                    }

                    var sessions = query.Select(s => new GetSessionDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Description = s.Description,
                        SessionState = s.SessionState,
                        ProposedAt = s.ProposedAt,
                        CancelledAt = s.CancelledAt,
                        ProponentFullName = s.Proponent.FirstName + " " + s.Proponent.LastName,
                        ModeratorFullName = s.Moderator.FirstName + " " + s.Moderator.LastName,
                        NumOfVotes = s.Votes.Count,
                        MeetupId = s.MeetupId
                    });

                    return Ok(await sessions.ToListAsync());
                }
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.SeeOther)
                {
                    Content = new StringContent(ex.Message)
                };
                throw new HttpResponseException(resp);
            }
        }

        // POST: api/v1/sessions
        /// <summary>
        /// Creates a new session
        /// </summary>
        /// <param name="postSessionDto">A DTO instance with the data for the new activity creation</param>
        /// <returns>The ID of the newly created activity</returns>
        [HttpPost]
        [RequireAppToken]
        // TODO: [CheckModelForNull]
        [ValidateModelState]
        [Route("")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> PostSessionAsync([FromBody] PostSessionDto postSessionDto)
        {
            try
            {
                var validationContext = new ValidationContext(postSessionDto, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(postSessionDto, validationContext, validationResults);

                if (isValid)
                {
                    using (var ctx = new ApplicationDbContext())
                    {
                        var foundProponent = await ctx.Users.FirstOrDefaultAsync(u => u.UserName == postSessionDto.ProponentUserName);
                        var foundModerator = await ctx.Users.FirstOrDefaultAsync(u => u.UserName == postSessionDto.ModeratorUserName);

                        if(foundProponent == null || foundModerator == null)
                        {
                            throw new Exception(validationResults?.First()?.ErrorMessage ?? "Session creation request data is incomplete or inconsistent.");
                        }

                        var session = new Session
                        {
                            Title = postSessionDto.Title,
                            Description = postSessionDto.Description,
                            SessionState = SessionState.Proposed,
                            ProposedAt = DateTimeOffset.Now,
                            CancelledAt = default(DateTimeOffset),
                            Proponent = foundProponent,
                            Moderator = foundModerator,
                            Meetup = null,
                            Votes = null
                        };

                        ctx.Sessions.Add(session);
                        await ctx.SaveChangesAsync();

                        return Ok(session.Id);
                    }
                }
                else
                {
                    throw new Exception(validationResults?.First()?.ErrorMessage ?? "Session creation request data is incomplete.");
                }
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.SeeOther)
                {
                    Content = new StringContent(ex.Message)
                };
                throw new HttpResponseException(resp);
            }
        }

        // PUT: api/v1/sessions/5
        /// <summary>
        /// Set the send completed flag to true for a specified activity
        /// </summary>
        /// <param name="activityId">The activity ID</param>
        /// <returns>The action result</returns>
        [HttpPut]
        [RequireAppToken]
        // [ValidateModelState]
        [Route("{sessionId:int}")]
        [ResponseType(typeof(IHttpActionResult))]
        public async Task<IHttpActionResult> PutSessionAsync(int sessionId, [FromBody] PutSessionDto putSessionDto)
        {
            try
            {
                var validationContext = new ValidationContext(putSessionDto, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(putSessionDto, validationContext, validationResults);

                if (isValid)
                {
                    if (sessionId != putSessionDto.Id)
                    {
                        throw new Exception("Session update request data is incomplete or inconsistent.");
                    }

                    using (var ctx = new ApplicationDbContext())
                    {
                        var foundSession = await ctx.Sessions.FirstOrDefaultAsync(m => m.Id == sessionId);

                        if (foundSession == null)
                        {
                            throw new Exception("Session not found.");
                        }

                        // Replace non null values
                        if (!string.IsNullOrWhiteSpace(putSessionDto.Title))
                        {
                            foundSession.Title = putSessionDto.Title;
                        }

                        if (!string.IsNullOrWhiteSpace(putSessionDto.Description))
                        {
                            foundSession.Description = putSessionDto.Description;
                        }

                        if (putSessionDto.SessionState.HasValue)
                        {
                            foundSession.SessionState = putSessionDto.SessionState.Value;
                        }

                        if (!string.IsNullOrWhiteSpace(putSessionDto.ModeratorUserName))
                        {
                            var foundModerator = await ctx.Users.FirstOrDefaultAsync(u => u.UserName == putSessionDto.ModeratorUserName);

                            if (foundModerator == null)
                            {
                                throw new Exception(validationResults?.First()?.ErrorMessage ?? "Session update request data is inconsistent.");
                            }

                            foundSession.Moderator = foundModerator;
                        }

                        await ctx.SaveChangesAsync();

                        return Ok();
                    }
                }
                else
                {
                    throw new Exception(validationResults?.First()?.ErrorMessage ?? "Session update request data is incomplete.");
                }
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.SeeOther)
                {
                    Content = new StringContent(ex.Message)
                };
                throw new HttpResponseException(resp);
            }
        }

    }
}
