﻿using System;
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
    public class MeetupController : ApiController
    {
        // GET: api/v1/meetups/5
        /// <summary>
        /// Gets a specific Meetup based on the ID you request
        /// </summary>
        /// <param name="meetupId">The Meetup ID</param>
        /// <returns>A specific Meetup based on the ID of your request</returns>
        [RequireAppToken]
        [Route("{meetupId:int}")]
        [ResponseType(typeof(GetMeetupDto))]
        public async Task<IHttpActionResult> GetMeetupByIdAsync(int meetupId)
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var getMeetupDto = await ctx.Meetups
                        .Include(m => m.Sessions)
                        .Include("Sessions.Votes")
                        .Where(m => m.Id == meetupId)
                        .Select(m => new GetMeetupDto
                        {
                            Id = m.Id,
                            Title = m.Title,
                            Description = m.Description,
                            StartAt = m.StartAt,
                            EndAt = m.EndAt,
                            Sessions = m.Sessions.Select(s => new GetSessionDto
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
                            }).ToList()
                        })
                        .FirstOrDefaultAsync();

                    return Ok(getMeetupDto);
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
        [ResponseType(typeof(List<GetMeetupDto>))]
        public async Task<IHttpActionResult> GetMeetupsAsync(string searchText = null, int pageIndex = 1, int pageSize = Defaults.PAGE_SIZE)
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    IQueryable<Meetup> query = ctx.Meetups.Include("Sessions").Include("Sessions.Votes");

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        query = query.Where(m => m.Title.Contains(searchText) || m.Description.Contains(searchText));
                    }

                    if (pageIndex > 1)
                    {
                        query = query.Skip(pageIndex - 1);
                    }

                    if (pageSize > 0)
                    {
                        query = query.Take(pageSize);
                    }

                    var meetups = query.Select(m => new GetMeetupDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Description = m.Description,
                        StartAt = m.StartAt,
                        EndAt = m.EndAt,
                        Sessions = m.Sessions.Select(s => new GetSessionDto
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
                        }).ToList()
                    });

                    return Ok(await meetups.ToListAsync());
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

        // POST: api/v1/meetups
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
        public async Task<IHttpActionResult> PostMeetupAsync([FromBody] PostMeetupDto postMeetupDto)
        {
            try
            {
                var validationContext = new ValidationContext(postMeetupDto, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(postMeetupDto, validationContext, validationResults);

                if (isValid)
                {
                    using (var ctx = new ApplicationDbContext())
                    {
                        var meetup = new Meetup
                        {
                            Title = postMeetupDto.Title,
                            Description = postMeetupDto.Description,
                            StartAt = postMeetupDto.StartAt,
                            EndAt = postMeetupDto.EndAt,
                            Sessions = null
                        };
                        ctx.Meetups.Add(meetup);
                        await ctx.SaveChangesAsync();

                        return Ok(meetup.Id);
                    }
                }
                else
                {
                    throw new Exception(validationResults?.First()?.ErrorMessage ?? "Meetup creation request data is incomplete.");
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
        public async Task<IHttpActionResult> PutMeetupAsync(int meetupId, [FromBody] PutMeetupDto putMeetupDto)
        {
            try
            {
                if (meetupId != putMeetupDto.Id)
                {
                    throw new Exception("Meetup update request data is incomplete or inconsistent.");
                }

                var validationContext = new ValidationContext(putMeetupDto, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(putMeetupDto, validationContext, validationResults);

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
                        if (!string.IsNullOrWhiteSpace(putMeetupDto.Title))
                        {
                            foundMeetup.Title = putMeetupDto.Title;
                        }

                        if (!string.IsNullOrWhiteSpace(putMeetupDto.Description))
                        {
                            foundMeetup.Description = putMeetupDto.Description;
                        }

                        if (putMeetupDto.StartAt.HasValue)
                        {
                            foundMeetup.StartAt = putMeetupDto.StartAt.Value;
                        }

                        if (putMeetupDto.EndAt.HasValue)
                        {
                            foundMeetup.EndAt = putMeetupDto.EndAt.Value;
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
