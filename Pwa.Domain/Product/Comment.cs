﻿using Pwa.Domain.Account;
using Pwa.Domain.Aggregates;
using Pwa.Domain.Base;
using System;
using WebFramework.Enums;

namespace Pwa.Domain.Product
{
    public class Comment : BaseDetail, ICommentAggregate
    {
        public int UserId { get; private set; }
        public int WebApplicationId { get; private set; }
        public string Description { get; private set; }
        public bool IsDeveloper { get; private set; }
        public Status Status { get; private set; }

        public User User { get; private set; }
        public WebApplication WebApplication { get; private set; }

        protected Comment()
        {

        }

        public Comment(string description, int userId, int webApplicationId)
        {
            Description = description;
            Status = Status.Pending;
            UserId = userId;
            WebApplicationId = webApplicationId;
            CreationDate = DateTime.Now;
            LastEditDate = DateTime.Now;
        }

        public void Edit(string description)
        {
            Description = description;
            LastEditDate = DateTime.Now;
        }

        public void Accept()
        {
            Status = Status.Accepted;
        }

        public void Reject()
        {
            Status = Status.Rejected;
        }
    }
}
