﻿using Microsoft.EntityFrameworkCore;
using Pwa.Domain.Product;
using Pwa.Infrastructure.EfCore;
using Pwa.Query.Contracts.Comment;
using Pwa.Query.Contracts.WebApp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework;
using WebFramework.Domain;
using WebFramework.Enums;
using WebFramework.Infrastructure;
using WebFramework.Utilities;

namespace Pwa.Query.Queries
{
    public class WebAppQuery : IWebAppQuery
    {
        private readonly ApplicationDbContext _context;
        public WebAppQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WebAppQueryModel>> GetBests()
        {
            var webApps = _context.WebApplications.OrderBy(_ => _.Installed).OrderByDescending(_ => _.Installed).Take(5).Select(_ => new WebAppQueryModel
            {
                Id = _.Id,
                Name = _.Name,
                Address = _.WebSiteAddress,
                Category = _.Category.Title,
                Icon = _.Icon,
                Installed = _.Installed,
            }).AsNoTracking();


            return await webApps.ToListAsync();
        }

        public async Task<List<WebAppQueryModel>> GetGames()
        {
            var games = _context.WebApplications.OrderByDescending(_ => _.Visit).Include(_ => _.Pictures).Where(_ => _.IsGame && _.Status == Status.Active).Select(_ => new WebAppQueryModel
            {
                Id = _.Id,
                Name = _.Name,
                Description = _.Description.Substring(0, 30),
                Address = _.WebSiteAddress,
                Picture = _.Pictures.Select(_ => _.FileName).First(),
                Visit = _.Visit
            }).AsNoTracking();

            return await games.ToListAsync();
        }

        public async Task<List<WebAppQueryModel>> GetMostVisit()
        {
            var webApps = _context.WebApplications.OrderBy(_ => _.Visit).OrderByDescending(_ => _.Visit).Take(5).Include(_ => _.Pictures).Select(_ => new WebAppQueryModel
            {
                Id = _.Id,
                Name = _.Name,
                Address = _.WebSiteAddress,
                Picture = _.Pictures.Select(_ => _.FileName).First(),
                Visit = _.Visit
            }).AsNoTracking();

            return await webApps.ToListAsync();
        }

        public async Task<OperationResult<WebAppQueryModel>> GetSingle(int id)
        {
            var result = await _context.WebApplications.Where(_ => _.Status == Status.Active).FirstOrDefaultAsync(_ => _.Id == id);
            if (result is null)
                return new OperationResult<WebAppQueryModel>(new WebAppQueryModel(), false);
            result.IncreaseVisit();
            await _context.SaveChangesAsync();

            var data = await _context.WebApplications
                .Where(_ => _.Status == Status.Active)
                .Include(_ => _.Pictures)
                .Include(_ => _.Comments).ThenInclude(_ => _.User)
                .Include(_ => _.Category)
                .Select(_ => new WebAppQueryModel
                {
                    Id = _.Id,
                    Name = _.Name,
                    Category = _.Category.Title,
                    Installed = _.Installed,
                    Rate = 2f,//not implemented
                    Icon = _.Icon,
                    Pictures = MapPictures(_.Pictures),
                    Description = _.Description,
                    Comments = MapComments(_.Comments),
                    CommentCount = _.Comments.Count(_ => _.Status == Status.Accepted)
                }).AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id);

            return new OperationResult<WebAppQueryModel>(data);
        }

        private static List<PictureQueryModel> MapPictures(List<Picture> pictures)
        {
            return pictures.Select(_ => new PictureQueryModel
            {
                Id = _.Id,
                Picture = _.FileName,
            }).ToList();
        }

        private static List<CommentQueryModel> MapComments(List<Comment> comments)
        {
            return comments.Where(_ => _.Status == Status.Accepted).Select(_ => new CommentQueryModel
            {
                Id = _.Id,
                Description = _.Description,
                Like = 5,
                UserName = _.User.FullName is null ? _.User.PhoneNumber : _.User.FullName,
                UserIcon = "not implement in domain user",
                CreationDate = _.CreationDate.ToFarsi()
            }).ToList();
        }

        public async Task<List<WebAppQueryModel>> RelatedApps(int id)
        {
            var categoryId = _context.WebApplications.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id).Result.CategoryId;

            return await _context.WebApplications.Include(_ => _.Category).Where(_ => _.CategoryId == categoryId && _.Status == Status.Active && _.Id != id)
                .Select(_ => new WebAppQueryModel
                {
                    Id = _.Id,
                    Name = _.Name,
                    Category = _.Category.Title,
                    Icon = _.Icon,
                }).Take(5).AsNoTracking().ToListAsync();
        }

        public async Task<ResponseDto<WebAppQueryModel>> List(ResponseDto<WebAppQueryModel> response)
        {
            var webApps = _context.WebApplications.Where(_ => _.Status == Status.Active).Include(_ => _.Category).Select(_ => new WebAppQueryModel
            {
                Id = _.Id,
                Name = _.Name,
                Rate = 4f,
                Category = _.Category.Title,
                Icon = _.Icon,
                CreationDate = _.CreationDate.ToFarsi(),
                IsGame = _.IsGame
            }).AsNoTracking();

            if (response.Search is not null)
            {
                webApps = webApps.Where(_ => _.Name.Contains(response.Search)
                || _.Category.Contains(response.Search));
            }

            const int pageSize = 3;
            if (response.Page < 1)
                response.Page = 1;

            int recsCount = webApps.Count();
            var pager = new Pager(recsCount, response.Page, pageSize);

            int recSkip = (response.Page - 1) * pageSize;

            webApps = webApps.Skip(recSkip)
           .Take(pager.PageSize);

            response.Items = await webApps.ToListAsync();
            response.Pager = new(recsCount, response.Page, pageSize);

            return response;
        }
    }
}
