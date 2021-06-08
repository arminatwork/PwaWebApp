﻿using Pwa.Domain.Account;
using Pwa.Domain.Aggregates;
using Pwa.Domain.Base;
using System;
using System.Collections.Generic;
using WebFramework.Enums;

namespace Pwa.Domain.Product
{
    public class WebApplication : BaseDetail, IWebApplicationAggregate
    {
        public int CategoryId { get; private set; }
        public int DeveloperId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string WebSiteAddress { get; private set; }
        public TypeAdd TypeAdd { get; private set; }
        public Status Status { get; private set; }
        public int Visit { get; private set; }
        public int Installed { get; private set; }

        public Category Category { get; private set; }
        public Developer Developer { get; private set; }
        public ICollection<Comment> Comments { get; private set; }
        public ICollection<Picture> Pictures { get; private set; }


        protected WebApplication()
        {

        }

        public WebApplication(string name, string description, string websiteAddress, TypeAdd typeAdd, Status status, int categoryId, int developerId)
        {
            Name = name;
            Description = description;
            WebSiteAddress = websiteAddress;
            TypeAdd = typeAdd;
            Status = status;
            DeActivate();
            CreationDate = DateTime.Now;
            CategoryId = categoryId;
            DeveloperId = developerId;
            Comments = new List<Comment>();
            Pictures = new List<Picture>();
        }

        public void Edit(string name, string description, Status status)
        {
            Name = name;
            Description = description;
            Status = status;
            LastEditDate = DateTime.Now;
        }

        public void IncreaseVisit()
        {
            Visit += 1;
        }
        public void IncreaseInstalled()
        {
            Installed += 1;
        }

        public void Activate()
        {
            Status = Status.Active;
        }

        public void DeActivate()
        {
            Status = Status.DeActive;
        }
    }
}
