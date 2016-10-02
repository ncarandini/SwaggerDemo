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
    [RoutePrefix("api/v1/meetups")]
    [RequireHttps]
    public class ActivityController : ApiController
    {
        // GET: api/v1/meetups/5
        /// <summary>
        /// Gets a specific Meetup based on the ID you request
        /// </summary>
        /// <param name="meetupId">The Meetup ID</param>
        /// <returns>A specific Meetup based on the ID of your request</returns>
        [RequireAppToken]
        [Route("{meetupId:int}")]
        [ResponseType(typeof(Meetup))]
        public async Task<IHttpActionResult> GetMeetupByIdAsync(int meetupId)
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var meetup = await ctx.Meetups.Include("Sessions").FirstOrDefaultAsync(m => m.Id == meetupId);
                    return Ok(meetup);
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

        // GET: api/v1/meetups
        /// <summary>
        /// Gets a paged set of meetups
        /// </summary>
        /// <param name="searchText">The text used for the fulltext search on the Title and Description</param>
        /// <param name="pageIndex">The page index (use 1 for the fist page)</param>
        /// <param name="pageSize">The number of elements per page</param>
        /// <returns>A paged set of meetups</returns>
        [RequireAppToken]
        [Route("")]
        [ValidateModelState]
        [ResponseType(typeof(List<Meetup>))]
        public async Task<IHttpActionResult> GetMeetupsAsync(string searchText = null, int pageIndex = 1, int pageSize = Defaults.PAGE_SIZE)
        {
            try
            {
                Expression<Func<Meetup, bool>> serachTextPredicate = op => true;
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    serachTextPredicate = m => m.Title.Contains(searchText) || m.Description.Contains(searchText);
                }

                using (var ctx = new ApplicationDbContext())
                {
                    IQueryable<Meetup> query = ctx.Meetups.Include("Sessions");

                    if (serachTextPredicate != null)
                    {
                        query = query.Where(serachTextPredicate);
                    }

                    if (pageIndex > 1)
                    {
                        query = query.Skip(pageIndex - 1);
                    }

                    if (pageSize > 0)
                    {
                        query = query.Take(pageSize);
                    }

                    return Ok(await query.ToListAsync());
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

        // POST: api/v1/activities
        /// <summary>
        /// Creates a new meetup
        /// </summary>
        /// <param name="postMeetupDto">A DTO instance with the data for the new activity creation</param>
        /// <returns>The ID of the newly created activity</returns>
        [HttpPost]
        [RequireAppToken]
        // TODO: [CheckModelForNull]
        [ValidateModelState]
        [Route("")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> PostMeetupAsync([FromBody] PostMeetupDto postActivityDto)
        {
            try
            {
                var validationContext = new ValidationContext(postActivityDto, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(postActivityDto, validationContext, validationResults);

                if (isValid)
                {
                    using (var ctx = new ApplicationDbContext())
                    {
                        var meetup = new Meetup
                        {
                            Title = postActivityDto.Title,
                            Description = postActivityDto.Description,
                            StartAt = postActivityDto.StartAt,
                            EndAt = postActivityDto.EndAt,
                            Sessions = null
                        };
                        ctx.Meetups.Add(meetup);
                        await ctx.SaveChangesAsync();

                        return Ok(meetup.Id);
                    }
                }
                else
                {
                    throw new Exception(validationResults?.First()?.ErrorMessage ?? "Activity creation request data is incomplete.");
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

        // PUT: api/v1/meetups/5
        /// <summary>
        /// Set the send completed flag to true for a specified activity
        /// </summary>
        /// <param name="activityId">The activity ID</param>
        /// <returns>The action result</returns>
        [HttpPut]
        [RequireAppToken]
        // [ValidateModelState]
        [Route("{meetupId:int}")]
        [ResponseType(typeof(IHttpActionResult))]
        public async Task<IHttpActionResult> PutMeetupAsync(int meetupId, [FromBody] PutMeetupDto putActivityDto)
        {
            try
            {
                if (meetupId != putActivityDto.Id)
                {
                    throw new Exception("Meetup update request data is incomplete or inconsistent.");
                }

                var validationContext = new ValidationContext(putActivityDto, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(putActivityDto, validationContext, validationResults);

                if (isValid)
                {
                    using (var ctx = new ApplicationDbContext())
                    {
                        var foundMeetup = await ctx.Meetups.FirstOrDefaultAsync(m => m.Id == meetupId);

                        if (foundMeetup == null)
                        {
                            throw new Exception("Meetup not found.");
                        }

                        // Replace non null values
                        if (!string.IsNullOrWhiteSpace(putActivityDto.Title))
                        {
                            foundMeetup.Title = putActivityDto.Title;
                        }

                        if (!string.IsNullOrWhiteSpace(putActivityDto.Description))
                        {
                            foundMeetup.Description = putActivityDto.Description;
                        }

                        if (putActivityDto.StartAt.HasValue)
                        {
                            foundMeetup.StartAt = putActivityDto.StartAt.Value;
                        }

                        if (putActivityDto.EndAt.HasValue)
                        {
                            foundMeetup.EndAt = putActivityDto.EndAt.Value;
                        }

                        await ctx.SaveChangesAsync();

                        return Ok();
                    }
                }
                else
                {
                    throw new Exception(validationResults?.First()?.ErrorMessage ?? "Meetup update request data is incomplete.");
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
